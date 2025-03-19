using Nemeio.Core.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nemeio.Core.Services.Updates
{
    [Serializable]
    public class TempUpdate
    {
        public string Url { get; }

        public string Version { get; }

        public int Type { get; }

        public string Checksum { get; }

        public TempUpdate(string url, string version, int type, string checksum)
        {
            Url = url;
            Version = version;
            Type = type;
            Checksum = checksum;
        }

        public static TempUpdate FromDomainModel(Update update) => new TempUpdate(update.Url.ToString(), update.VersionProxy, (int)update.Type, update.Checksum);

        public Update ToDomainModel() => new Update(Url, Version, (UpdateType)Type, Checksum);
    }
}
