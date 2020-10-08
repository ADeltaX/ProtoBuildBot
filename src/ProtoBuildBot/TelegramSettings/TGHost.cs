using System.Net;
using Telegram.Bot;
using System;
using System.IO;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Microsoft.AspNetCore.Hosting;

namespace ProtoBuildBot
{
    public class TGHost
    {
        private static IWebHost _host;
        public static TelegramBotClient Bot { get; set; }

        public TGHost()
        {

#if PRODUCTION
            Logger.BotLogger.LogInfo($"{TelegramBotSettings.BotName} running in PRODUCTION MODE!", "INIT");
#else
            Logger.BotLogger.LogInfo($"{TelegramBotSettings.BotName} running in development mode!", "INIT");
#endif
            Bot = new TelegramBotClient(TelegramBotSettings.ApiKey);
        }

        public async Task StartPolingHost()
        {
            //Just to be absolutely sure
            try
            {
                await Bot.DeleteWebhookAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //NO, WE CAN'T CONTINUE
                Logger.BotLogger.LogFatal(ex.Message, "TG_BOT");
                throw;
            }

            int offset = 0;
            while (true)
            {
                var updates = Array.Empty<Update>();

                try
                {
                    updates = await Bot.GetUpdatesAsync(offset, allowedUpdates: TelegramBotSettings.AllowedUpdates).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Logger.BotLogger.LogWarning($"{ex.Message}", "TG_BOT");
                }

                foreach (var update in updates)
                {
                    TelegramBotSettings.TelegramUpdatesCallback(update);
                    offset = update.Id + 1;
                }
            }
        }

        

        public static void CreateAndStartWebHost()
        {
#if PRODUCTION
            int port = 443;
#else
            int port = 8443;
#endif

            _host = new WebHostBuilder()
                    .UseKestrel(options => {
                        options.Listen(IPAddress.Any, port, listenOptions =>
                        {
#if PRODUCTION
                            listenOptions.UseHttps(Path.Combine(Directory.GetCurrentDirectory(), "CERT.pfx"), TelegramBotSettings.ProdCertPass);
#else
                            listenOptions.UseHttps(Path.Combine(Directory.GetCurrentDirectory(), "CERT.pfx"), TelegramBotSettings.DevCertPass);
#endif
                        });

                        options.AddServerHeader = false;
                    })
                    .UseContentRoot(Directory.GetCurrentDirectory() + "/nothing")
                    .UseStartup<StartupAsp>()
                    .Build();

#pragma warning disable CS4014
            _host.RunAsync();
#pragma warning restore CS4014
            Logger.BotLogger.LogInfo($"Server web started!", "INIT");
        }

        public async void StartTelegramWebhookHost()
        {
#if PRODUCTION
            await TGHost.Bot.SetWebhookAsync($"https://{SecretKeys.ProdHost}/api/internal/telegram/{TelegramBotSettings.ApiUrlKey}/data", null, 0, TelegramBotSettings.AllowedUpdates).ConfigureAwait(false);
#else
            await TGHost.Bot.SetWebhookAsync($"https://{SecretKeys.DevHost}/api/internal/telegram/{TelegramBotSettings.ApiUrlKey}/data", null, 0, TelegramBotSettings.AllowedUpdates).ConfigureAwait(false);
#endif
            Logger.BotLogger.LogInfo($"Webhooking started!", "INIT");
        }
    }
}
