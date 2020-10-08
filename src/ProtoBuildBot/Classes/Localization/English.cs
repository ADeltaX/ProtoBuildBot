using BuildChecker;
using ProtoBuildBot.Enums;
using System.Text;

namespace ProtoBuildBot.Classes.Localization
{

    //TODO: REMOVE THIS CLASS
    public static class English
    {
        public const string UserHelpPageMessage = "Hi <b>{0}</b>!\n\nThese are the available commands:\n🔔 /subscribe - <i>Subscribe to being notified when a new build appears on a ring</i>\n❌ /unsubscribe - <i>Stop being notified</i>\n❓ /version - <i>Bot version</i>";
        public const string ModHelpPageMessage = "Hi <b>{0}</b>!\n\nThese are the available commands:\n🔔 /subscribe - <i>Subscribe to being notified when a new build appears on a ring</i>\n❌ /unsubscribe - <i>Stop being notified</i>\n❓ /version - <i>Bot version</i>\n🗝️ /genkey [times] - <i>With this command you can generate keys, \"times\" is a optional value (max 3)</i>";
        public const string AdminHelpPageMessage = "Hi <b>{0}</b>, my boss!\n\nYour master commands:\n🔔 /subscribe - <i>Subscribe to being notified when a new build appears on a ring</i>\n❌ /unsubscribe - <i>Stop being notified</i>\n❓ /version - <i>Bot version</i>\n🗝️ /genkey [times] - <i>With this command you can generate keys, \"times\" is a optional value (max 3)</i>\n🆙 /upgradeuser - <i>Upgrade user</i>";

        public const string KeyAlreadyRedeemedMessage = "🔓 You don't need it, you are already with us :)\nDigit /help to display all available commands.";
        public const string KeyRedeemedSuccessfullyMessage = "🔓✅ Key redeemed successfully! You can now access the service! :) Digit /help to display all available commands.";

        public const string CommandNotFoundMessage = "🤐 Command not found. Digit /help to display all available commands.";

        public const string BotVersionMessage = "Bot version:\n" + TelegramBotSettings.BotVersion;
    }
}
