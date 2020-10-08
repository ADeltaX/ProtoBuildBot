using BuildChecker;
using ProtoBuildBot.DataStore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;

namespace ProtoBuildBot.Classes.Automation
{
    public static class SearchAutomation
    {
        public static DateTime LastExecutedAt { get; private set; } = DateTime.MinValue;
        public static bool IsRunning { get; private set; } = false;

        private const int MAX_RETRIES = 3;
        private static readonly Queue<SearchItem> _silentQueue = new Queue<SearchItem>();
        public static readonly UUP thisUUP = new UUP();

        #region Search filters

        public static string[] GetSearchableFamilies() 
            => SearchHelpers.GetSearchItems.Where(item => !item.SilentSearch).GroupBy(itemSelect => itemSelect.DeviceFamily).Select(g => g.First().DeviceFamily).ToArray();

        public static string[] GetSearchableRings(SearchItem[] searchItems, string deviceFamily)
        {
            if (deviceFamily == "@EV")
                return new[] { "@AR" };

            var filtered = searchItems.Where(item => !item.SilentSearch && item.DeviceFamily == deviceFamily).Select(
                itemSelect =>
                {
                    var tmp = itemSelect.Ring;
                    if (itemSelect.ShouldMergeString)
                        tmp += $" ({itemSelect.BranchCodename})";

                    return tmp;
                }).ToArray();

            return filtered;
        }

        public static SearchItem[] GetFilteredSearchItems(SearchItem[] searchItems, string deviceFamily, string ring)
        {
            if (deviceFamily.Equals("@EV", StringComparison.Ordinal))
                return searchItems.Where(item => !item.SilentSearch).ToArray();

            string branchCodename = "";
            string curRing = ring ?? "";

            if (curRing.Equals("@EVR", StringComparison.Ordinal))
                curRing = "";
            else if (curRing.Contains("(", StringComparison.Ordinal) && curRing.Contains(")", StringComparison.Ordinal))
            {
                branchCodename = curRing.Split('(', 2)[1].Split(')', 2)[0];
                curRing = curRing.Split(' ')[0];
            }

            //Filter per device family, check if the ring is empty (means do whatever you want) OR compare it, same for build.
            var filtered = searchItems.Where(item => !item.SilentSearch &&
            item.DeviceFamily == deviceFamily && 
            (string.IsNullOrEmpty(curRing) || item.Ring == curRing) && 
            (string.IsNullOrEmpty(branchCodename) || item.BranchCodename == branchCodename)).ToArray();

            return filtered;
        }

        public static SearchItem[] GetFilteredSilentSearchItems(SearchItem[] searchItems, string family, string ring, string branchCodename) 
            => searchItems.Where(item => item.SilentSearch && item.DeviceFamily == family && item.Ring == ring 
                                         && (string.IsNullOrEmpty(branchCodename) || item.BranchCodename == branchCodename)).ToArray();

        public static SearchItem[] GetSilentSearchItems()
            => SearchHelpers.GetSearchItems.Where(item => item.SilentSearch).ToArray();

        public static SearchItem[] GetActiveSearchItems()
            => SearchHelpers.GetSearchItems.Where(item => !item.SilentSearch).ToArray();

        #endregion

        //NEW
        public static void RunSearch(SearchItem[] searchItems)
        {
            if (searchItems == null)
                throw new ArgumentNullException(nameof(searchItems));

            if (IsRunning)
                return;

            IsRunning = true;
            LastExecutedAt = DateTime.UtcNow;

            Logger.BotLogger.LogInfo(MessageHelpers.GetLocalizedText("Z_SearchStarted", CultureInfo.InvariantCulture), "SEARCH_AUTOMATION");

            try
            {
                Dictionary<SearchItem, string> ignoreUpdateIDs = GetIgnorableUpdateIDs(searchItems);
                foreach (var item in searchItems)
                {
                    ignoreUpdateIDs.TryGetValue(item, out string updateID);
                    InternalRunSearch(item.DeviceFamily, item.Ring, item.Arch, item.BranchCodename, item.SilentSearch, item.ShouldMergeString, updateID);
                }
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "SEARCH_AUTOMATION");
            }

            while (_silentQueue.Count > 0)
            {
                var item = _silentQueue.Dequeue();
                InternalRunSearch(item.DeviceFamily, item.Ring, item.Arch, item.BranchCodename, item.SilentSearch, item.ShouldMergeString, null);
            }

            Logger.BotLogger.LogInfo("Search finished", "SEARCH_AUTOMATION");

            IsRunning = false;
            LastExecutedAt = DateTime.UtcNow;
        }

        //alias deep search
        public static void RunSilentSearch(SearchItem[] searchItems)
        {
            if (searchItems == null)
                throw new ArgumentNullException(nameof(searchItems));

            if (IsRunning)
                return;

            IsRunning = true;
            LastExecutedAt = DateTime.UtcNow;

            Logger.BotLogger.LogInfo("Silent search started", "SEARCH_AUTOMATION");

            try
            {
                //refresh Cookies and MS-CV
                thisUUP.UpdateCookie().ConfigureAwait(false).GetAwaiter();
                thisUUP.GenerateNewCV();

                Dictionary<SearchItem, string> ignoreUpdateIDs = GetIgnorableUpdateIDs(searchItems);
                foreach (var item in searchItems)
                {
                    ignoreUpdateIDs.TryGetValue(item, out string updateID);
                    InternalRunSearch(item.DeviceFamily, item.Ring, item.Arch, item.BranchCodename, item.SilentSearch, item.ShouldMergeString, updateID);
                }
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "SEARCH_AUTOMATION");
            }

            Logger.BotLogger.LogInfo("Silent search finished", "SEARCH_AUTOMATION");

            IsRunning = false;
            LastExecutedAt = DateTime.UtcNow;
        }

        private static void InternalRunSearch(string family, string ring, string arch, string branchCodename, 
                                              bool isSilent, bool mergeString, string ignoreUpdateID = null, int tries = 0)
        {
            try
            {
                var extractor = new CoreCheck(family, ring, arch, branchCodename, thisUUP);
                Logger.BotLogger.LogVerbose($"{(isSilent ? "[SILENT]" : "[ACTIVE]")} Searching for: {family} - {ring} - {arch} - {branchCodename}", "SEARCH_AUTOMATION");

                var downloads = extractor.FetchBuild(false, ignoreUpdateID);

                if (downloads != null)
                {
                    if (downloads.IsUpdateIDIgnored)
                        Logger.BotLogger.LogVerbose("UpdateID ignored for (" + family + " - " + ring + " - " + arch + ")", "SEARCH_AUTOMATION");
                    else if (!downloads.IsUpdateIDIgnored && downloads.DownloadInfo?.Length > 0)
                    {
                        var res = extractor.ReadBuildVersion(downloads, false);
                        if (res != null)
                        {
                            if (mergeString)
                                res.DeviceFamily = family + "-" + branchCodename;

                            res.Ring = ring;

                            if (res.ReleaseType == null)
                                res.ReleaseType = "Not defined";

                            if (res.UpdateID != null)
                            {
                                //If BUILD is not present, add it.
                                if (res.BuildID != null && !SharedDBcmd.BuildIdExists(res.BuildID))
                                    SharedDBcmd.AddNewBuild(res.BuildID, res.Build, res.CreatedDate.Value);

                                if (!SharedDBcmd.UpdateIdExists(res.UpdateID))
                                {
                                    //New UpdateID (BuildID can be the same).
                                    extractor.ExportJSON(downloads, TelegramBotSettings.SaveJSONPath + System.IO.Path.DirectorySeparatorChar + res.UpdateID + ".json", false);
                                    SharedDBcmd.AddNewUpdateId(res.UpdateID, res.BuildLong, res.Title, res.Description);
                                }

                                if (!SharedDBcmd.BuildFromExists(res.DeviceFamily, res.Ring, res.ReleaseType, res.Architecture, res.UpdateID))
                                {
                                    //Add to BuildsFrom
                                    SharedDBcmd.AddNewBuildsFrom(res.DeviceFamily, res.Ring, res.ReleaseType, res.Architecture, res.UpdateID, res.FlightID, res.LastTimeChanged, res.RevisionNumber, res.BuildID);
#if PRODUCTION
                                    if (!isSilent)
                                    {
                                        //Notify subscribed users!
                                        NotifyGroups(res);
                                        NotifyUsers(res);
                                        EnqueueByDeviceFamily(res);
                                    }
#else
                                    Console.WriteLine(OutputVals(res, CultureInfo.InvariantCulture));
                                    Console.WriteLine("\n---------------------------------------------------");

                                    if (!isSilent)
                                    {
                                        NotifyGroups(res);
                                        NotifyUsers(res);
                                        EnqueueByDeviceFamily(res);
                                    }
#endif
                                }
                            }
                            else
                            {
                                Logger.BotLogger.LogWarning("BuildInfo UpdateID is null (" + family + " " + ring + " " + arch + ")", "SEARCH_AUTOMATION");
                                if (tries < MAX_RETRIES - 1)
                                    InternalRunSearch(family, ring, arch, branchCodename, isSilent, mergeString, ignoreUpdateID, ++tries);
                            }
                        }
                        else
                        {
                            Logger.BotLogger.LogWarning("BuildInfo is null (" + family + " " + ring + " " + arch + ")", "SEARCH_AUTOMATION");
                            if (tries < MAX_RETRIES - 1)
                                InternalRunSearch(family, ring, arch, branchCodename, isSilent, mergeString, ignoreUpdateID, ++tries);
                        }
                    }
                }
                else
                {
                    if (tries < MAX_RETRIES - 1)
                        InternalRunSearch(family, ring, arch, branchCodename, isSilent, mergeString, ignoreUpdateID, ++tries);
                }
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "SEARCH_AUTOMATION");
                if (tries < MAX_RETRIES - 1)
                    InternalRunSearch(family, ring, arch, branchCodename, isSilent, mergeString, ignoreUpdateID, ++tries);
            }

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
        }

        public static void EnqueueByDeviceFamily(BuildInfo buildInfo)
        {
            if (buildInfo == null)
                throw new ArgumentNullException(nameof(buildInfo));

            var deviceFamily = buildInfo.DeviceFamily.Split('-')[0];
            var ring = buildInfo.Ring;

            var branchCodename = buildInfo.DeviceFamily.Count(c => c == '-') > 1 ? buildInfo.DeviceFamily.Split('-')[1] : null;

            var items = GetFilteredSilentSearchItems(SearchHelpers.GetSearchItems, deviceFamily, ring, branchCodename);

            foreach (var item in items)
                _silentQueue.Enqueue(item);
        }

        public static void NotifyGroups(BuildInfo buildInfo)
        {
            //Key = ID, Value = VALUE
            var groupFiltered = SharedDBcmd.GetSubscribedGroups().FindAll(element => {
                return IsMatch(element.Value, buildInfo.DeviceFamily);
            });

            groupFiltered.ForEach(async groupDevice => {
                try
                {
                    await TGHost.Bot.SendTextMessageAsync(groupDevice.Key, OutputVals(buildInfo, SharedDBcmd.GetGroupState(groupDevice.Key).CultureInfo), Telegram.Bot.Types.Enums.ParseMode.Html).ConfigureAwait(false);
                }
                catch (ApiRequestException ex)
                {
                    Logger.BotLogger.LogWarning($"Error: bot can't push notifications to a group anymore. --> {groupDevice.Key} || {ex.Message}", "SEARCH_AUTOMATION");

                    if (!SharedDBcmd.IsGroupSubscribed(groupDevice.Key))
                        SharedDBcmd.RemoveGroupSubscriber(groupDevice.Key);
                }
            });
        }

        public static void NotifyUsers(BuildInfo buildInfo)
        {
            //Key = ID, Value = VALUE
            var userFiltered = SharedDBcmd.GetSubscribedUsers().FindAll(element => {
                return IsMatch(element.Value, buildInfo.DeviceFamily);
            });

            userFiltered.ForEach(async userDevice =>
            {
                try
                {
                    await TGHost.Bot.SendTextMessageAsync(userDevice.Key, OutputVals(buildInfo, SharedDBcmd.GetUserState(userDevice.Key).CultureInfo), Telegram.Bot.Types.Enums.ParseMode.Html).ConfigureAwait(false);
                }
                catch (ApiRequestException ex)
                {
                    //Forbidden: bot was blocked by the user
                    if (ex.Message == "Forbidden: bot was blocked by the user")
                    {
                        Logger.BotLogger.LogWarning($"User {userDevice.Key} blocked the bot. Unsubscribing.", "SEARCH_AUTOMATION");

                        if (SharedDBcmd.IsUserSubscribed(userDevice.Key))
                            SharedDBcmd.RemoveUserSubscriber(userDevice.Key);
                    }
                }
                catch (Exception ex)
                {
                    Logger.BotLogger.LogError(ex.Message, "SEARCH_AUTOMATION");
                }
            });
        }

        private static bool IsMatch(string item, string filter) => item.Split(',').Contains(filter.Split('-')[0]);

        public static string OutputVals(BuildInfo bi, CultureInfo cultureInfo)
        {
            if (bi == null)
                throw new ArgumentNullException(nameof(bi));

            var icon = "";

            if (SearchHelpers.GetEmojiIconFromDeviceFamily.TryGetValue(bi.DeviceFamily, out var iconTmp))
                icon = iconTmp + " ";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format(cultureInfo, MessageHelpers.GetLocalizedText("P_NewUpdateFound", cultureInfo),
                    icon, bi.DeviceFamily, bi.Ring, bi.Architecture));

            if (bi.Title != null)
                sb.AppendLine(string.Format(cultureInfo, MessageHelpers.GetLocalizedText("P_NewUpdateFoundTitle", cultureInfo), bi.Title));

            sb.AppendLine(string.Format(cultureInfo, MessageHelpers.GetLocalizedText("P_NewUpdateFoundBLE", cultureInfo), bi.BuildLong));

            if (bi.Description != null && bi.Title != null && bi.Description != bi.Title)
                sb.AppendLine(string.Format(cultureInfo, MessageHelpers.GetLocalizedText("P_NewUpdateFoundDE", cultureInfo), bi.Description));

            return sb.ToString();
        }

        private static Dictionary<SearchItem, string> GetIgnorableUpdateIDs(SearchItem[] searchItems)
        {
            var dto = SharedDBcmd.GetLatestUpdateIDForeachBuildFrom();

            var dict = new Dictionary<SearchItem, string>();

            if (dto == null || dto.Count == 0)
                return dict;

            for (int i = 0; i < searchItems.Length; i++)
            {
                var element = dto.FindIndex(item =>
                item.Architecture.ToUpper(CultureInfo.InvariantCulture) == searchItems[i].Arch.ToUpper(CultureInfo.InvariantCulture) &&
                item.DeviceFamily.ToUpper(CultureInfo.InvariantCulture) == (searchItems[i].ShouldMergeString ? 
                    $"{searchItems[i].DeviceFamily}-{searchItems[i].BranchCodename}".ToUpper(CultureInfo.InvariantCulture) : 
                    searchItems[i].DeviceFamily.ToUpper(CultureInfo.InvariantCulture)) &&
                item.Ring.ToUpper(CultureInfo.InvariantCulture) == searchItems[i].Ring.ToUpper(CultureInfo.InvariantCulture));

                if (element != -1)
                {
                    dict.Add(searchItems[i], dto[element].UpdateID);
                }
            }

            return dict;
        }

        public static async Task UpdateCookie()
        {
            try
            {
                await thisUUP.UpdateCookie().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                SharedDBcmd.TraceError(-1, $"Internal error: {ex.Message}");
            }
        }
    }
}
