using Nemeio.Core.DataModels;
using System;
using System.Security.Cryptography;

namespace Nemeio.Core.Services.Layouts
{
    public class LayoutId : GuidType<LayoutId>
    {
        public LayoutId(string value) : base(value) { }

        public LayoutId(byte[] layoutId) : base(layoutId) { }

        public static LayoutId Compute(LayoutInfo layoutInfo, byte[] imageBytes)
        {
            var bytes = layoutInfo.GetBytes().Append(imageBytes);
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(bytes);
                return new LayoutId(hash);
            }
        }
    }
}
