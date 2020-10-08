using ProtoBuildBot.Classes.Automation;
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
    public sealed class SearchCommand : ModMessageBase
    {
        public override string[] SupportedCommands => new[] { "/search" };

        public override bool IsAuthorizationTargeted => false;

        public override void HandleCallbackQuery(UserState userState, CallbackQuery callbackQuery, string command, string param)
            => Common(userState, callbackQuery.From, callbackQuery.Message, command, param, false);

        private void Common(UserState userState, User user, Message message, string command, string param, bool sendOnly = false)
        {
            if (!string.IsNullOrEmpty(param))
            {
                var splitted = param.Split('+');
                if (splitted.Length == 1)
                {
                    //Ring Page
                    EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                string.Format(CultureInfo.InvariantCulture, 
                                GetLocalizedText("P_SearchRing", userState),
                                    splitted[0].Equals("@EV", StringComparison.Ordinal) ? GetLocalizedText("W_Everything", userState) : splitted[0]), //Everything
                                InlKeyboardRingPage(userState, splitted[0]), sendOnly);
                }
                else if (splitted.Length == 2)
                {
                    //Confirmation Page
                    EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                string.Format(CultureInfo.InvariantCulture,
                                GetLocalizedText("P_SearchConfirm", userState),
                                    splitted[0].Equals("@EV", StringComparison.Ordinal) ? GetLocalizedText("W_Everything", userState) : splitted[0],
                                    splitted[1].Equals("@EVR", StringComparison.Ordinal) ? GetLocalizedText("W_AllRings", userState) :
                                        splitted[1].Equals("@AR", StringComparison.Ordinal) ? GetLocalizedText("W_AllApplicableRings", userState) : splitted[1]),
                                InlKeyboardConfirmPage(userState, splitted[0], splitted[1]), sendOnly);
                }
                else if (splitted.Length == 3)
                {
                    //Searching Page
                    if (!SearchAutomation.IsRunning)
                    {
                        var searchItems = SearchAutomation.GetFilteredSearchItems(SearchHelpers.GetSearchItems, splitted[0], splitted[1]);
                        EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                GetLocalizedText("P_SearchingNow", userState),
                                InlKeyboardSearchingPage(userState), sendOnly);

                        SharedDBcmd.AddToHistoryCommands(user.Id, command + " " + param);

                        SearchAutomation.RunSearch(searchItems);

                        SendMessageText(message.Chat.Id, 
                                GetLocalizedText("P_SearchCompleted", userState), 
                                InlKeyboardSearchingPage(userState));
                    }
                    else
                        EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                string.Format(CultureInfo.InvariantCulture, 
                                GetLocalizedText("P_AlreadySearching", userState),
                                    (DateTime.UtcNow - SearchAutomation.LastExecutedAt).TotalMinutes.ToString("0.00", userState.CultureInfo),
                                    splitted[0].Equals("@EV", StringComparison.Ordinal) ? GetLocalizedText("W_Everything", userState) : splitted[0],
                                    splitted[1].Equals("@EVR", StringComparison.Ordinal) ? GetLocalizedText("W_AllRings", userState) :
                                        splitted[1].Equals("@AR", StringComparison.Ordinal) ? GetLocalizedText("W_AllApplicableRings", userState) : splitted[1]),
                                InlKeyboardConfirmPage(userState, splitted[0], splitted[1]), sendOnly);

                    return;
                }
            }
            else
            {
                EditOrSendMessageText(message.Chat.Id, message.MessageId,
                                GetLocalizedText("P_Search", userState),
                                InlKeyboardMainPage(userState), sendOnly);
            }
        }

        public override void HandleCommandMessage(UserState userState, Message message, string command) 
            => SendMessageText(message.Chat.Id, GetLocalizedText("P_UseInteractiveVersion", userState), GetDefaultStartButton(userState));

        private static InlineKeyboardMarkup InlKeyboardSearchingPage(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            return new InlineKeyboardMarkup(new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_Back", userState), stringId, "/start")
            });
        }

        private static InlineKeyboardMarkup InlKeyboardConfirmPage(UserState userState, string deviceFamily, string ring)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var searchRings = SearchAutomation.GetSearchableRings(SearchHelpers.GetSearchItems, deviceFamily);

            buttons.Add(new InlineKeyboardButton[]
            {
                MakeInlButton(GetLocalizedText("B_SearchNow", userState), stringId, "/search", deviceFamily, ring, "@CO")
            });

            buttons.Add(new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/search", deviceFamily) });

            return new InlineKeyboardMarkup(buttons);
        }

        private static InlineKeyboardMarkup InlKeyboardRingPage(UserState userState, string deviceFamily)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var searchRings = SearchAutomation.GetSearchableRings(SearchHelpers.GetSearchItems, deviceFamily);

            foreach (var ring in searchRings)
            {
                buttons.Add(new InlineKeyboardButton[]
                {
                    MakeInlButton(ring.Equals("@AR", StringComparison.Ordinal) ? GetLocalizedText("B_AllApplicableRings", userState) : ring, stringId, "/search", deviceFamily, ring)
                });
            }

            if (searchRings.Length > 1)
            {
                buttons.Add(new InlineKeyboardButton[]
                {
                    MakeInlButton(GetLocalizedText("B_AllRings", userState), stringId, "/search", deviceFamily + "+" + "@EVR")
                });
            }

            buttons.Add(new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/search") });

            return new InlineKeyboardMarkup(buttons);
        }

        private static InlineKeyboardMarkup InlKeyboardMainPage(UserState userState)
        {
            var stringId = userState.UserId.ToString(CultureInfo.InvariantCulture);

            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();

            var searchFamil = SearchAutomation.GetSearchableFamilies();

            foreach (var family in searchFamil)
            {
                var famStr = family;

                if (SearchHelpers.GetEmojiIconFromDeviceFamily.TryGetValue(family, out var icon))
                    famStr = icon + " " + family;

                buttons.Add(new InlineKeyboardButton[]
                {
                    MakeInlButton(famStr, stringId, "/search", family)
                });
            }

            if (searchFamil.Length > 1)
            {
                buttons.Add(new InlineKeyboardButton[]
                {
                    MakeInlButton(GetLocalizedText("B_AllDeviceFamilies", userState), stringId, "/search", "@EV") //EVERYTHING
                });
            }

            buttons.Add(new InlineKeyboardButton[] { GetDefaultBackButton(userState, "/start") });

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
