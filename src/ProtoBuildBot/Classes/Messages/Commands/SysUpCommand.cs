using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System;
using System.Globalization;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static ProtoBuildBot.Classes.Localization.English;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class SysUpCommand : UserMessageBase
    {
        public override string[] SupportedCommands => new[] { "/sysup", "/sysuptime" };

        public override bool IsAuthorizationTargeted => false;

        public override bool IsGroupSupported => true;
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.UNREGISTERED;

        public override void HandleCommandMessage(UserState userState, Message message, string command) 
            => MessageHelpers.SendMessageText(message.Chat.Id, "Uptime: " + (DateTime.UtcNow - Program.SystemUptime).ToString(@"dd\.hh\:mm\:ss", CultureInfo.InvariantCulture));
    }
}