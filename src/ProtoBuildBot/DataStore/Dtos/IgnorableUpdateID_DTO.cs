using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.DataStore.Dtos
{
    public class IgnorableUpdateID_DTO
    {
        public string UpdateID { get; set; }
        public string DeviceFamily { get; set; }
        public string Ring { get; set; }
        public string Architecture { get; set; }

        public IgnorableUpdateID_DTO(string updateID, string deviceFamily, string ring, string architecture)
        {
            UpdateID = updateID;
            DeviceFamily = deviceFamily;
            Ring = ring;
            Architecture = architecture;
        }
    }
}
