using ProtoBuildBot.Enums;

namespace ProtoBuildBot.Classes.Messages.Base
{
    public abstract class CreatorMessageBase : MessageBase
    {
        public override AuthLevel MinimalAuthorizationLevel => AuthLevel.CREATOR;
    }
}
