using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes
{
    public class GroupState : UserStateBase
    {
        public long ChatId { get; set; }
        public bool IsRegistered { get; set; }

        public GroupState(long chatId, CultureInfo cultureInfo, bool isRegistered)
        {
            ChatId = chatId;
            IsRegistered = isRegistered;
            CultureInfo = cultureInfo;
            Queue = new ConcurrentQueue<Message>();
        }

        public override long GetID() => ChatId;
    }
}
