using ProtoBuildBot.Classes;
using ProtoBuildBot.Enums;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace ProtoBuildBot
{
    public interface IMessageHandler
    {
        bool IsGroupSupported { get; }
        AuthLevel MinimalAuthorizationLevel { get; }
        AuthLevel MinimalAuthorizationLevelForGroups { get; }
        bool IsAuthorizationTargeted { get; }
        bool IsGenericTextMessageSupported { get; }
        bool IsGenericMessageSupported { get; }
        string[] SupportedCommands { get; }
        

        void HandleCommandMessageFromGroup(GroupState groupState, Message message, string command);
        void HandleCallbackQueryFromGroup(GroupState groupState, CallbackQuery callbackQuery, string command, string param);
        void HandleCommandMessage(UserState userState, Message message, string command);
        void HandleCallbackQuery(UserState userState, CallbackQuery callbackQuery, string command, string param);

        //hehNice
        bool HandleGenericTextMessage(UserState userState, Message message);
        bool HandleMessage(UserState userState, Message message); //HANDLEEEEED
    }
}
