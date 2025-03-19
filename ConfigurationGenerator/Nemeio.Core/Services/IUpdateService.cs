using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Nemeio.Core.DataModels;
using Nemeio.Core.Services.Updates;

namespace Nemeio.Core.Services
{
    public interface IUpdateService
    {
        UpdateStatus Status { get; }

        void AddUpdate(Update update);

        void SetupOnUpdateAdded(Action added);

        void SetupOnUpdateStarted(Action added);

        void SetupOnUpdateEnded(Action added);

        void SetupShowMessage(Action<string> showMessage);

        void SetupOnUpdateStatusChanged(Action<UpdateStatus> statusChanged);

        void FirmwareUpdateFailed(int errorCode);

        Task DownloadUpdates(Action onDownloadCompleted, Action onErrorRaised);

        Task ApplyUpdates(Keyboard keyboard);

        Task CheckKeyboardUpdatesInProgress(Keyboard keyboard);

        void CheckAppUpdate(VersionProxy appVersionProxy);

        bool VerifyFile(string filePath, string checksum);

        void Abort();
    }
}
