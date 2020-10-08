using ProtoBuildBot.Enums;
using ProtoBuildBot;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes.Messages.Base
{
    public abstract class MessageBase : IMessageHandler
    {
        public abstract AuthLevel MinimalAuthorizationLevel { get; }

        public virtual bool IsAuthorizationTargeted => false;

        public virtual bool IsGenericMessageSupported => false;

        public virtual bool IsGenericTextMessageSupported => false;

        public virtual string[] SupportedCommands { get; }

        public virtual bool IsGroupSupported => false;

        public virtual AuthLevel MinimalAuthorizationLevelForGroups => AuthLevel.CREATOR;

        public virtual bool HandleMessage(UserState userState, Message message) => true;
        public virtual bool HandleGenericTextMessage(UserState userState, Message message) => false;

        public virtual void HandleCommandMessage(UserState userState, Message message, string command) { }
        public virtual void HandleCallbackQuery(UserState userState, CallbackQuery callbackQuery, string command, string param) { }

        public virtual void HandleCommandMessageFromGroup(GroupState groupState, Message message, string command) { }
        public virtual void HandleCallbackQueryFromGroup(GroupState groupState, CallbackQuery callbackQuery, string command, string param) { }
    }
}
