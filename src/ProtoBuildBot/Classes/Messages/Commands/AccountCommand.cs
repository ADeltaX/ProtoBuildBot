using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using System;
using System.Collections.Generic;
using System.Globalization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static ProtoBuildBot.Classes.MessageHelpers;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public class AccountCommand : UserMessageBase
    {
        public override string[] SupportedCommands => new[] { "/account", "/language" };

        public override void HandleCallbackQuery(UserState userState, CallbackQuery callbackQuery, string command, string param)
            => Common(userState, callbackQuery.From, callbackQuery.Message, command, param, false);

        private void Common(UserState userState, User user, Message message, string command, string param, bool sendOnly = false)
        {
            var dateRegistration = SharedDBcmd.UserRegisteredAt(user.Id);
            var dateRegStr = "-";

            if (dateRegistration.HasValue)
                dateRegStr = dateRegistration.Value.ToString("F", userState.CultureInfo);

            if (command == "/account")
            {
                if (!string.IsNullOrEmpty(param) && param == "DELETE")
                {
                    //TODO (2.0)
                    return;
                }

                EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                string.Format(CultureInfo.InvariantCulture,
                                    GetLocalizedText("P_Account", userState),
                                    user.Id, CultureInfo.InvariantCulture.TextInfo.ToTitleCase(userState.AuthLevel.ToString().ToLowerInvariant()),
                                    dateRegStr, userState.CultureInfo.NativeName),
                                InlKeyboardAccount(userState), sendOnly);
            }
            else if (command == "/language")
            {
                if (!string.IsNullOrEmpty(param))
                {
                    SharedDBcmd.UpdateUserLanguage(user.Id, param);
                    SharedDBcmd.UpdateUserState(user.Id);
                }

                EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                string.Format(CultureInfo.InvariantCulture,
                                    GetLocalizedText("P_Language", userState),
                                    userState.CultureInfo.NativeName),
                                InlKeyboardLanguage(userState), sendOnly);
            }
        }

        private static InlineKeyboardMarkup InlKeyboardLanguage(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var availableLanguages = Resources.ResourcesHelpers.GetAvailableLanguages;

            foreach (var item in availableLanguages)
            {
                if (item.Key == userState.CultureInfo.Name)
                {
                    buttons.Add(new InlineKeyboardButton[]
                    {
                        MakeInlButton("🔘 " + availableLanguages[userState.CultureInfo.Name], stringId, "/language", "")
                    });
                }
                else
                {
                    buttons.Add(new InlineKeyboardButton[]
                    {
                        MakeInlButton("⚫️ " + item.Value, stringId, "/language", item.Key)
                    });
                }                
            }

            buttons.Add(new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/account") });
            
            return new InlineKeyboardMarkup(buttons);
        }

        private static InlineKeyboardMarkup InlKeyboardAccount(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            var delAccount = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_DeleteAccount", userState), stringId, "/account", "DELETE"), //TODO (2.0)
                MakeInlButton(GetLocalizedText("B_ChangeLanguage", userState), stringId, "/language")
            };

            //BACK BUTTON!
            var backButton = new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/start") };

            return new InlineKeyboardMarkup(new InlineKeyboardButton[][] { delAccount, backButton });
        }
    }
}
