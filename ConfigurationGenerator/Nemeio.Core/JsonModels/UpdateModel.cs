namespace Nemeio.Core.JsonModels
{
    public class UpdateModel
    {
        public VersionModel Cpu { get; set; }

        public VersionModel Ble { get; set; }

        public VersionModel OSX { get; set; }

        public VersionModel Windows { get; set; }
    }
}
