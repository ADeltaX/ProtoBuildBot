using System.Collections.Generic;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ProtoBuildBot
{
    public static class TelegramBotSettings
    {
        public const string BotName = "ProtoBuildBot";
        public const int BotMessageVersion = 0;
        public const string BotVersion = "ProtoBuild v1.4 20M07 (ALPHA 14b)";
        public static string DatabaseName { get; } = "DataStore" + System.IO.Path.DirectorySeparatorChar + "ProtoBuildDB.sqlite";
        public static string SaveJSONPath { get; } = "DataStore" + System.IO.Path.DirectorySeparatorChar + "JSON";
        public static IReadOnlyList<string> DirectoriesToInit { get; } = new string[] 
        { 
            "DataStore", 
            "nothing", 
            "DataStore" + System.IO.Path.DirectorySeparatorChar + "JSON" 
        };

        public const string ProdHost = SecretKeys.ProdHost;
        public const string DevHost = SecretKeys.DevHost;
        public const string DevCertPass = SecretKeys.DevCertPass;
        public const string ProdCertPass = SecretKeys.ProdCertPass;

        public const string ApiKey = SecretKeys.ApiKey;
        public const string ApiUrlKey = SecretKeys.ApiUrlKey;

        public delegate void TelegramCallback(Update update);
        public static TelegramCallback TelegramUpdatesCallback { get; set; }

        public static IReadOnlyList<UpdateType> AllowedUpdates { get; } = new UpdateType[]
        {
            UpdateType.Message, 
            UpdateType.InlineQuery, 
            UpdateType.CallbackQuery, 
            UpdateType.ChosenInlineResult
        };
    }
}
