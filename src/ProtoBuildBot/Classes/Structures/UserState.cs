using ProtoBuildBot.Enums;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using Telegram.Bot.Types;

namespace ProtoBuildBot.Classes
{
    public class UserState : UserStateBase
    {
        public long UserId { get; set; } //Identificativo
        public AuthLevel AuthLevel { get; set; } //Authorization level >> -1 = Unauthorized (no keys are linked) | 1 = User | 2 = Mod | 3 = Admin | 4 = Creator
        public string LastCommand { get; set; } //Last executed command - useful for generic replies
        public int State { get; set; } //In what state the user is in - Unused at the moment
        public object ObjectState { get; set; } //Unused

        public UserState(long userId, AuthLevel level, CultureInfo cultureInfo = null)
        {
            UserId = userId;
            AuthLevel = level;
            CultureInfo = cultureInfo ?? CultureInfo.GetCultureInfo("en-US");
            State = 0;
            Queue = new ConcurrentQueue<Message>();
        }

        public void ResetUserState()
        {
            State = 0;
            ObjectState = null;
        }
        public override long GetID() => UserId;
    }
}
