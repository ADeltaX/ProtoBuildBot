using ProtoBuildBot.Classes.Automation;
using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static ProtoBuildBot.Classes.MessageHelpers;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public class GetRingStatesCommand : UnregMessageBase
    {
        public override string[] SupportedCommands => new[] { "/getringstates", "/grs" };

        public override bool IsAuthorizationTargeted => false;
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.UNREGISTERED;

        public override bool IsGroupSupported => true;

        public override void HandleCallbackQueryFromGroup(GroupState groupState, CallbackQuery callbackQuery, string command, string param)
        {
            if (groupState.IsRegistered)
                Common(groupState, callbackQuery.From, callbackQuery.Message, command, param, false);
        }

        public override void HandleCallbackQuery(UserState userState, CallbackQuery callbackQuery, string command, string param)
            => Common(userState, callbackQuery.From, callbackQuery.Message, command, param, false);

        private void Common(UserStateBase userState, User user, Message message, string command, string param, bool sendOnly = false)
        {
            if (!string.IsNullOrEmpty(param))
            {
                var grs = SharedDBcmd.GetLatestBuildLabForeachBuildFrom();

                StringBuilder sb = new StringBuilder();

                if (param == "EVERYTHING")
                    grs.ForEach(itm => {

                        if (SearchHelpers.GetSearchItemsForGRS.FirstOrDefault(u => u.DeviceFamily.Split('-')[0].Equals(itm.DeviceFamily.Split('-')[0], StringComparison.OrdinalIgnoreCase) &&
                                                                                    u.Ring.Equals(itm.Ring, StringComparison.OrdinalIgnoreCase) &&
                                                                                    u.Arch.Equals(itm.Architecture, StringComparison.OrdinalIgnoreCase)) != null)
                        {
                            if (itm.DeviceFamily.Contains('-', StringComparison.Ordinal))
                                sb.Append($"➡️ <b>{itm.DeviceFamily.Split('-')[0]}</b> ({itm.DeviceFamily.Split('-')[1]}) - <b>{itm.Ring}</b>:\n <code>{itm.BuildLab}</code>\n");
                            else
                                sb.Append($"➡️ <b>{itm.DeviceFamily}</b> - <b>{itm.Ring}</b>:\n <code>{itm.BuildLab}</code>\n");
                        }
                    });
                else
                {
                    var specificFamily = grs.Where(f => f.DeviceFamily.ToUpperInvariant().StartsWith(param, StringComparison.Ordinal)).ToList();
                    var specificReferenceFilter = SearchHelpers.GetSearchItemsForGRS.Where(u => u.DeviceFamily.ToUpperInvariant().Split('-')[0] == param).ToList();

                    specificFamily.ForEach(itm => {
                        if (specificReferenceFilter.FirstOrDefault(u => u.Ring.ToUpperInvariant() == itm.Ring.ToUpperInvariant() && u.Arch.ToUpperInvariant() == itm.Architecture.ToUpperInvariant()) != null)
                        {
                            if (itm.DeviceFamily.Contains('-', StringComparison.Ordinal))
                                sb.Append($"➡️ <b>{itm.Ring}</b> ({itm.DeviceFamily.Split('-')[1]}):\n <code>{itm.BuildLab}</code>\n\n");
                            else
                                sb.Append($"➡️ <b>{itm.Ring}</b>:\n <code>{itm.BuildLab}</code>\n\n");
                        }
                    });
                }

                if (sb.Length == 0)
                    sb.Append("-");

                EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                string.Format(CultureInfo.InvariantCulture,
                                GetLocalizedText("P_DetailedUpdates", userState),
                                    param, sb.ToString()),
                                InlKeyboardDetailedPage(userState), sendOnly);
            }
            else
            {
                EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                GetLocalizedText("P_Updates", userState),
                                InlKeyboardMainPage(userState), sendOnly);
            }
        }

        private static InlineKeyboardMarkup InlKeyboardDetailedPage(UserStateBase userState)
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>
            {
                new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/getringstates") }
            };

            return new InlineKeyboardMarkup(buttons);
        }

        private static InlineKeyboardMarkup InlKeyboardMainPage(UserStateBase userState)
        {
            var stringId = userState is UserState ? userState.GetID().ToString(CultureInfo.InvariantCulture) : "GROUP";

            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var grs = SharedDBcmd.GetLatestBuildLabForeachBuildFrom();
            var families = grs.GroupBy(f => f.DeviceFamily.Split("-")[0]);

            foreach (var family in families)
            {
                //Everything that MS messed up ends up here
                if (family.Key.StartsWith("MESSED", StringComparison.OrdinalIgnoreCase))
                    continue;

                var famStr = family.Key;

                if (SearchHelpers.GetEmojiIconFromDeviceFamily.TryGetValue(family.Key, out var icon))
                    famStr = icon + " " + family.Key;

                buttons.Add(new InlineKeyboardButton[]
                {
                    MakeInlButton(famStr, stringId, "/getringstates", family.Key)
                });
            }

            if (families.Count() > 1)
            {
                buttons.Add(new InlineKeyboardButton[]
                {
                    MakeInlButton(GetLocalizedText("B_Everything", userState), stringId, "/getringstates", "EVERYTHING")
                });
            }

            buttons.Add(new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/start") });

            return new InlineKeyboardMarkup(buttons);
        }

        public override async void HandleCommandMessage(UserState userState, Message message, string command) 
            => SendMessageText(message.Chat.Id, GetLocalizedText("P_UseInteractiveVersion", userState), GetDefaultStartButton(userState));

        private static bool IsMatch(string item, string filter) 
            => filter.Contains(item.Split('-')[0], StringComparison.OrdinalIgnoreCase);
    }
}
