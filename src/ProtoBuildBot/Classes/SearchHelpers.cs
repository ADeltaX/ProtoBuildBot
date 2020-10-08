using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.Classes
{
    public static class SearchHelpers
    {
        /// <summary>
        /// Dictionary of | Device Family - Emoji Icon |
        /// </summary>
        public static readonly Dictionary<string, string> GetEmojiIconFromDeviceFamily = new Dictionary<string, string>
        {
            { "DESKTOP", "🖥"},
            { "HOLOLENS", "👓" },
            { "HOLOLENS2", "👓" },
            { "IOT", "🎛" },
            { "TEAM", "📺" },
            { "SERVICING", "⚙️" },
            { "MOBILE", "📱" },
        };


        public static readonly SearchItem[] GetSearchItems = new[]
        {
            new SearchItem("DESKTOP",   "Dev",      "x86",      "RS5",          shouldMergeString: false,   silentSearch: false),
            new SearchItem("DESKTOP",   "RETAIL",   "x86",      "B20H1",        shouldMergeString: false,   silentSearch: false),
            new SearchItem("DESKTOP",   "RP",       "x86",      "RS5",          shouldMergeString: false,   silentSearch: false),
            new SearchItem("DESKTOP",   "Beta",      "x86",     "RS5",          shouldMergeString: false,   silentSearch: false),

            new SearchItem("IOT",       "RETAIL",   "x86",      "RS4",          shouldMergeString: false,   silentSearch: false),

            new SearchItem("HOLOLENS",  "RETAIL",   "x86",      "RS4",          shouldMergeString: false,   silentSearch: false),

            new SearchItem("HOLOLENS2", "WIF",      "arm64",    "B19H1",        shouldMergeString: false,   silentSearch: false),
            new SearchItem("HOLOLENS2", "WIS",      "arm64",    "B19H1",        shouldMergeString: false,   silentSearch: false),
            new SearchItem("HOLOLENS2", "RETAIL",   "arm64",    "B19H1",        shouldMergeString: false,   silentSearch: false),

            new SearchItem("TEAM",      "WIF",      "amd64",      "RS2",          shouldMergeString: false,   silentSearch: false),
            new SearchItem("TEAM",      "WIS",      "amd64",      "RS2",          shouldMergeString: false,   silentSearch: false),

            new SearchItem("SERVICING", "RETAIL",   "amd64",    "RS3",          shouldMergeString: true,    silentSearch: false),
            new SearchItem("SERVICING", "RETAIL",   "amd64",    "RS4",          shouldMergeString: true,    silentSearch: false),
            new SearchItem("SERVICING", "RETAIL",   "amd64",    "RS5",          shouldMergeString: true,    silentSearch: false),
            new SearchItem("SERVICING", "RETAIL",   "amd64",    "B19H1",        shouldMergeString: true,    silentSearch: false),

            //
            //SILENT
            //

            new SearchItem("IOT",       "RETAIL",   "ARM64",    "RS4",          shouldMergeString: false,   silentSearch: true),
            new SearchItem("IOT",       "RETAIL",   "AMD64",    "RS4",          shouldMergeString: false,   silentSearch: true),
            new SearchItem("IOT",       "RETAIL",   "ARM",      "RS4",          shouldMergeString: false,   silentSearch: true),

            new SearchItem("TEAM",      "WIF",      "ARM64",    "RS2",          shouldMergeString: false,   silentSearch: true),
            new SearchItem("TEAM",      "WIS",      "ARM64",    "RS2",          shouldMergeString: false,   silentSearch: true),

            new SearchItem("DESKTOP",   "RETAIL",   "AMD64",    "B20H1",        shouldMergeString: false,   silentSearch: true),
            new SearchItem("DESKTOP",   "RETAIL",   "ARM64",    "B20H1",        shouldMergeString: false,   silentSearch: true),
            new SearchItem("DESKTOP",   "RP",       "AMD64",    "RS5",          shouldMergeString: false,   silentSearch: true), //TODO: ReleasePreview
            new SearchItem("DESKTOP",   "RP",       "ARM64",    "RS5",          shouldMergeString: false,   silentSearch: true), //TODO: ReleasePreview
            new SearchItem("DESKTOP",   "Beta",     "AMD64",    "RS5",          shouldMergeString: false,   silentSearch: true),
            new SearchItem("DESKTOP",   "Beta",     "ARM64",    "RS5",          shouldMergeString: false,   silentSearch: true),
            new SearchItem("DESKTOP",   "Dev",      "AMD64",    "RS5",          shouldMergeString: false,   silentSearch: true),
            new SearchItem("DESKTOP",   "Dev",      "ARM64",    "RS5",          shouldMergeString: false,   silentSearch: true),
        };


        //Global filter for GetRingStates
        public static readonly SearchItem[] GetSearchItemsForGRS = new SearchItem[]
        {
            new SearchItem("IOT",       "WIF",      "x86",      "RS4"),
            new SearchItem("IOT",       "RETAIL",   "x86",      "RS4"),

            new SearchItem("HOLOLENS",  "WIF",      "x86",      "RS4"),
            new SearchItem("HOLOLENS",  "WIS",      "x86",      "RS4"),
            new SearchItem("HOLOLENS",  "RETAIL",   "x86",      "RS4"),

            new SearchItem("HOLOLENS2", "WIF",      "arm64",    "B19H1"),
            new SearchItem("HOLOLENS2", "WIS",      "arm64",    "B19H1"),
            new SearchItem("HOLOLENS2", "RETAIL",   "arm64",    "B19H1"),

            new SearchItem("TEAM",      "WIF",      "amd64",      "RS2"),
            new SearchItem("TEAM",      "WIS",      "amd64",      "RS2"),

            new SearchItem("DESKTOP",   "RETAIL",   "x86",      "B20H1"),
            new SearchItem("DESKTOP",   "RP",       "x86",      "RS5"),
            new SearchItem("DESKTOP",   "Beta",      "x86",      "RS5"),
            new SearchItem("DESKTOP",   "Dev",      "x86",      "RS5"),

            new SearchItem("MOBILE",    "WIF",      "arm",      "RS2"),
            new SearchItem("MOBILE",    "WIF",      "arm",      "FEATURE2"),

            new SearchItem("SERVICING", "RETAIL",   "amd64",    "RS3"),
            new SearchItem("SERVICING", "RETAIL",   "amd64",    "RS4"),
            new SearchItem("SERVICING", "RETAIL",   "amd64",    "RS5"),
            new SearchItem("SERVICING", "RETAIL",   "amd64",    "B19H1"),
        };
    }
}
