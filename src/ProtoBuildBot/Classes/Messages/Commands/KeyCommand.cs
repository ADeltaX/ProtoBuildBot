using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static ProtoBuildBot.Classes.Localization.English;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class KeyCommand : UnregMessageBase
    {
        //TODO: use resx

        public override string[] SupportedCommands => new[] { "/key" };

        public override bool IsAuthorizationTargeted => false;

        public override async void HandleCommandMessage(UserState userState, Message message, string command)
        {
            switch (userState.AuthLevel)
            {
                case AuthLevel.UNREGISTERED:
                    if (message.Text.Length > 5 && message.Text.Length < 50)
                    {
                        var key = message.Text.Substring(command.Length).ToUpperInvariant().Replace("-", "", StringComparison.Ordinal)
                                        .Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal).Trim();
                        var valid = ProductActivationSystem.VerifyKey(key);
                        if (valid)
                        {
                            if (!SharedDBcmd.KeyAlreadyRedeemed(message.From.Id, key))
                            {
                                //CHECK if the key is BLOCKED
                                if (SharedDBcmd.IsKeyBlocked(key))
                                    Logger.BotLogger.LogWarning($"Key is blocked => {message.From.Id} | {message.From.FirstName} | {message.From.Username ?? ""}", "KEY_COMMAND");
                                else
                                {
                                    SharedDBcmd.AddNewRegisteredUser(message.From.Id, key);
                                    userState.AuthLevel = AuthLevel.USER;
                                    await TGHost.Bot.SendTextMessageAsync(message.From.Id, KeyRedeemedSuccessfullyMessage, ParseMode.Html).ConfigureAwait(false);

                                    SharedDBcmd.GetAdminList().ForEach(async id => await TGHost.Bot.SendTextMessageAsync(id, $"!!! NEW USER: \"{message.From.Id} - @{message.From.Username} - {message.From.FirstName} {message.From.LastName}\"").ConfigureAwait(false));
                                }
                            }
                        }
                        else
                        {
                            Logger.BotLogger.LogWarning($"Key validation failed => {message.From.Id} | {message.From.FirstName} | {message.From.Username ?? ""}", "KEY_COMMAND");
                        }
                    }
                    break;
                default: //ALTRI
                    await TGHost.Bot.SendTextMessageAsync(message.From.Id, KeyAlreadyRedeemedMessage, ParseMode.Html).ConfigureAwait(false);
                    break;
            }
        }


    }
}
