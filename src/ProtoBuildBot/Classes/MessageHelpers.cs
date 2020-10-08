using ProtoBuildBot.DataStore;
using System;
using System.Globalization;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ProtoBuildBot.Classes
{
    public static class MessageHelpers
    {
		public static InlineKeyboardButton GetDefaultStartButton(UserStateBase userState)
			=> MakeInlButton(GetLocalizedText("B_Start", userState.CultureInfo), userState is UserState ? userState.GetID().ToString(CultureInfo.InvariantCulture) : "GROUP", "/start");

		public static InlineKeyboardButton GetDefaultBackButton(UserStateBase userState, string command, params string[] param)
			=> MakeInlButton(GetLocalizedText("B_Back", userState.CultureInfo), userState is UserState ? userState.GetID().ToString(CultureInfo.InvariantCulture) : "GROUP", command, param);

		public static bool DeleteMessageText(long chatId, int messageId)
			=> ExecuteAndLogErrors(() => TGHost.Bot.DeleteMessageAsync(chatId, messageId));

		public static bool SendMessageText(long chatId, string htmlMessage, InlineKeyboardMarkup markup = null)
			=> ExecuteAndLogErrors(() => TGHost.Bot.SendTextMessageAsync(chatId, htmlMessage, ParseMode.Html, false, false, 0, markup).ConfigureAwait(false).GetAwaiter().GetResult());

		public static bool EditOrSendMessageText(long chatId, int messageId, string htmlMessage, InlineKeyboardMarkup markup = null, bool sendOnly = false)
		{
			if (sendOnly || !ExecuteAndLogErrors(() => TGHost.Bot.EditMessageTextAsync(chatId, messageId, htmlMessage, ParseMode.Html, false, markup).ConfigureAwait(false).GetAwaiter().GetResult()))
			{
				return SendMessageText(chatId, htmlMessage, markup);
			}
			else
				return true;
        }

		public static InlineKeyboardButton MakeInlButton(string text, string userId, string command, params string[] param) 
			=> new InlineKeyboardButton { Text = text, CallbackData = $"{TelegramBotSettings.BotMessageVersion}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}-{userId}-{command}{ComposeMultipleParameters(param)}" };

		public static string GetLocalizedText(string name, UserStateBase userState) 
			=> Language.Resources.ResourceManager.GetString(name, userState?.CultureInfo ?? throw new ArgumentNullException(nameof(userState)));

		public static string GetLocalizedText(string name, CultureInfo cultureInfo)
			=> Language.Resources.ResourceManager.GetString(name, cultureInfo ?? throw new ArgumentNullException(nameof(cultureInfo)));

		private static string ComposeMultipleParameters(string[] param)
		{
			string str = "";
			for (int i = 0; i < param.Length; i++)
				str += "+" + param[i];

			return str;
		}

		public static bool IsMessageFromGroup(Telegram.Bot.Types.Message message) 
			=> message?.Chat.Type == ChatType.Group || message?.Chat.Type == ChatType.Supergroup;

		private static bool ExecuteAndLogErrors(Action action)
		{
			try
			{
				action();
				return true;
			}
			catch (MessageIsNotModifiedException ex)
			{
				Logger.BotLogger.LogInfo(ex.Message, "MESSAGE_HELPER");
				return true;
			}
			catch (Exception ex)
			{
				Logger.BotLogger.LogError(ex.Message, "MESSAGE_HELPER");
				return false;
			}
		}
    }
}
