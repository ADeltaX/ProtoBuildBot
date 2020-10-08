using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace ProtoBuildBot.Structures
{
    [DataContract]
    public class RingState
    {
        [DataMember(Name = "DeviceFamily")]
        public string DeviceFamily { get; set; }

        [DataMember(Name = "Ring")]
        public string Ring { get; set; }

        [DataMember(Name = "BuildLab")]
        public string BuildLab { get; set; }
    }
}
