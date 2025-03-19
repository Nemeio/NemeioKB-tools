using System;
using Nemeio.Core.JsonModels;

namespace Nemeio.Core.Services
{
    internal class UpdateFirmwareWatcher : ResourceWatcher<UpdateModel>
    {
        public UpdateFirmwareWatcher(INemeioHttpService nemeioHttpService, Action<UpdateModel> checkedResourceHandler)
            : base(nemeioHttpService.CheckForUpdates, checkedResourceHandler)
        {
            Start();
        }
    }
}
