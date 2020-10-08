using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using System;
using System.Globalization;
using System.Linq;
using Telegram.Bot.Types;
using static ProtoBuildBot.Classes.Localization.English;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class ChangeAuthCommand : UserMessageBase
    {
        public override string[] SupportedCommands => new[] { "/changeauth", "/upgradeuser" };

        public override bool IsAuthorizationTargeted => false;

        public override async void HandleCommandMessage(UserState userState, Message message, string command)
        {
            //TODO: REWAMP THIS (2.0)
            //Because you can.....

            try
            {
                SharedDBcmd.AddToHistoryCommands(message.From.Id, message.Text);

                if (message.Text.Trim().Length == command.Length)
                    await TGHost.Bot.SendTextMessageAsync(message.From.Id, MessageHelpers.GetLocalizedText("Z_CommandIncomplete", CultureInfo.InvariantCulture)).ConfigureAwait(false);
                else
                {
                    if (message.Text.ToUpperInvariant().Contains("@", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var msg = message.Text.ToUpperInvariant().Substring(command.Length).Trim();

                        if (msg.Contains("USER", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var idToUpgrade = msg.Split('@').Last();
                            SharedDBcmd.UpdateUserAuthLevel(int.Parse(idToUpgrade, CultureInfo.InvariantCulture), 1);
                            SharedDBcmd.UpdateUserState(int.Parse(idToUpgrade, CultureInfo.InvariantCulture));
                            await TGHost.Bot.SendTextMessageAsync(idToUpgrade, "You are now an \"user\".").ConfigureAwait(false);
                        }
                        else if (msg.Contains("MOD", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var idToUpgrade = msg.Split('@').Last();
                            SharedDBcmd.UpdateUserAuthLevel(int.Parse(idToUpgrade, CultureInfo.InvariantCulture), 2);
                            SharedDBcmd.UpdateUserState(int.Parse(idToUpgrade, CultureInfo.InvariantCulture));
                            await TGHost.Bot.SendTextMessageAsync(idToUpgrade, "You are now a \"mod\"!").ConfigureAwait(false);
                        }
                        else if (msg.Contains("ADMIN", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var idToUpgrade = msg.Split('@').Last();
                            SharedDBcmd.UpdateUserAuthLevel(int.Parse(idToUpgrade, CultureInfo.InvariantCulture), 3);
                            SharedDBcmd.UpdateUserState(int.Parse(idToUpgrade, CultureInfo.InvariantCulture));
                            await TGHost.Bot.SendTextMessageAsync(idToUpgrade, "Your are now an \"admin\"!").ConfigureAwait(false);
                        }
                        else
                        {
                            await TGHost.Bot.SendTextMessageAsync(message.From.Id, MessageHelpers.GetLocalizedText("Z_CommandIncomplete", CultureInfo.InvariantCulture)).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await TGHost.Bot.SendTextMessageAsync(message.From.Id, MessageHelpers.GetLocalizedText("Z_CommandIncomplete", CultureInfo.InvariantCulture)).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception)
            {
                
            }
        }
    }
}
