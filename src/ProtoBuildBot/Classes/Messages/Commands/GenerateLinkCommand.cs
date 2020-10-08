using ProtoBuildBot.Classes.Messages.Base;
using ProtoBuildBot.DataStore;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ProtoBuildBot.Classes.Messages.Commands
{
    public sealed class GenerateLinkCommand : UnregMessageBase
    {
        //TODO (2.0): make an interactive version

        public override string[] SupportedCommands => new[] { "/generatelink", "/ohlink" };

        public override bool IsAuthorizationTargeted => false;

        public override void HandleCommandMessage(UserState userState, Message message, string command)
        {
            try
            {
                if ((message.Chat.Type == ChatType.Supergroup || message.Chat.Type == ChatType.Group) && SharedDBcmd.IsGroupSubscribed(message.Chat.Id))
                {
                    GenLinks(userState, message, command);
                }
                else if (userState.AuthLevel >= Enums.AuthLevel.USER)
                {
                    GenLinks(userState, message, command);
                }
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "GENERATELINK_COMMAND");
            }
        }

        private async void GenLinks(UserState userState, Message message, string command)
        {
            var msg = message.Text.ToLowerInvariant().Substring(command.Length).Trim();

            string[] cmds = Array.Empty<string>();

            if (msg.StartsWith("@", StringComparison.InvariantCultureIgnoreCase))
                cmds = msg.ToUpperInvariant().Split(" ").Skip(1).ToArray();
            else
                cmds = msg.ToUpperInvariant().Split(" ");

            if (cmds.Length != 3)
            {
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "[DeviceFamily] [ArCh] [BuIlD]", ParseMode.Html).ConfigureAwait(false);
                return;
            }

            var updateID = SharedDBcmd.GetUpdateID(cmds[0], cmds[1], cmds[2]);
            if (updateID != null)
            {
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "UpdateID: " + updateID, ParseMode.Html).ConfigureAwait(false);
                using var mem = DecompressAndLoadFile(updateID);
                if (mem != null)
                {
                    var result = await UploadFile(mem, updateID, message.Chat.Id).ConfigureAwait(false);
                    if (!result)
                        await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "RIP", ParseMode.Html).ConfigureAwait(false);
                }
                else
                {
                    await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "File not available, can't generate download links :(", ParseMode.Html).ConfigureAwait(false);
                }
            }
            else
                await TGHost.Bot.SendTextMessageAsync(message.Chat.Id, "Not found.", ParseMode.Html).ConfigureAwait(false);
        }

        static async Task<bool> UploadFile(MemoryStream file, string updateID, long id)
        {
            if (file == null || !file.CanRead)
                return false;

            await TGHost.Bot.SendDocumentAsync(id, new Telegram.Bot.Types.InputFiles.InputOnlineFile(file, updateID + ".json")).ConfigureAwait(false);

            file.Dispose();
            GC.Collect();
            return true;
        }

        static MemoryStream DecompressAndLoadFile(string updateID)
        {
            string fileIn = TelegramBotSettings.SaveJSONPath + Path.DirectorySeparatorChar + updateID + ".json.gz";

            if (!System.IO.File.Exists(fileIn))
                return null;

            using var input = new GZipStream(System.IO.File.OpenRead(fileIn), CompressionMode.Decompress);
            var output = new MemoryStream();
            Write(input, output);
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        static void Write(Stream input, Stream output, int bufferSize = 10 * 1024 * 1024)
        {
            var buffer = new byte[bufferSize];
            for (int readCount; (readCount = input.Read(buffer, 0, buffer.Length)) > 0;)
                output.Write(buffer, 0, readCount);
        }
    }
}
