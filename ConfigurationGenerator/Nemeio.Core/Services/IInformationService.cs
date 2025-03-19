using System;
using System.Collections.Generic;
using System.Text;
using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services
{
    public interface IInformationService
    {
        VersionProxy GetAppVersion();
    }
}
