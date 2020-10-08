using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.Classes
{
    public class SearchItem
    {
        public string DeviceFamily { get; set; }
        public string Ring { get; set; }
        public string Arch { get; set; }
        public string BranchCodename { get; set; }
        public bool ShouldMergeString { get; set; }
        public bool SilentSearch { get; set; }
        public SearchItem(string deviceFamily, string ring, string arch, string branchCodename, bool shouldMergeString = false, bool silentSearch = false)
        {
            DeviceFamily = deviceFamily;
            Ring = ring;
            Arch = arch;
            BranchCodename = branchCodename;
            ShouldMergeString = shouldMergeString;
            SilentSearch = silentSearch;
        }
    }
}
