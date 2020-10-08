using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class UpdateCookieCommand : AdminMessageBase
    {
        public override string[] SupportedCommands => new[] { "/updatecookie", "/uc" };

        public override async void HandleCommandMessage(UserState userState, Message message, string command)
        {
            try
            {
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "Updating cookies...").ConfigureAwait(false);
                await Automation.SearchAutomation.UpdateCookie().ConfigureAwait(false);
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "...done!").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] {ex.Message}");
                SharedDBcmd.TraceError(-1, $"Error: {ex.Message}");
            }
        }
    }
}
