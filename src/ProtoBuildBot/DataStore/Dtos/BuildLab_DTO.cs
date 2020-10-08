using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.DataStore.Dtos
{
    public class BuildLab_DTO
    {
        public string DeviceFamily { get; set; }
        public string Ring { get; set; }
        public string Architecture { get; set; }
        public string BuildLab { get; set; }

        public BuildLab_DTO(string deviceFamily, string ring, string architecture, string buildLab)
        {
            DeviceFamily = deviceFamily;
            Ring = ring;
            Architecture = architecture;
            BuildLab = buildLab;
        }
    }
}
