using System;
using System.Text;
using Nemeio.Core.DataModels.Configurator;
using Nemeio.Core.Extensions;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutInfo
    {
        public OsLayoutId OsLayoutId { get; }
        public bool Default { get; set; }
        public bool Hid { get; set; }
        public bool Mac { get; set; }

        public LayoutInfo(OsLayoutId osLayoutId, bool isDefault, bool isHid)
        {
            OsLayoutId = osLayoutId;
            Default = isDefault;
            Hid = isHid;
            Mac = this.IsOSXPlatform();
        }

        public byte[] GetBytes()
        {
            var id = Encoding.UTF8.GetBytes(OsLayoutId.ToString().ToCharArray());
            var @default = Encoding.UTF8.GetBytes(BoolToString(Default));
            var hid = Encoding.UTF8.GetBytes(BoolToString(Hid));
            var mac = Encoding.UTF8.GetBytes(BoolToString(Mac));

            var res = id.Append(@default).Append(hid).Append(mac);

            return res;
        }

        private string BoolToString(bool val) => val ? "1" : "0";
    }
}
