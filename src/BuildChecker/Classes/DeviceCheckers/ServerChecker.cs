using BuildChecker.Classes.DeviceBuilderExtensions;

namespace BuildChecker.Classes.DeviceCheckers
{
    public sealed class ServerChecker : BaseChecker
    {
        protected override string DeviceFamily { get => "Windows.Server"; }

        public ServerChecker(string Branch, string Build, string Arch, string Flight, string Ring, UUP uup)
            : base(Branch, Build, Arch, Flight, Ring, uup)
        { }

        public override FileRequests FetchBuild(bool updateAgentOnly, string ignoreUpdateID = null) 
            => uup.GetFileRequests(new ServerBuilderExtension(Branch, Build, Arch, Flight, Ring), updateAgentOnly, ignoreUpdateID).GetAwaiter().GetResult();
    }
}