using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.Enums;
using Telegram.Bot.Types;
using static ProtoBuildBot.Classes.Localization.English;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class WhoamiCommand : UserMessageBase
    {
        public override string[] SupportedCommands => new[] { "/whoami" };

        public override bool IsAuthorizationTargeted => false;
        public override bool IsGroupSupported => true;
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.USER;
        public override void HandleCommandMessage(UserState userState, Message message, string command) 
            => MessageHelpers.SendMessageText(message.Chat.Id, $"@{message.From.Username} - {message.From.FirstName} {message.From.LastName} | {userState.AuthLevel}");
    }
}
