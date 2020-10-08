using ProtoBuildBot.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.Classes.Messages.Base
{
    public abstract class UnregMessageBase : MessageBase
    {
        public override AuthLevel MinimalAuthorizationLevel => AuthLevel.UNREGISTERED;
    }
}
