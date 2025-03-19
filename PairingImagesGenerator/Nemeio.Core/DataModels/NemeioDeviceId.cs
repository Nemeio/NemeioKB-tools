namespace Nemeio.Core.DataModels
{
    public class NemeioDeviceId : StringType<NemeioDeviceId>
    {
        public NemeioDeviceId(string deviceId) : base(deviceId) { }
    }
}
