using System;
using System.Collections.Generic;
using System.Linq;

namespace ProtoBuildBot.DataStore
{
    public static class InitDBcmd
    {
        //SQLITE doesn't support modifying tables. AAAAAAAAAAAAAAAAAAAAAAAAAAA

        public const int CurrentDatabaseVersion = 1;

        public static readonly string CurrentSqlCmd = @"CREATE TABLE IF NOT EXISTS Users (id int NOT NULL PRIMARY KEY, authLevel int NOT NULL, mKey varchar(25) NOT NULL, RegisteredAt DATETIME, Language varchar(8) DEFAULT 'en-US');
CREATE TABLE IF NOT EXISTS BlockedKeys (mKey varchar(25) NOT NULL PRIMARY KEY);
CREATE TABLE IF NOT EXISTS RegisteredGroups (id NVARCHAR NOT NULL PRIMARY KEY, Language varchar(8) DEFAULT 'en-US');
CREATE TABLE IF NOT EXISTS SubscribedUsers (id int NOT NULL PRIMARY KEY, SubscribedAt DATETIME, Filter NVARCHAR, foreign key(id) references Users(id));
CREATE TABLE IF NOT EXISTS Builds (BuildID TEXT PRIMARY KEY, Build NVARCHAR NOT NULL, CreatedDate datetime);
CREATE TABLE IF NOT EXISTS Updates (UpdateID TEXT PRIMARY KEY, BuildLong NVARCHAR NOT NULL, Title NVARCHAR, Description NVARCHAR);
CREATE TABLE IF NOT EXISTS SubscribedGroups (id NVARCHAR NOT NULL PRIMARY KEY, Filter NVARCHAR, RegisteredAt DATETIME);
CREATE TABLE IF NOT EXISTS BuildsFrom (id INTEGER PRIMARY KEY, DeviceFamily NVARCHAR, Ring NVARCHAR, ReleaseType NVARCHAR, Architecture NVARCHAR, AddedAt DATETIME, UpdateID NVARCHAR NOT NULL, FlightID NVARCHAR, LastTimeChanged NVARCHAR NOT NULL, RevisionNumber int, BuildID NVARCHAR, foreign key(BuildID) references Builds(BuildID), foreign key(UpdateID) references Updates(UpdateID));
CREATE TABLE IF NOT EXISTS HistoryCommands(idCommand INTEGER PRIMARY KEY, commandDateTime datetime NOT NULL, issuedBy int NOT NULL, commandMessage NVARCHAR, foreign key(issuedBy) references Users(id));
CREATE TABLE IF NOT EXISTS Errors(idError INTEGER PRIMARY KEY, id int NOT NULL, mDateTime datetime NOT NULL, errorMessage NVARCHAR, foreign key(id) references Users(id));";

        public static readonly Dictionary<int, string> MigrateSqlCmd = new Dictionary<int, string>()
        {
            {
                1,
                @"CREATE TABLE IF NOT EXISTS BlockedKeys (mKey varchar(25) NOT NULL PRIMARY KEY);
CREATE TABLE IF NOT EXISTS RegisteredGroups (id NVARCHAR NOT NULL PRIMARY KEY, Language varchar(8) DEFAULT 'en-US');
ALTER TABLE Users ADD COLUMN Language varchar(8) DEFAULT 'en-US';
ALTER TABLE SubscribedUsers ADD COLUMN Filter NVARCHAR;"
            }

        };

        public static readonly Dictionary<int, Action> PostUpgradeActions = new Dictionary<int, Action>()
        {
            {
                1,
                new Action(RunPUScript1)
            }
        };

        //ALL POST UPGRADE SCRIPTS

        private static void RunPUScript1()
        {
            //Migrate "SubscribedUsers" by adding "all subscribed device families"
            var subscribedUsers = SharedDBcmd.GetSubscribedUsers();

            foreach (var user in subscribedUsers)
            {
                var subscribableDeviceFamily = ProtoBuildBot.Classes.SearchHelpers.GetSearchItemsForGRS.GroupBy(item => item.DeviceFamily).Select(itemSelect => itemSelect.Key).ToArray();

                DBEngine.DBInstance.RunCommand("UPDATE SubscribedUsers SET Filter = @filtro WHERE id = @utente", 
                    new KeyValuePair<string, object>("filtro", string.Join(',', subscribableDeviceFamily)), new KeyValuePair<string, object>("utente", user.Key));
            }

            //Migrate "RegisteredGroups" by adding every SubscribedGroups
            var subscribedGroups = SharedDBcmd.GetSubscribedGroups();

            foreach (var group in subscribedGroups)
                SharedDBcmd.UpdateGroupRegistrationStatus(group.Key, true);

        }
    }
}
