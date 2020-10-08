using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class GenerateTimeLinkCommand : ModMessageBase
    {
        //TODO (2.0): used for debugging

        public override string[] SupportedCommands => new[] { "/gg" };

        public override bool IsAuthorizationTargeted => false;

        public override async void HandleCommandMessage(UserState userState, Message message, string command)
        {
            try
            {
#if DEBUG
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, $"https://{TelegramBotSettings.DevHost}/v1/getallringstates?t=" + TimeLimitGeneration.GenerateNewHashTime(TimeSpan.FromMinutes(5))).ConfigureAwait(false);
#else
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, $"https://{TelegramBotSettings.ProdHost}/v1/getallringstates?t=" + TimeLimitGeneration.GenerateNewHashTime(TimeSpan.FromMinutes(5))).ConfigureAwait(false);
#endif
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
                return;
            }
        }
    }
}
