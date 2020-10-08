using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static ProtoBuildBot.Classes.MessageHelpers;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public class GroupSettingsCommand : CreatorMessageBase
    {
        public override string[] SupportedCommands => new[] { "/groupsettings", "/grouplanguage" };
        public override bool IsGroupSupported => true;
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.MOD;

        public override void HandleCallbackQueryFromGroup(GroupState groupState, CallbackQuery callbackQuery, string command, string param)
        {
            if (groupState.IsRegistered)
            {

                //TODO: language
                if (command == "/groupsettings")
                {
                    EditOrSendMessageText(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId,
                            "PLEB",
                            InlKeyboardAccount(groupState));
                }
                else if (command == "/grouplanguage")
                {
                    if (!string.IsNullOrEmpty(param))
                        SharedDBcmd.UpdateGroupLanguage(groupState.ChatId, param);

                    EditOrSendMessageText(callbackQuery.Message.Chat.Id,
                            callbackQuery.Message.MessageId,
                            "PLEBV2",
                            InlKeyboardLanguage(groupState));
                }
            }
        }

        private static InlineKeyboardMarkup InlKeyboardLanguage(UserStateBase userState)
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var availableLanguages = Resources.ResourcesHelpers.GetAvailableLanguages;

            foreach (var item in availableLanguages)
            {
                if (item.Key == userState.CultureInfo.Name)
                {
                    buttons.Add(new InlineKeyboardButton[]
                    {
                        MakeInlButton("🔘 " + availableLanguages[userState.CultureInfo.Name], "GROUP", "/grouplanguage", "")
                    });
                }
                else
                {
                    buttons.Add(new InlineKeyboardButton[]
                    {
                        MakeInlButton("⚫️ " + item.Value, "GROUP", "/grouplanguage", item.Key)
                    });
                }
            }

            buttons.Add(new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/groupsettings") });

            return new InlineKeyboardMarkup(buttons);
        }

        private static InlineKeyboardMarkup InlKeyboardAccount(UserStateBase userState)
        {
            var changeLanguage = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_ChangeLanguage", userState), "GROUP", "/grouplanguage")
            };

            var backButton = new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/start") };

            return new InlineKeyboardMarkup(new InlineKeyboardButton[][] { changeLanguage, backButton });
        }
    }
}
