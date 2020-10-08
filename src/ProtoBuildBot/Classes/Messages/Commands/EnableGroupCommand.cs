using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    //GROUP command
    public sealed class EnableGroupCommand : AdminMessageBase
    {
        
        public override string[] SupportedCommands => new[] { "/registergroup", "/unregistergroup" };
        public override bool IsGroupSupported => true;
        public override AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.MOD;
        public override void HandleCommandMessageFromGroup(GroupState groupState, Message message, string command)
        {
            SharedDBcmd.AddToHistoryCommands(message.From.Id, message.Text);

            try
            {
                if (command == "/registergroup")
                {
                    if (!groupState.IsRegistered)
                    {
                        SharedDBcmd.UpdateGroupRegistrationStatus(groupState.ChatId, true);
                        MessageHelpers.SendMessageText(groupState.ChatId, "✅ Group successfully <b>registered</b>!");
                    }
                    else
                    {
                        MessageHelpers.SendMessageText(groupState.ChatId, "⚠️ Group is already registered!");
                    }
                }
                else if (command == "/unregistergroup")
                {
                    if (groupState.IsRegistered)
                    {
                        SharedDBcmd.UpdateGroupRegistrationStatus(groupState.ChatId, false);
                        MessageHelpers.SendMessageText(groupState.ChatId, "✅❕ Group successfully <b>unregistered</b>!");
                    }
                    else
                    {
                        MessageHelpers.SendMessageText(groupState.ChatId, "⚠️ Group is not registered!");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "ENABLEGROUP_COMMAND");
            }
        }
    }
}
