using Microsoft.Data.Sqlite;
using ProtoBuildBot.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProtoBuildBot.DataStore
{
    public class DBEngine : IDisposable
    {
        public static DBEngine DBInstance { get; set; }

        private SqliteConnection _mDbConnection;

        public DBEngine()
        {
            DBInstance = this;

            bool creationNeeded = false;

            if (!System.IO.File.Exists(TelegramBotSettings.DatabaseName))
            {
                System.IO.File.Create(TelegramBotSettings.DatabaseName).Close();
                creationNeeded = true;
            }

            _mDbConnection = new SqliteConnection($"Data Source={TelegramBotSettings.DatabaseName};");
            _mDbConnection.Open();
            Logger.BotLogger.LogInfo(MessageHelpers.GetLocalizedText("Z_DBConnectionOpened", CultureInfo.InvariantCulture), "DBENGINE");

            if (creationNeeded)
            {
                Logger.BotLogger.LogInfo(MessageHelpers.GetLocalizedText("Z_DBCreatingTables", CultureInfo.InvariantCulture), "DBENGINE");
                RunCommand(InitDBcmd.CurrentSqlCmd);
                SetDatabaseStoreVersion(InitDBcmd.CurrentDatabaseVersion);
            }
            else
            {
                Logger.BotLogger.LogInfo(MessageHelpers.GetLocalizedText("Z_DBCheckingMigration", CultureInfo.InvariantCulture), "DBENGINE");

                var dbStoreVer = GetDatabaseStoreVersion();
                if (dbStoreVer > InitDBcmd.CurrentDatabaseVersion)
                    throw new Exception(MessageHelpers.GetLocalizedText("Z_DBStoreIsTooNew", CultureInfo.InvariantCulture)); //Oksir
                else if (dbStoreVer == InitDBcmd.CurrentDatabaseVersion)
                    Logger.BotLogger.LogInfo(MessageHelpers.GetLocalizedText("Z_DBNoMigration", CultureInfo.InvariantCulture), "DBENGINE");
                else
                {
                    Logger.BotLogger.LogInfo($"Migration needed. Database store version: {dbStoreVer} --> Database store version to upgrade to: {InitDBcmd.CurrentDatabaseVersion}", "DBENGINE");
                    MigrateDB(dbStoreVer, InitDBcmd.CurrentDatabaseVersion);
                    Logger.BotLogger.LogInfo(MessageHelpers.GetLocalizedText("Z_DBMigrationCompleted", CultureInfo.InvariantCulture), "DBENGINE");
                }

                Logger.BotLogger.LogInfo(MessageHelpers.GetLocalizedText("Z_DBCheckCompleted", CultureInfo.InvariantCulture), "DBENGINE");
            }
        }

        private void MigrateDB(int fromVersion, int toVersion)
        {
            for (int i = fromVersion + 1; i <= toVersion; i++)
            {
                Logger.BotLogger.LogInfo(string.Format(CultureInfo.InvariantCulture, MessageHelpers.GetLocalizedText("Z_RunningMigration", CultureInfo.InvariantCulture), i), "DBENGINE");
                RunCommand(InitDBcmd.MigrateSqlCmd[i]);
            }
                
            RunPostUpgradeDB(fromVersion, toVersion);

            SetDatabaseStoreVersion(toVersion);

            Vacuum();
        }

        private void RunPostUpgradeDB(int fromVersion, int toVersion)
        {
            for (int i = fromVersion + 1; i <= toVersion; i++)
            {
                if (InitDBcmd.PostUpgradeActions.TryGetValue(i, out var action))
                {
                    Logger.BotLogger.LogInfo(string.Format(CultureInfo.InvariantCulture, MessageHelpers.GetLocalizedText("Z_RunningPostUpgrade", CultureInfo.InvariantCulture), i), "DBENGINE");
                    action();
                }
            }
        }

        public SqliteConnection GetCurrentSqliteConnection() => _mDbConnection;

        public void SetDatabaseStoreVersion(int version) => RunCommand($"PRAGMA user_version={version}");
        public int GetDatabaseStoreVersion() => GetIntCommand("PRAGMA user_version;");

        public void Vacuum() => RunCommand("VACUUM");

        public void RunCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            try
            {
                using var command = new SqliteCommand(sql, _mDbConnection);
                foreach (var param in parameteres)
                    command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message + $"|| SQL ERROR: ({sql})", "DATABASE");
            }
        }

        public T GetCommand<T>(string sql, params KeyValuePair<string, object>[] parameters)
        {
            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameters)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        return reader.IsDBNull(0) ? default : reader.GetFieldValue<T>(0);
                }

                return default;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return default;
            }
        }


        public bool GetBoolCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        return reader.GetBoolean(0);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return false;
            }
        }

        public string GetStringCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        return reader.IsDBNull(0) ? default : reader.GetString(0);
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return null;
            }
        }

        public int GetIntCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        return reader.GetInt32(0);
                }

                return -1;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return -1;
            }
        }

        public DateTime? GetDateTimeCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        return reader.IsDBNull(0) ? (DateTime?)null : reader.GetDateTime(0);
                }

                return null;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return null;
            }
        }

        public List<int> GetListIntCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            List<int> ints = new List<int>();

            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        ints.Add(reader.GetInt32(0));
                }

                return ints;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return ints;
            }
        }

        //complex types

        public List<KeyValuePair<long, string>> GetListKVLongStringCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            List<KeyValuePair<long, string>> valuesList = new List<KeyValuePair<long, string>>();

            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        valuesList.Add(new KeyValuePair<long, string>(reader.GetInt64(0), reader.IsDBNull(1) ? null : reader.GetString(1)));
                }

                return valuesList;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return valuesList;
            }
        }

        public List<ValueTuple<string, string, string, string, string, string, string>> GetList7TupleStringCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            List<ValueTuple<string, string, string, string, string, string, string>> valuesList = new List<ValueTuple<string, string, string, string, string, string, string>>();

            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        valuesList.Add((reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.IsDBNull(5) ? "0" : reader.GetString(5), reader.GetString(6)));
                }

                return valuesList;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return valuesList;
            }
        }

        public List<ValueTuple<string, string, string>> GetList3TupleStringCommand(string sql, params KeyValuePair<string, object>[] parameteres)
        {
            List<ValueTuple<string, string, string>> valuesList = new List<ValueTuple<string, string, string>>();

            try
            {
                using (var command = new SqliteCommand(sql, _mDbConnection))
                {
                    foreach (var param in parameteres)
                        command.Parameters.Add(new SqliteParameter(param.Key, param.Value));

                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                        valuesList.Add((reader.GetString(0), reader.GetString(1), reader.GetString(2)));
                }

                return valuesList;
            }
            catch (Exception ex)
            {
                Logger.BotLogger.LogError(ex.Message, "DATABASE");
                return valuesList;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);  // Violates rule
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mDbConnection != null)
                {
                    _mDbConnection.Dispose();
                    _mDbConnection = null;
                }
            }
        }
    }
}
