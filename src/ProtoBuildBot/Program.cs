using Microsoft.Extensions.DependencyModel;
using ProtoBuildBot.Classes;
using ProtoBuildBot.Classes.Automation;
using ProtoBuildBot.DataStore;
using ProtoBuildBot.Enums;
using Schedule;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ProtoBuildBot
{
    public static class Program
    {
        //cache?
        private static Dictionary<string, IMessageHandler> CommandHandlers { get; set; }
        private static List<IMessageHandler> GenericTextHandlers { get; set; }
        private static List<IMessageHandler> GenericHandlers { get; set; }
        public static DateTime SystemUptime { get; set; }

        static ScheduleTimer _scheduleTimer, _highSpeedScheduleTimer;
        static ScanType _currentScanType = ScanType.Normal;
        static DBEngine _dbEngine;

        static void Main()
        {
            var lel = CultureInfo.CreateSpecificCulture("scn");

            Console.WriteLine($"Freshy {TelegramBotSettings.BotName}!");
            Console.WriteLine("Release version: " + TelegramBotSettings.BotVersion);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n!!! Initializing !!!");
            Console.ResetColor();

            SystemUptime = DateTime.UtcNow;

            Console.WriteLine("\nInitializing Directories...");
            InitializeDirectory();

            Logger.BotLogger.LogInfo("Preloading cultures...", "INIT");
            PreloadCultures();

            Console.WriteLine("\nInitializing Database...");
            _dbEngine = new DBEngine();

            Logger.BotLogger.LogInfo("Initializing Commands...", "INIT");
            InitializeCommands();

            Logger.BotLogger.LogInfo("Initializing scan scheduler...", "INIT");
            InitializeScheduleJobs();
            ConfigureAndStartJobs();

            Logger.BotLogger.LogInfo("Initializing Telegram Bot engine...", "INIT");
            TelegramBotSettings.TelegramUpdatesCallback = new TelegramBotSettings.TelegramCallback(TelegramUpdates);

            TGHost tb = new TGHost();

            Console.ForegroundColor = ConsoleColor.Green;
            Logger.BotLogger.LogInfo("Initialization completed!\n", "INIT");
            Console.ResetColor();

            TGHost.CreateAndStartWebHost();
            _ = tb.StartPolingHost().ConfigureAwait(false);

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                //TODO when needed handle this
            };

            Console.ReadKey();

            _dbEngine.Dispose();
        }

        #region Initialization

        private static void PreloadCultures()
        {
            CultureInfo.CreateSpecificCulture("en-US");
            CultureInfo.CreateSpecificCulture("it-IT");
        }

        private static void InitializeCommands()
        {
            CommandHandlers = new Dictionary<string, IMessageHandler>(StringComparer.InvariantCultureIgnoreCase);
            GenericHandlers = new List<IMessageHandler>();
            GenericTextHandlers = new List<IMessageHandler>();

            var instances = CreateInstances(GetAllMessageHandlers());

            foreach (var item in instances)
            {
                if (item.SupportedCommands != null && item.SupportedCommands.Length > 0)
                {
                    foreach (var cmd in item.SupportedCommands)
                    {
                        Logger.BotLogger.LogInfo($"Added command: {cmd,-20} --> {item.GetType().Name}", "INIT");
                        CommandHandlers.Add(cmd, item);
                    }
                }

                if (item.IsGenericTextMessageSupported)
                    GenericTextHandlers.Add(item);

                if (item.IsGenericMessageSupported)
                    GenericHandlers.Add(item);
            }
        }

        private static void InitializeDirectory()
        {
            foreach (var dir in TelegramBotSettings.DirectoriesToInit)
            {
                try
                {
                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[INITIALIZATION ERROR] {ex.Message}");
                }
            }
        }

        static readonly TimeSpan[] dailyActiveSearchTimeSpanPT = new TimeSpan[]
        {
            new TimeSpan(00, 00, 02), new TimeSpan(00, 20, 02), new TimeSpan(00, 40, 02),
            new TimeSpan(01, 00, 02), new TimeSpan(01, 20, 02), new TimeSpan(01, 40, 02),
            new TimeSpan(02, 00, 02), new TimeSpan(02, 20, 02), new TimeSpan(02, 40, 02),
            new TimeSpan(03, 00, 02), new TimeSpan(03, 20, 02), new TimeSpan(03, 40, 02),
            new TimeSpan(04, 00, 02), new TimeSpan(04, 20, 02), new TimeSpan(04, 40, 02),
            new TimeSpan(05, 00, 02), new TimeSpan(05, 15, 02), new TimeSpan(05, 45, 02),
            new TimeSpan(06, 00, 02), new TimeSpan(06, 20, 02), new TimeSpan(06, 40, 02),
            new TimeSpan(07, 00, 02), new TimeSpan(07, 20, 02), new TimeSpan(07, 40, 02),
            new TimeSpan(08, 00, 02), new TimeSpan(08, 20, 02), new TimeSpan(08, 40, 02),
            new TimeSpan(09, 15, 02), new TimeSpan(09, 30, 02), new TimeSpan(09, 45, 02),
            new TimeSpan(10, 00, 02), new TimeSpan(10, 05, 02), new TimeSpan(10, 30, 02),  //------- Possible release time
            new TimeSpan(11, 00, 02), new TimeSpan(11, 05, 02), new TimeSpan(11, 30, 02),  //------- Possible release time
            new TimeSpan(12, 00, 02), new TimeSpan(12, 20, 02), new TimeSpan(12, 40, 02),  
            new TimeSpan(13, 00, 02), new TimeSpan(13, 05, 02), new TimeSpan(13, 30, 02),  //------- Possible release time
            new TimeSpan(14, 00, 02), new TimeSpan(14, 20, 02), new TimeSpan(14, 40, 02),  
            new TimeSpan(15, 00, 02), new TimeSpan(15, 05, 02), new TimeSpan(15, 30, 02),  //------- Possible release time
            new TimeSpan(16, 00, 02), new TimeSpan(16, 05, 02), new TimeSpan(16, 30, 02),  //------- Possible release time
            new TimeSpan(17, 00, 02), new TimeSpan(17, 20, 02), new TimeSpan(17, 40, 02),
            new TimeSpan(18, 00, 02), new TimeSpan(18, 05, 02), new TimeSpan(18, 30, 02),  //------- Possible release time
            new TimeSpan(19, 15, 02), new TimeSpan(19, 30, 02), new TimeSpan(19, 45, 02),
            new TimeSpan(20, 00, 02), new TimeSpan(20, 20, 02), new TimeSpan(20, 40, 02),  //04:00 GMT+1
            new TimeSpan(21, 00, 02), new TimeSpan(21, 20, 02), new TimeSpan(21, 40, 02),
            new TimeSpan(22, 00, 02), new TimeSpan(22, 20, 02), new TimeSpan(22, 40, 02),
            new TimeSpan(23, 00, 02), new TimeSpan(23, 20, 02), new TimeSpan(23, 40, 02),

            //For lost updates
            new TimeSpan(20, 00, 02), new TimeSpan(20, 05, 02), //04:00 GMT+1
        };

        static readonly TimeSpan[] dailySilentSearchesTimeSpanPT = new TimeSpan[] 
        {
            new TimeSpan(05, 30, 02),
            new TimeSpan(09, 00, 02),
            new TimeSpan(19, 00, 02)
        };

        public static void ToggleScanType(ScanType type)
        {
            if (_currentScanType == type)
                return;

            _currentScanType = type;

            Logger.BotLogger.LogWarning("Scan type switched to: " + type.ToString(), "SCHEDULER");

            switch (type)
            {
                case ScanType.Normal:
                    _highSpeedScheduleTimer.ClearJobs();
                    _highSpeedScheduleTimer.Stop();
                    ConfigureAndStartJobs();
                    break;
                case ScanType.UltraFast:
                    _scheduleTimer.ClearJobs();
                    _scheduleTimer.Stop();
                    ConfigureFastAndStartJobs();
                    break;
            }
        }

        private static void InitializeScheduleJobs()
        {
            _scheduleTimer = new ScheduleTimer() { EventStorage = new LocalEventStorage() };
            _scheduleTimer.Elapsed += ScheduleTimer_Elapsed;

            _highSpeedScheduleTimer = new ScheduleTimer() { EventStorage = new LocalEventStorage() };
            _highSpeedScheduleTimer.Elapsed += HighSpeedScheduleTimer_Elapsed;
        }

        private static void ConfigureAndStartJobs()
        {
            foreach (var searchTime in dailyActiveSearchTimeSpanPT)
            {
                _scheduleTimer.AddEvent(new ScheduledTime(EventTimeBase.Daily, ScheduledTime.GetLocalTimeSpanFromPT(searchTime)));
                Logger.BotLogger.LogInfo("Adding scheduled ACTIVE scan for time: " + ScheduledTime.GetLocalTimeSpanFromPT(searchTime), "SCHEDULER");
            }

            foreach (var searchTime in dailySilentSearchesTimeSpanPT)
            {
                _scheduleTimer.AddEvent(new ScheduledTime(EventTimeBase.Daily, ScheduledTime.GetLocalTimeSpanFromPT(searchTime)));
                Logger.BotLogger.LogInfo("Adding scheduled SILENT scan for time: " + ScheduledTime.GetLocalTimeSpanFromPT(searchTime), "SCHEDULER");
            }

            _scheduleTimer.Start();
        }

        private static void ConfigureFastAndStartJobs()
        {
            _highSpeedScheduleTimer.AddEvent(new SimpleInterval(DateTime.Now, TimeSpan.FromMinutes(5)));
            Logger.BotLogger.LogInfo("Adding scheduled ACTIVE UltraFast scan, repeated every: " + TimeSpan.FromMinutes(5).Minutes + " minutes", "SCHEDULER");
            _highSpeedScheduleTimer.Start();
        }

        private static void ScheduleTimer_Elapsed(object sender, ScheduledEventArgs e)
        {
            var localTime = ScheduledTime.GetUtcTimeSpanFromLocal(e.EventTime.TimeOfDay);

            if (dailySilentSearchesTimeSpanPT.Contains(localTime))
            {
                //RUUN DEEEEEEPSEARCH
                Task.Run(() => SearchAutomation.RunSilentSearch(SearchAutomation.GetSilentSearchItems())).ConfigureAwait(false);
            }
            else
            {
                //Everything else is just normal
                Task.Run(() => SearchAutomation.RunSearch(SearchAutomation.GetActiveSearchItems())).ConfigureAwait(false);
            }
        }

        private static void HighSpeedScheduleTimer_Elapsed(object sender, ScheduledEventArgs e) 
            => Task.Run(() => SearchAutomation.RunSearch(SearchAutomation.GetActiveSearchItems())).ConfigureAwait(false);

        #endregion

        public static void TelegramUpdates(Update update)
        {

            switch (update?.Type)
            {
                case UpdateType.Message:
                    HandleMessage(update.Message);
                    break;
                case UpdateType.CallbackQuery:
                    _ = Task.Run(() => HandleCallbackQuery(update.CallbackQuery)).ConfigureAwait(false);
                    break;
            }

        }

        private static async void HandleCallbackQuery(CallbackQuery query)
        {
            //ToDo: Implement last command HERE (2.0)
            var splitted = query.Data?.Split('-', 4);
            if (splitted != null && splitted.Length > 3)
            {
                var version = splitted[0];
                var dateTime = splitted[1];
                var localId = splitted[2];
                var cmdWithParam = splitted[3].Split('+', 2);
                var command = cmdWithParam[0];
                var param = cmdWithParam.Length > 1 ? cmdWithParam[1] : "";

                bool isExpired = false;

                //Check if it's expired
                if (version != TelegramBotSettings.BotMessageVersion.ToString(CultureInfo.InvariantCulture) ||
                    DateTimeOffset.FromUnixTimeSeconds(long.Parse(dateTime, CultureInfo.InvariantCulture)).AddHours(48) < DateTime.UtcNow)
                {
                    isExpired = true;
                }


                if (localId == "GROUP")
                {
                    if (MessageHelpers.IsMessageFromGroup(query.Message))
                    {
                        //Nothing
                        if (CommandHandlers.TryGetValue(command, out var msgHandler))
                        {
                            var us = SharedDBcmd.GetUserState(query.From.Id);
                            if (us != null)
                            {
                                if (msgHandler.IsGroupSupported && us.AuthLevel >= msgHandler.MinimalAuthorizationLevelForGroups)
                                {
                                    var gs = SharedDBcmd.GetGroupState(query.Message.Chat.Id);
                                    if (gs != null)
                                    {
                                        if (isExpired)
                                        {
                                            MessageHelpers.EditOrSendMessageText(query.Message.Chat.Id, query.Message.MessageId,
                                                MessageHelpers.GetLocalizedText("P_Expired", CultureInfo.InvariantCulture),
                                                MessageHelpers.GetDefaultStartButton(gs));
                                        }
                                        else
                                        {
                                            msgHandler.HandleCallbackQueryFromGroup(gs, query, command, param);
                                        }                                        
                                    }
                                }
                            }
                        }

                        try
                        {
                            await TGHost.Bot.AnswerCallbackQueryAsync(query.Id).ConfigureAwait(false);
                        }
                        catch (Exception)
                        { } //WHO cares
                    }
                }
                else
                {
                    long id = long.Parse(localId, CultureInfo.InvariantCulture);
                    if (id == query.From.Id)
                    {
                        //Ok, it's the user
                        if (CommandHandlers.TryGetValue(command, out var msgHandler))
                        {
                            var us = SharedDBcmd.GetUserState(query.From.Id);
                            if (us != null)
                            {
                                if ((!msgHandler.IsAuthorizationTargeted && us.AuthLevel >= msgHandler.MinimalAuthorizationLevel)
                                    || (msgHandler.IsAuthorizationTargeted && us.AuthLevel == msgHandler.MinimalAuthorizationLevel))
                                {
                                    if (isExpired)
                                    {
                                        MessageHelpers.EditOrSendMessageText(query.Message.Chat.Id, query.Message.MessageId,
                                            MessageHelpers.GetLocalizedText("P_Expired", CultureInfo.InvariantCulture),
                                            MessageHelpers.GetDefaultStartButton(us));
                                    }
                                    else
                                    {
                                        msgHandler.HandleCallbackQuery(us, query, command, param);
                                    }
                                }
                            }
                        }
                        try
                        {
                            await TGHost.Bot.AnswerCallbackQueryAsync(query.Id).ConfigureAwait(false);
                        }
                        catch (Exception)
                        { } //WHO cares
                    }
                    else
                    {
                        try
                        {
                            await TGHost.Bot.AnswerCallbackQueryAsync(query.Id, "You dare not", false).ConfigureAwait(false);
                        }
                        catch (Exception)
                        { }  //WHO cares
                    }
                }
            }
        }

        private static void HandleMessage(Message message)
        {
            var us = SharedDBcmd.GetUserState(message.From.Id);
            if (us == null)
                return; //internal error.

            switch (message.Type)
            {
                case MessageType.Text:
                    if (message.Text == "/cancel")
                    {
                        if (us.CancellationTokenSource != null)
                        {
                            try
                            {
                                us.CancellationTokenSource.Cancel();
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                            us.ResetUserState();
                        }
                    }
                    else
                    {
                        if (us.Queue.IsEmpty)
                        {
                            us.Queue.Enqueue(message);
                            us.CancellationTokenSource = new CancellationTokenSource();
                            _ = Task.Run(() => HandleText(message, us, true), us.CancellationTokenSource.Token).ConfigureAwait(false);
                        }
                        else
                            us.Queue.Enqueue(message);
                    }
                    break;
                case MessageType.Document:
                    HandlePossibleUpdatePackage(message, us);
                    break;
            }
        }

        private static async void HandlePossibleUpdatePackage(Message message, UserState us)
        {
            //TODO 2.0
            if (us.AuthLevel == AuthLevel.CREATOR && !string.IsNullOrEmpty(message.Document.FileName) && message.Document.FileName.Equals("update.pbb", StringComparison.OrdinalIgnoreCase))
                MessageHelpers.SendMessageText(message.From.Id, "A nice update package. Surely going to update it /s");
        }

        private static async void HandleText(Message message, UserState us, bool firstCall = false)
        {
            try
            {
                if (message.Text != null)
                {
                    if (message.Text.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var cmdPart = message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                        if (cmdPart[0].Contains("@", StringComparison.InvariantCultureIgnoreCase))
                            cmdPart[0] = cmdPart[0].Split("@")[0];

                        bool msgHandlerFound = CommandHandlers.TryGetValue(cmdPart[0], out var msgHandler);
                        if (msgHandlerFound)
                        {
                            //Check if it's a group or private chat
                            if (MessageHelpers.IsMessageFromGroup(message))
                            {
                                if (msgHandler.IsGroupSupported && us.AuthLevel >= msgHandler.MinimalAuthorizationLevelForGroups)
                                {
                                    try
                                    {
                                        var gs = SharedDBcmd.GetGroupState(message.Chat.Id);
                                        if (gs != null)
                                        {
                                            msgHandler.HandleCommandMessageFromGroup(gs, message, cmdPart[0]);
                                            us.LastCommand = cmdPart[0];
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        //hmmm....
                                        Logger.BotLogger.LogError(ex.Message, "MSGHandler");
                                    }
                                }
                            }
                            else
                            {
                                //Check if msgHandler has auth target
                                if ((!msgHandler.IsAuthorizationTargeted && us.AuthLevel >= msgHandler.MinimalAuthorizationLevel)
                                    || (msgHandler.IsAuthorizationTargeted && us.AuthLevel == msgHandler.MinimalAuthorizationLevel))
                                {
                                    //SEEEEEND
                                    try
                                    {
                                        msgHandler.HandleCommandMessage(us, message, cmdPart[0]);
                                        us.LastCommand = cmdPart[0];
                                    }
                                    catch (Exception ex)
                                    {
                                        //hmmm....
                                        Logger.BotLogger.LogError(ex.Message, "MSGHandler");
                                    }
                                }
                                else
                                {
                                    //Unauthorized OR unregistered
                                    //do nothing
                                }
                            }
                        }
                        else
                        {
                            //Command not found
                            if (us.AuthLevel >= AuthLevel.USER && (message.Chat.Type == ChatType.Private))
                                MessageHelpers.SendMessageText(message.Chat.Id, MessageHelpers.GetLocalizedText("P_CmdNotFound", us.CultureInfo), 
                                                                    MessageHelpers.GetDefaultStartButton(us));
                        }
                    }
                    else
                    {
                        //TODO: Generic message
                    }
                }
                else
                {
                    //Invalid message
                }

                //AREA QUEUE & TASKS HANDLE

                Message msg;
                if (firstCall)
                {
                    bool isOk;
                    do
                    {
                        if (!us.Queue.IsEmpty)
                            isOk = us.Queue.TryDequeue(out msg);
                        else
                        {
                            if (us.CancellationTokenSource == null) return;

                            us.CancellationTokenSource.Dispose();
                            us.CancellationTokenSource = null;
                            return;
                        }
                    } while (!isOk);
                }
                if (!us.Queue.IsEmpty)
                {
                    bool isOk;
                    do
                    {
                        if (!us.Queue.IsEmpty)
                            isOk = us.Queue.TryDequeue(out msg);
                        else
                        {
                            if (us.CancellationTokenSource == null) return;

                            us.CancellationTokenSource.Dispose();
                            us.CancellationTokenSource = null;
                            return;
                        }
                    } while (!isOk);

                    HandleText(msg, us);
                }
                if (us.CancellationTokenSource == null) return;
                us.CancellationTokenSource.Dispose();
                us.CancellationTokenSource = null;
            }
            catch (OperationCanceledException)
            {
                try
                {
                    MessageHelpers.SendMessageText(message.Chat.Id, MessageHelpers.GetLocalizedText("P_CmdAborted", us.CultureInfo), 
                                                    MessageHelpers.GetDefaultStartButton(us));
                    us.Queue = new ConcurrentQueue<Message>();
                    if (us.CancellationTokenSource != null)
                    {
                        us.CancellationTokenSource.Dispose();
                        us.CancellationTokenSource = null;
                    }
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    Logger.BotLogger.LogError(ex.Message, "MSGHandler");
                    us.CancellationTokenSource = null;
                }
            }
        }

        #region Find classes that implements IMessageHandler

        private static IList<IMessageHandler> CreateInstances(IList<Type> types)
        {
            var instances = new List<IMessageHandler>(types.Count);
            foreach (var item in types)
                instances.Add((IMessageHandler)Activator.CreateInstance(item));

            return instances;
        }

        private static IList<Type> GetAllMessageHandlers()
        {
            var platform = Environment.OSVersion.Platform.ToString();
            var runtimeAssemblyNames = DependencyContext.Default.GetRuntimeAssemblyNames(platform);

            IList<Type> types = runtimeAssemblyNames.AsParallel()
                .Select(Assembly.Load)
                .SelectMany(a => a.ExportedTypes)
                .Where(t => typeof(IMessageHandler).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface).ToList();

            return types;
        }

        #endregion
    }
}
