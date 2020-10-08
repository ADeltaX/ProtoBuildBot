using ProtoBuildBot.Enums;
using ProtoBuildBot;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes.Messages.Base
{
    public abstract class AdminMessageBase : MessageBase
    {
        public override AuthLevel MinimalAuthorizationLevel => AuthLevel.ADMIN;
    }
}
