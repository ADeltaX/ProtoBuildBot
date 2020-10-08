using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using System.Text;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class GenKeyCommand : ModMessageBase
    {
        //TODO (2.0): make interactive version

        public override string[] SupportedCommands => new[] { "/genkey" };

        public override bool IsAuthorizationTargeted => false;

        public override async void HandleCommandMessage(UserState userState, Message message, string command)
        {
            if (message.Text.Length == 7)
                await TGHost.Bot.SendTextMessageAsync(message.From.Id, ProductActivationSystem.GenerateNewKey()).ConfigureAwait(false);
            else
            {
                var numStr = message.Text.Substring(7);
                if (uint.TryParse(numStr, out uint num))
                {
                    if (num > 10)
                        num = 10;

                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < num; i++)
                        sb.AppendLine(ProductActivationSystem.GenerateNewKey());

                    await TGHost.Bot.SendTextMessageAsync(message.From.Id, sb.ToString()).ConfigureAwait(false);
                }
                else
                {
                    await TGHost.Bot.SendTextMessageAsync(message.From.Id, ProductActivationSystem.GenerateNewKey()).ConfigureAwait(false);
                }
            }

            SharedDBcmd.AddToHistoryCommands(message.From.Id, message.Text);
        }
    }
}
