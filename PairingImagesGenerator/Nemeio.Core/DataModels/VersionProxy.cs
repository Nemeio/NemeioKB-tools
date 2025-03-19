using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nemeio.Core.DataModels
{
    public enum VersionStatus
    {
        Higher,
        Equal,
        Lower
    }

    public class VersionProxy
    {
        private const int IS_EQUAL = 0;
        private const int IS_LATER = 1;
        private const int IS_EARLIER = -1;
        private const int DEFAULT_FIELD_COUNT = 3;

        private readonly Version _version;

        public VersionProxy(Version version) => _version = version;

        public VersionProxy(string version) => _version = new Version(version);

        public bool IsHigherThan(VersionProxy version) => Compare(version) == VersionStatus.Higher;

        public VersionStatus Compare(VersionProxy version)
        {
            switch (CompareTo(version))
            {
                case IS_LATER:
                    return VersionStatus.Higher;
                case IS_EARLIER:
                    return VersionStatus.Lower;
                case IS_EQUAL:
                default:
                    return VersionStatus.Equal;
            }
        }

        private int CompareTo(VersionProxy version) => _version.CompareTo(version._version);

        public override string ToString() => _version.ToString(DEFAULT_FIELD_COUNT);

        public static implicit operator string(VersionProxy value) => value?.ToString();
    }
}
