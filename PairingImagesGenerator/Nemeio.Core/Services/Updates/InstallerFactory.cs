using System;
using System.Collections.Generic;
using System.Text;
using Nemeio.Core.DataModels;
using Nemeio.Core.Extensions;

namespace Nemeio.Core.Services.Updates
{
    public class InstallerFactory
    {
        private IDocument _documentService;

        public InstallerFactory(IDocument documentService)
        {
            _documentService = documentService;
        }

        public BaseInstaller CreateInstaller(UpdateType type)
        {
            switch (type)
            {
                case UpdateType.Stm32:
                    return new StmInstaller(_documentService);
                case UpdateType.Nrf:
                    return new NrfInstaller(_documentService);
                case UpdateType.App:
                    if (this.IsOSXPlatform())
                    {
                        return new OSXAppInstaller(_documentService);
                    }
                    else
                    {
                        return new WinAppInstaller(_documentService);
                    }
                default:
                    throw new ArgumentException("Value is unknow", nameof(type));
            }
        }
    }
}
