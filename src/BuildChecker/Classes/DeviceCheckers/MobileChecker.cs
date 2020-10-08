using BuildChecker.Classes.DeviceBuilderExtensions;

namespace BuildChecker.Classes.DeviceCheckers
{
    public sealed class MobileChecker : BaseChecker
    {
        protected override string DeviceFamily { get => "MOBILE"; }

        public MobileChecker(string branch, string build, string arch, string flight, string ring, UUP uup)
            : base(branch, build, arch, flight, ring, uup)
        { }

        public override FileRequests FetchBuild(bool updateAgentOnly, string ignoreUpdateID = null) 
            => uup.GetFileRequests(new MobileBuilderExtension(Branch, Build, Arch, Flight, Ring), updateAgentOnly, ignoreUpdateID).GetAwaiter().GetResult();
    }
}
