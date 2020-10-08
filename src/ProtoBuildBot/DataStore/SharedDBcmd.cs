using Microsoft.Data.Sqlite;
using ProtoBuildBot.Classes;
using ProtoBuildBot.DataStore.Dtos;
using ProtoBuildBot.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using KV = System.Collections.Generic.KeyValuePair<string, object>;

namespace ProtoBuildBot.DataStore
{
    public static class SharedDBcmd
    {
        readonly static DBEngine _db = DBEngine.DBInstance;
        readonly static ConcurrentDictionary<long, UserState> _userStatesCache = new ConcurrentDictionary<long, UserState>();
        readonly static ConcurrentDictionary<long, GroupState> _groupsCache = new ConcurrentDictionary<long, GroupState>();

        public static bool KeyAlreadyRedeemed(long id, string chiave) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT * FROM Users WHERE mKey = @chiave AND NOT id = @utente) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("utente", id), new KV("chiave", chiave));

        public static bool IsKeyBlocked(string chiave) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT * FROM BlockedKeys WHERE mKey = @chiave) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("chiave", chiave));
        public static DateTime? UserRegisteredAt(long id) => _db.GetDateTimeCommand("SELECT RegisteredAt FROM Users WHERE id = @utente", new KV("utente", id));
        public static void AddNewRegisteredUser(long id, string chiave) => _db.RunCommand("INSERT INTO Users (id, authLevel, mKey, RegisteredAt) VALUES (@utente, 1, @chiave, datetime('now'))", new KV("utente", id), new KV("chiave", chiave));
        
        public static void AddToHistoryCommands(long id, string command) => _db.RunCommand("INSERT INTO HistoryCommands (commandDateTime, issuedBy, commandMessage) VALUES (datetime('now'), @utente, @comando)", new KV("utente", id), new KV("comando", command));

        public static void UpdateUserAuthLevel(long id, int level) => _db.RunCommand("UPDATE Users SET authLevel = @livello WHERE id = @utente", new KV("utente", id), new KV("livello", level));
        public static void UpdateUserLanguage(long id, string language) => _db.RunCommand("UPDATE Users SET Language = @lingua WHERE id = @utente", new KV("utente", id), new KV("lingua", language));

        public static List<int> GetAdminList() => _db.GetListIntCommand($"SELECT id FROM Users WHERE authLevel = {(int)AuthLevel.ADMIN}");
        public static List<int> GetModList() => _db.GetListIntCommand($"SELECT id FROM Users WHERE authLevel = {(int)AuthLevel.MOD}");
        public static List<KeyValuePair<long, string>> GetSubscribedUsers() => _db.GetListKVLongStringCommand("SELECT id, Filter FROM SubscribedUsers");
        public static List<KeyValuePair<long, string>> GetSubscribedGroups() => _db.GetListKVLongStringCommand("SELECT id, Filter FROM SubscribedGroups");

        public static void AddNewUserSubscriber(long id, string filter) => _db.RunCommand("INSERT INTO SubscribedUsers (id, SubscribedAt, Filter) VALUES (@utente, datetime('now'), @filtro)", new KV("utente", id), new KV("filtro", filter));
        public static void RemoveUserSubscriber(long id) => _db.RunCommand("DELETE FROM SubscribedUsers WHERE id = @utente", new KV("utente", id));
        public static bool IsUserSubscribed(long id) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT id FROM SubscribedUsers WHERE id = @utente) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("utente", id));
        public static string GetUserFilterSubscriber(long id) => _db.GetStringCommand("SELECT Filter FROM SubscribedUsers WHERE id = @utente", new KV("utente", id));
        public static void UpdateUserFilterSubscriber(long id, string filter) => _db.RunCommand("UPDATE SubscribedUsers SET filter = @filtro WHERE id = @utente", new KV("utente", id), new KV("filtro", filter));

        private static void AddGroupRegistration(long id) => _db.RunCommand("INSERT INTO RegisteredGroups (id) VALUES (@gruppo)", new KV("gruppo", id));
        private static void RemoveGroupRegistration(long id) => _db.RunCommand("DELETE FROM RegisteredGroups WHERE id = @gruppo", new KV("gruppo", id));
        //private static bool IsGroupRegisteredFromDB(long id) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT id FROM RegisteredGroups WHERE id = @gruppo) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("gruppo", id));

        public static void UpdateGroupLanguageFromDB(long id, string language) => _db.RunCommand("UPDATE RegisteredGroups SET Language = @lingua WHERE id = @gruppo", new KV("gruppo", id), new KV("lingua", language));
        public static bool IsGroupSubscribed(long id) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT id FROM SubscribedGroups WHERE id = @gruppo) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("gruppo", id));
        public static void AddNewGroupSubscriber(long id, string filter) => _db.RunCommand("INSERT INTO SubscribedGroups VALUES (@gruppo, @filtro, datetime('now'))", new KV("gruppo", id), new KV("filtro", filter));
        public static void UpdateGroupFilterSubscriber(long id, string filter) => _db.RunCommand("UPDATE SubscribedGroups SET filter = @filtro WHERE id = @gruppo", new KV("gruppo", id), new KV("filtro", filter));
        public static void RemoveGroupSubscriber(long id) => _db.RunCommand("DELETE FROM SubscribedGroups WHERE id = @gruppo", new KV("gruppo", id));
        public static string GetGroupFilterSubscriber(long id) => _db.GetStringCommand("SELECT filter FROM SubscribedGroups WHERE id = @gruppo", new KV("gruppo", id));

        public static void TraceError(long id, string errorMessage, [CallerMemberName] string memberName = "") => _db.RunCommand("INSERT INTO Errors(id, mDateTime, errorMessage) VALUES (@utente, datetime('now'), @errorMessage)", new KV("utente", id), new KV("errorMessage", errorMessage + " | ON " + memberName + "()"));
        public static void TraceError(long id, string errorMessage, DateTime timestamp, [CallerMemberName] string memberName = "") => _db.RunCommand("INSERT INTO Errors(id, mDateTime, errorMessage) VALUES (@utente, @dateTime, @errorMessage)", new KV("utente", id), new KV("dateTime", timestamp), new KV("errorMessage", errorMessage + " | ON " + memberName + "()"));

        public static bool BuildIdExists(string buildId) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT * FROM Builds WHERE BuildID = @buildId) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("buildId", buildId));
        public static void AddNewBuild(string buildId, string build, DateTime createdDate) => _db.RunCommand("INSERT INTO Builds VALUES (@buildId, @build, @crTime)", new KV("buildId", buildId), new KV("build", build), new KV("crTime", createdDate));

        public static bool UpdateIdExists(string updateId) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT * FROM Updates WHERE UpdateID = @updateId) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("updateId", updateId));
        public static void AddNewUpdateId(string updateId, string buildLong, string title, string description) => _db.RunCommand("INSERT INTO Updates (UpdateID, BuildLong, Title, Description) VALUES (@updateId, @buildLong, @title, @description)", new KV("updateId", updateId), new KV("buildLong", buildLong), new KV("title", title), new KV("description", description));

        public static List<ValueTuple<string, string, string>> GetAllRingStates() => _db.GetList3TupleStringCommand("SELECT DeviceFamily, Ring, BuildLong FROM (SELECT * FROM BuildsFrom WHERE BuildsFrom.AddedAt IN (SELECT MAX(AddedAt) FROM BuildsFrom GROUP BY DeviceFamily, Ring) ORDER BY DeviceFamily) AS Filter, Updates WHERE Filter.UpdateID = Updates.UpdateID");

        public static bool BuildFromExists(string devFamily, string ring, string relType, string arch, string updateId) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT * FROM BuildsFrom WHERE BuildsFrom.AddedAt = (SELECT MAX(AddedAt) FROM BuildsFrom WHERE DeviceFamily = @devFamily  AND Ring = @ring AND ReleaseType = @relType AND Architecture = @arch AND UpdateID = @updateId)) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("devFamily", devFamily), new KV("ring", ring), new KV("relType", relType), new KV("arch", arch), new KV("updateId", updateId));
        public static void AddNewBuildsFrom(string devFamily, string ring, string relType, string arch, string updateId, string flightId, string lastTimeChanged, string revisionNumber, string buildId) => _db.RunCommand("INSERT INTO BuildsFrom (DeviceFamily, Ring, ReleaseType, Architecture, AddedAt, UpdateID, FlightID, LastTimeChanged, RevisionNumber, BuildID) VALUES (@devFamily, @ring, @relType, @arch, datetime('now'), @updateId, @flightId, @lastTimeChanged, @revisionNumber, @buildId)", new KV("devFamily", devFamily), new KV("ring", ring), new KV("relType", relType), new KV("arch", arch), new KV("updateId", updateId), new KV("flightId", flightId), new KV("lastTimeChanged", lastTimeChanged), new KV("revisionNumber", revisionNumber == null ? DBNull.Value : (object)int.Parse(revisionNumber, CultureInfo.InvariantCulture)), new KV("buildId", buildId == null ? DBNull.Value : (object)buildId));

        public static string GetUpdateID(string devFamily, string arch, string build) => _db.GetStringCommand("SELECT Updates.UpdateID FROM Updates, BuildsFrom WHERE BuildsFrom.DeviceFamily = @devFamily AND BuildsFrom.Architecture = @arch AND BuildsFrom.UpdateID = Updates.UpdateID AND Updates.BuildLong LIKE @build LIMIT 1;", new KV("devFamily", devFamily), new KV("arch", arch), new KV("build", $"%{build}%"));

        public static List<ValueTuple<string, string, string, string, string, string, string>> GetBuildInfo(string updateId) => _db.GetList7TupleStringCommand("SELECT DeviceFamily, Ring, ReleaseType, Architecture, FlightID, RevisionNumber, BuildLong FROM BuildsFrom, Updates WHERE BuildsFrom.UpdateID = Updates.UpdateID AND BuildsFrom.UpdateID = @updateId", new KV("updateId", updateId));

        public static bool IsAuthorizedForLinks(string id) => _db.GetBoolCommand("SELECT CASE WHEN EXISTS (SELECT * FROM GeneratedIds WHERE id = @id) THEN CAST (1 AS BIT) ELSE CAST (0 AS BIT) END", new KV("id", id));

        public static List<BuildLab_DTO> GetLatestBuildLabForeachBuildFrom()
        {
            string sql = $"SELECT DeviceFamily, Ring, Architecture, BuildLong FROM (SELECT MAX(AddedAt), DeviceFamily, Ring, Architecture, UpdateID FROM BuildsFrom GROUP BY DeviceFamily, Ring, Architecture) up, Updates WHERE up.UpdateID = Updates.UpdateID;";

            List<BuildLab_DTO> dtos = new List<BuildLab_DTO>();

            try
            {
                using (var command = new SqliteCommand(sql, _db.GetCurrentSqliteConnection()))
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        dtos.Add(new BuildLab_DTO(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return null;
            }
        }

        public static List<IgnorableUpdateID_DTO> GetLatestUpdateIDForeachBuildFrom()
        {
            string sql = $"SELECT UpdateID, DeviceFamily, Ring, Architecture FROM (SELECT MAX(AddedAt), UpdateID, DeviceFamily, Ring, Architecture FROM BuildsFrom GROUP BY DeviceFamily, Ring, Architecture);";

            List<IgnorableUpdateID_DTO> dtos = new List<IgnorableUpdateID_DTO>();

            try
            {
                using (var command = new SqliteCommand(sql, _db.GetCurrentSqliteConnection()))
                {
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        dtos.Add(new IgnorableUpdateID_DTO(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
                }

                return dtos;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return null;
            }
        }

        /// <summary>
        /// Get UserState from DB
        /// </summary>
        /// <param name="id">Telegram ID of the user to query</param>
        /// <returns>A UserState if the user is found otherwise null.</returns>
        private static UserState GetUserStateFromDB(long id)
        {
            //INT, STRING
            string sql = $"SELECT authLevel, Language FROM Users WHERE id = @utente";

            Tuple<int, string> valTu = null;

            try
            {
                using (var command = new SqliteCommand(sql, _db.GetCurrentSqliteConnection()))
                {
                    command.Parameters.Add(new SqliteParameter("utente", id));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        valTu = new Tuple<int, string>(reader.GetInt32(0), reader.GetString(1));
                }

                if (valTu == null)
                    return new UserState(id, AuthLevel.UNREGISTERED);

                return new UserState(id,
                    (AuthLevel)valTu.Item1,
                    CultureInfo.GetCultureInfo(valTu.Item2));
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return null;
            }
        }

        private static GroupState GetGroupStateFromDB(long id)
        {
            string sql = $"SELECT Language FROM RegisteredGroups WHERE id = @group";

            Tuple<long, string> valTu = null;

            try
            {
                using (var command = new SqliteCommand(sql, _db.GetCurrentSqliteConnection()))
                {
                    command.Parameters.Add(new SqliteParameter("group", id));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        valTu = new Tuple<long, string>(id, reader.GetString(0));
                }

                if (valTu == null)
                    return new GroupState(id, new CultureInfo("en-US"), false);

                return new GroupState(valTu.Item1, CultureInfo.GetCultureInfo(valTu.Item2), true);
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return null;
            }
        }

        /// <summary>
        /// Get UserState from either cache or DB.
        /// </summary>
        /// <param name="id">User Telegram id.</param>
        /// <returns>UserState or null if there was an internal error.</returns>
        public static UserState GetUserState(long id)
        {
            if (_userStatesCache.TryGetValue(id, out UserState user))
                return user;

            user = GetUserStateFromDB(id);
            if (user == null) //There was an error
                return null;

            _userStatesCache.TryAdd(id, user);
            return user;
        }

        /// <summary>
        /// Updates UserState cache from DB. It will be removed from cache if there was an internal error.
        /// </summary>
        /// <param name="id">User Telegram id.</param>
        public static void UpdateUserState(long id)
        {
            _ = _userStatesCache.TryGetValue(id, out UserState user);
            if (user == null) return;

            var currUser = GetUserStateFromDB(id);

            if (currUser == null) //There was an error, remove from cache list
            {
                _userStatesCache.TryRemove(id, out _);
                return;
            }
            
            user.AuthLevel = currUser.AuthLevel;
            user.CultureInfo = currUser.CultureInfo;
        }

        /// <summary>
        /// Get GroupState from either cache or DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsGroupRegistered(long id)
        {
            if (_groupsCache.TryGetValue(id, out var grp))
                return grp.IsRegistered;

            var groupReg = GetGroupStateFromDB(id);
            if (groupReg == null)
                return false;

            _groupsCache.TryAdd(id, groupReg);
            return groupReg.IsRegistered;
        }

        public static GroupState GetGroupState(long id)
        {
            if (_groupsCache.TryGetValue(id, out var grp))
                return grp;

            var groupReg = GetGroupStateFromDB(id);
            if (groupReg == null)
                return null;

            _groupsCache.TryAdd(id, groupReg);
            return groupReg;
        }

        public static void UpdateGroupLanguage(long id, string language)
        {
            UpdateGroupLanguageFromDB(id, language);
            if (_groupsCache.TryGetValue(id, out var grp))
                grp.CultureInfo = new CultureInfo(language);
        }

        /// <summary>
        /// Register or unregister a group
        /// </summary>
        /// <param name="id">Telegram Chat id</param>
        /// <param name="register">true to register the group, false to unregister it</param>
        public static void UpdateGroupRegistrationStatus(long id, bool register)
        {
            if (IsGroupRegistered(id))
            {
                if (!register)
                {
                    _groupsCache.TryRemove(id, out _);
                    if (IsGroupSubscribed(id))
                        RemoveGroupSubscriber(id);

                    RemoveGroupRegistration(id);
                }
            }
            else
            {
                if (register)
                {
                    AddGroupRegistration(id);

                    if (_groupsCache.TryGetValue(id, out var grp))
                        grp.IsRegistered = true;
                    else
                        _groupsCache.TryAdd(id, new GroupState(id, new CultureInfo("en-US"), true));
                }
            }

        }
    }
}
