using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.Enums
{
    public enum AuthLevel
    {
        UNREGISTERED = -1, //No Msgs until HEH
        USER = 1,
        MOD = 2,
        ADMIN = 3,
        CREATOR = 4
    }
}
