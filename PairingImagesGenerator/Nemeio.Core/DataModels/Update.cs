using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Nemeio.Core.Services;

namespace Nemeio.Core.DataModels
{
    public enum UpdateType
    {
        App,
        Stm32,
        Nrf
    }

    public class Update : IComparable<Update>
    {
        private const int BEFORE_OBJECT = -1;
        private const int EQUAL_OBJECT = 0;
        private const int AFTER_OBJECT = 1;

        public Uri Url { get; }

        public VersionProxy VersionProxy { get; }

        public UpdateType Type { get; }

        public string Checksum { get; }

        public string InstallerPath { get; private set; }

        public Update(string url, string version, UpdateType type, string checksum)
        {
            Url = new Uri(url);
            VersionProxy = new VersionProxy(new Version(version));
            Type = type;
            Checksum = checksum;
        }

        public Update(string url, VersionProxy versionProxy, UpdateType type, string checksum)
        {
            Url = new Uri(url);
            VersionProxy = versionProxy;
            Type = type;
            Checksum = checksum;
        }

        public int CompareTo(Update other)
        {
            if (Type == UpdateType.App)
            {
                return AFTER_OBJECT;
            }

            if (Type == UpdateType.Stm32)
            {
                return BEFORE_OBJECT;
            }

            return EQUAL_OBJECT;
        }

        public bool IsKeyboardUpdate() => Type == UpdateType.Stm32 || Type == UpdateType.Nrf;

        public void ComputeInstallerPath(IDocument documentService) => InstallerPath = Path.Combine(documentService.TemporaryFolderPath, Path.GetFileName(Url.ToString()));
    }
}
