using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Nemeio.Core.Services.Updates
{
    public class WinAppInstaller : AppInstaller
    {
        public WinAppInstaller(IDocument documentService)
            :base(documentService) { }

        public override async Task Install(string filePath)
        {
            await Task.Yield();

            var p = new Process();
            p.StartInfo.FileName = filePath;
            p.Start();

            Process.GetCurrentProcess().Kill();
        }

        public override string InstallerExtensionFilter() => ".exe";
    }
}
