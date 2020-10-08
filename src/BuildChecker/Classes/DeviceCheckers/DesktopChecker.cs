using BuildChecker.Classes.DeviceBuilderExtensions;

namespace BuildChecker.Classes.DeviceCheckers
{
    public sealed class DesktopChecker : BaseChecker
    {
        protected override string DeviceFamily { get => "DESKTOP"; }

        public DesktopChecker(string Branch, string Build, string Arch, string Flight, string Ring, UUP uup)
            : base(Branch, Build, Arch, Flight, Ring, uup)
        { }

        public override FileRequests FetchBuild(bool updateAgentOnly, string ignoreUpdateID = null) 
            => uup.GetFileRequests(new DesktopBuilderExtension(Branch, Build, Arch, Flight, Ring), updateAgentOnly, ignoreUpdateID).GetAwaiter().GetResult();
    }
}