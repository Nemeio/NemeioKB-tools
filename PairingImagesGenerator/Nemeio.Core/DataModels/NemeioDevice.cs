namespace Nemeio.Core.DataModels
{
    public class NemeioDevice
    {
        public ComPort ComPort { get; }

        public NemeioDeviceId Id { get; }

        public NemeioDevice(ComPort comPort, string deviceId)
        {
            ComPort = comPort;
            Id = new NemeioDeviceId(deviceId);
        }
    }
}
