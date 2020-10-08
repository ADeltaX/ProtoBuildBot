using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static ProtoBuildBot.Classes.Localization.English;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class VersionCommand : UnregMessageBase
    {
        public override string[] SupportedCommands => new[] { "/version", "/botversion" };

        public override bool IsAuthorizationTargeted => false;
        public override bool IsGroupSupported => base.IsGroupSupported;
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.UNREGISTERED;

        public override async void HandleCommandMessage(UserState userState, Message message, string command)
        {
            try
            {
                if (message.Chat.Type == ChatType.Supergroup || message.Chat.Type == ChatType.Group)
                    await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, BotVersionMessage).ConfigureAwait(false);
                else if (userState.AuthLevel >= Enums.AuthLevel.USER)
                    await TGHost.Bot.SendTextMessageAsync(message.From.Id, BotVersionMessage).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "VERSION_COMMAND");
            }
        }
    }
}
