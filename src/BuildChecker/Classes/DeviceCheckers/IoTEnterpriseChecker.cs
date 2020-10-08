using BuildChecker.Classes.DeviceBuilderExtensions;

namespace BuildChecker.Classes.DeviceCheckers
{
    public sealed class IoTEnterpriseChecker : BaseChecker
    {
        protected override string DeviceFamily { get => "IOTENTERPRISE"; }

        public IoTEnterpriseChecker(string Branch, string Build, string Arch, string Flight, string Ring, UUP uup)
            : base(Branch, Build, Arch, Flight, Ring, uup)
        { }

        public override FileRequests FetchBuild(bool updateAgentOnly, string ignoreUpdateID = null) 
            => uup.GetFileRequests(new IoTEnterpriseBuilderExtension(Branch, Build, Arch, Flight, Ring), updateAgentOnly, ignoreUpdateID).GetAwaiter().GetResult();
    }
}