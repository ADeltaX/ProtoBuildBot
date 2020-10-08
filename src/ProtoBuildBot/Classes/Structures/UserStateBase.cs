using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes
{
    public abstract class UserStateBase
    {
        public CultureInfo CultureInfo { get; set; }
        public ConcurrentQueue<Message> Queue { get; set; } //FIFO
        public CancellationTokenSource CancellationTokenSource { get; set; } //Current thread lel
        public abstract long GetID();
    }
}
