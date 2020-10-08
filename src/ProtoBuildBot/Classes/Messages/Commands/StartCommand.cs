using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System.Globalization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static ProtoBuildBot.Classes.MessageHelpers;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class StartCommand : UserMessageBase
    {
        public override string[] SupportedCommands => new[] { "/help", "/start" };
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.UNREGISTERED;
        public override bool IsGroupSupported => true;

        public override void HandleCommandMessageFromGroup(GroupState groupState, Message message, string command)
            => CommonGroup(groupState, message, true);

        public override void HandleCallbackQueryFromGroup(GroupState groupState, CallbackQuery callbackQuery, string command, string param)
            => CommonGroup(groupState, callbackQuery.Message, false);

        public override void HandleCallbackQuery(UserState userState, CallbackQuery callbackQuery, string command, string param) 
            => CommonPrivate(userState, callbackQuery.From, callbackQuery.Message, false);

        public override void HandleCommandMessage(UserState userState, Message message, string command) 
            => CommonPrivate(userState, message.From, message, true);

        private void CommonGroup(GroupState groupState, Message message, bool sendOnly = false)
        {
            //check if group is registered.
            if (groupState.IsRegistered)
            {
                EditOrSendMessageText(groupState.ChatId, message.MessageId,
                                        string.Format(CultureInfo.InvariantCulture, GetLocalizedText("P_HelpGroup", groupState)),
                                        Group(groupState), sendOnly);
            }
        }

        private void CommonPrivate(UserState userState, User user, Message message, bool sendOnly = false)
        {
            switch (userState.AuthLevel)
            {
                case AuthLevel.USER:
                    EditOrSendMessageText(user.Id, message.MessageId, 
                                        string.Format(CultureInfo.InvariantCulture, GetLocalizedText("P_HelpUser", userState),
                                        user.Id, user.FirstName),
                                        User(userState), sendOnly);
                    break;
                case AuthLevel.MOD:
                    EditOrSendMessageText(user.Id, message.MessageId,
                                        string.Format(CultureInfo.InvariantCulture, GetLocalizedText("P_HelpMod", userState),
                                        user.Id, user.FirstName),
                                        Mod(userState), sendOnly);
                    break;
                case AuthLevel.ADMIN:
                    EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                        string.Format(CultureInfo.InvariantCulture, GetLocalizedText("P_HelpAdmin", userState),
                                        user.Id, user.FirstName),
                                        Admin(userState), sendOnly);
                    break;
                case AuthLevel.CREATOR:
                    EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                        string.Format(CultureInfo.InvariantCulture, GetLocalizedText("P_HelpCreator", userState),
                                        user.Id, user.FirstName),
                                        Creator(userState), sendOnly);
                    break;
            }
        }

        private static InlineKeyboardMarkup Group(GroupState groupState)
        {
            var helpLine = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Updates", groupState), "GROUP", "/getringstates"),
                MakeInlButton(GetLocalizedText("B_Notifications", groupState), "GROUP", "/subscribe"),
                MakeInlButton(GetLocalizedText("B_BotSettings", groupState), "GROUP", "/groupsettings")
            };

            return new InlineKeyboardMarkup(helpLine);
        }

        //TODO: ADD INFO IN HELP PAGE ABOUT "HOW TO GET NOTIFICATIONS ABOUT UPDATES"
        //TODO: FEEDBACKS

        private static InlineKeyboardMarkup User(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            var helpLine1 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Web", userState), stringId, "/web"), //TODO: API key
                MakeInlButton(GetLocalizedText("B_Updates", userState), stringId, "/getringstates"),
            };

            var helpLine2 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Notifications", userState), stringId, "/subscribe"),
                MakeInlButton(GetLocalizedText("B_Account", userState), stringId, "/account")
            };

            var UWU = new InlineKeyboardButton[][] { helpLine1, helpLine2 };

            return new InlineKeyboardMarkup(UWU);
        }

        private static InlineKeyboardMarkup Mod(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            var helpLine1 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Web", userState), stringId, "/web"), //TODO: API key
                MakeInlButton(GetLocalizedText("B_Updates", userState), stringId, "/getringstates"),
                MakeInlButton(GetLocalizedText("B_Search", userState), stringId, "/search")
            };

            var helpLine2 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Notifications", userState), stringId, "/subscribe"),
                MakeInlButton(GetLocalizedText("B_Account", userState), stringId, "/account")
            };

            var UWU = new InlineKeyboardButton[][] { helpLine1, helpLine2 };

            return new InlineKeyboardMarkup(UWU);
        }

        private static InlineKeyboardMarkup Admin(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            var helpLine1 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Web", userState), stringId, "/web"), //TODO: API key
                MakeInlButton(GetLocalizedText("B_Updates", userState), stringId, "/getringstates"),
                MakeInlButton(GetLocalizedText("B_Search", userState), stringId, "/search")
            };

            var helpLine2 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_BotSettings", userState), stringId, "/settings"), //TODO: In settings add: Generate keys, DB errors, search settings (?), uptime, reboot, update, Account hierarchy
                MakeInlButton(GetLocalizedText("B_Notifications", userState), stringId, "/subscribe"),
                MakeInlButton(GetLocalizedText("B_Account", userState), stringId, "/account")
            };

            var UWU = new InlineKeyboardButton[][] { helpLine1, helpLine2 };

            return new InlineKeyboardMarkup(UWU);
        }

        private static InlineKeyboardMarkup Creator(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            var helpLine1 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Web", userState), stringId, "/web"), //TODO: API key
                MakeInlButton(GetLocalizedText("B_Updates", userState), stringId, "/getringstates"),
                MakeInlButton(GetLocalizedText("B_Search", userState), stringId, "/search")
            };

            var helpLine2 = new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_BotSettings", userState), stringId, "/settings"), //TODO: In settings add: Generate keys, DB errors, search settings (?), uptime, reboot, update, Account hierarchy
                MakeInlButton(GetLocalizedText("B_Notifications", userState), stringId, "/subscribe"),
                MakeInlButton(GetLocalizedText("B_Account", userState), stringId, "/account") 
            };

            var UWU = new InlineKeyboardButton[][] { helpLine1, helpLine2 };

            return new InlineKeyboardMarkup(UWU);
        }
    }
}
