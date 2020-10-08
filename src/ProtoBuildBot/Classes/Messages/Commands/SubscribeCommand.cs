using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static ProtoBuildBot.Classes.MessageHelpers;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class SubscribeCommand : UserMessageBase
    {
        public override string[] SupportedCommands => new[] { "/subscribe", "/unsubscribe" };

        public override bool IsAuthorizationTargeted => false;

        public override bool IsGroupSupported => true;
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.MOD;

        public override void HandleCallbackQuery(UserState userState, CallbackQuery callbackQuery, string command, string param)
            => Common(userState, callbackQuery.From, callbackQuery.Message, false, param);

        public override void HandleCallbackQueryFromGroup(GroupState groupState, CallbackQuery callbackQuery, string command, string param)
        {
            if (groupState.IsRegistered)
                Common(groupState, callbackQuery.From, callbackQuery.Message, false, param);
        }

        public override async void HandleCommandMessage(UserState userState, Message message, string command)
        {
            try
            {
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "Please use the interactive version, /start").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogWarning(ex.Message, "SUBSCRIBE_COMMAND");
            }
        }

        private void Common(UserStateBase userState, User user, Message message, bool sendOnly, string param = null)
        {
            if (!string.IsNullOrEmpty(param))
                HandleSubscription(userState, param);

            EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                GetLocalizedText("P_Subscribe", userState),
                                InlKeyboard(userState), sendOnly);
        }

        private static void HandleSubscription(UserStateBase userState, string command)
        {
            var subscribableDeviceFamily = SearchHelpers.GetSearchItemsForGRS.GroupBy(item => item.DeviceFamily).Select(itemSelect => itemSelect.Key).ToArray();

            bool isGroup = userState is GroupState;

            var subscribed = isGroup ? SharedDBcmd.IsGroupSubscribed(userState.GetID()) : SharedDBcmd.IsUserSubscribed(userState.GetID());

            string subscribedTo = null;

            if (isGroup && subscribed)
            {
                string sub = SharedDBcmd.GetGroupFilterSubscriber(userState.GetID());
                subscribedTo = sub ?? "";
            }
            else if (!isGroup && subscribed)
            {
                string sub = SharedDBcmd.GetUserFilterSubscriber(userState.GetID());
                subscribedTo = sub ?? "";
            }

            if (command == "+ALL")
            {
                if (!subscribed)
                {
                    if (isGroup)
                        SharedDBcmd.AddNewGroupSubscriber(userState.GetID(), string.Join(',', subscribableDeviceFamily));
                    else
                        SharedDBcmd.AddNewUserSubscriber(userState.GetID(), string.Join(',', subscribableDeviceFamily));
                }
                else
                {
                    if (isGroup)
                        SharedDBcmd.UpdateGroupFilterSubscriber(userState.GetID(), string.Join(',', subscribableDeviceFamily));
                    else
                        SharedDBcmd.UpdateUserFilterSubscriber(userState.GetID(), string.Join(',', subscribableDeviceFamily));
                }
            }
            else if (command == "-ALL")
            {
                if (subscribed)
                {
                    if (isGroup)
                        SharedDBcmd.RemoveGroupSubscriber(userState.GetID());
                    else
                        SharedDBcmd.RemoveUserSubscriber(userState.GetID());
                }
            }
            else if (command.StartsWith('+'))
            {
                string devFamily = subscribableDeviceFamily.FirstOrDefault(item => item == command.Replace("+", "", StringComparison.Ordinal));
                if (devFamily != null)
                {
                    if (!subscribed)
                    {
                        if (isGroup)
                            SharedDBcmd.AddNewGroupSubscriber(userState.GetID(), devFamily);
                        else
                            SharedDBcmd.AddNewUserSubscriber(userState.GetID(), devFamily);
                    }
                    else
                    {
                        if (!subscribedTo.Split(',').Contains(devFamily))
                        {
                            if (isGroup)
                                SharedDBcmd.UpdateGroupFilterSubscriber(userState.GetID(), string.Join(',', subscribedTo) + (string.IsNullOrEmpty(subscribedTo) ? "" : ",") + devFamily);
                            else
                                SharedDBcmd.UpdateUserFilterSubscriber(userState.GetID(), string.Join(',', subscribedTo) + (string.IsNullOrEmpty(subscribedTo) ? "" : ",") + devFamily);
                        }
                    }
                }
            }
            else if (command.StartsWith('-'))
            {
                if (subscribed)
                {
                    string devFamily = subscribableDeviceFamily.FirstOrDefault(item => item == command.Replace("-", "", StringComparison.Ordinal));
                    if (devFamily != null)
                    {
                        if (subscribedTo.Split(',').Contains(devFamily))
                        {
                            if (subscribedTo.Length == 1)
                            {
                                if (isGroup)
                                    SharedDBcmd.RemoveGroupSubscriber(userState.GetID());
                                else
                                    SharedDBcmd.RemoveUserSubscriber(userState.GetID());
                            }
                            else
                            {
                                //awful re-split
                                var sPlItTeD = string.Join(',', subscribedTo.Split(',').Where(c => c != devFamily));
                                if (isGroup)
                                    SharedDBcmd.UpdateGroupFilterSubscriber(userState.GetID(), sPlItTeD);
                                else
                                    SharedDBcmd.UpdateUserFilterSubscriber(userState.GetID(), sPlItTeD);
                            }
                        }
                    }
                }
            }
        }

        private static InlineKeyboardMarkup InlKeyboard(UserStateBase userState)
        {
            bool isGroup = userState is GroupState;
            var stringId = isGroup ? "GROUP" : userState.GetID().ToString(CultureInfo.InvariantCulture);

            //ARRAY of devices (with statuses)

            List<InlineKeyboardButton[]> meme = new List<InlineKeyboardButton[]>();

            var subscribableDeviceFamily = SearchHelpers.GetSearchItemsForGRS.GroupBy(item => item.DeviceFamily).Select(itemSelect => itemSelect.Key).ToArray();

            string subscribedTo = null;

            if (isGroup && SharedDBcmd.IsGroupSubscribed(userState.GetID()))
            {
                string sub = SharedDBcmd.GetGroupFilterSubscriber(userState.GetID());
                subscribedTo = sub ?? "";
            }
            else if (SharedDBcmd.IsUserSubscribed(userState.GetID()))
            {
                string sub = SharedDBcmd.GetUserFilterSubscriber(userState.GetID());
                subscribedTo = sub ?? "";
            }

            string[] subscribedToFamily = subscribedTo?.Split(',');

            var subscribedDeviceFamilyFlags = new bool[subscribableDeviceFamily.Length];

            for (int i = 0; i < subscribableDeviceFamily.Length; i++)
            {
                bool? exist = subscribedToFamily?.Contains(subscribableDeviceFamily[i], StringComparer.InvariantCultureIgnoreCase);

                subscribedDeviceFamilyFlags[i] = exist.HasValue && exist.Value;

                var symbol = subscribedDeviceFamilyFlags[i] ? "-" : "+";
                var symbolChat = subscribedDeviceFamilyFlags[i] ? "✅" : "❌";

                meme.Add(new InlineKeyboardButton[]
                {
                    MakeInlButton(symbolChat + " " + subscribableDeviceFamily[i], stringId, "/subscribe", symbol + subscribableDeviceFamily[i])
                });
            }

            List<InlineKeyboardButton> fixedBottomButtons = new List<InlineKeyboardButton>();

            if (subscribedDeviceFamilyFlags.Contains(false))
                fixedBottomButtons.Add(MakeInlButton(GetLocalizedText("B_SubscribeAll", userState.CultureInfo), stringId, "/subscribe", "+ALL"));
            else
                fixedBottomButtons.Add(MakeInlButton(GetLocalizedText("B_AllSubscribed", userState.CultureInfo), stringId, "/subscribe"));

            if (subscribedDeviceFamilyFlags.Contains(true))
                fixedBottomButtons.Add(MakeInlButton(GetLocalizedText("B_UnsubscribeAll", userState.CultureInfo), stringId, "/subscribe", "-ALL"));
            else
                fixedBottomButtons.Add(MakeInlButton(GetLocalizedText("B_AllUnsubscribed", userState.CultureInfo), stringId, "/subscribe"));

            meme.Add(fixedBottomButtons.ToArray());

            meme.Add(new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/start") });

            return new InlineKeyboardMarkup(meme);
        }
    }
}
