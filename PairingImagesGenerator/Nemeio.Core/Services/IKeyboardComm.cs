using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services.Layouts;
using Version = System.Version;

namespace Nemeio.Core.Services
{
    public interface IKeyboardComm
    {
        Task KeepAlive();

        void SetupOnLayoutIdsReceived(Func<LayoutId[], Task> action);

        Task SendGetLayoutIds();

        Task DeleteLayout(LayoutId layoutId);

        Task SendLayout(Layout layout);

        Task ApplyLayout(LayoutId layoutId);

        Task SendGetBatteryLevel();

        Firmware SendGetFirmwareVersion();

        Task SetHid();

        Task LeaveHid();

        Task GetFirmwareVersions();

        Task SendGetCrashInformation();

        void SetupOnCrashInformationReceived(Action<IList<KeyboardFailure>> action);

        void Close();

        void SetupSetOsLayout(Action<LayoutId> setOsLayout);

        void SetupCheckBatteryLevel(Action<BatteryLevel> action);

        void SetupOnKeyPressed(Action<NemeioIndexKeystroke[]> action);

        void SetupGetKeyboardVersion(Action<DataModels.VersionProxy, DataModels.VersionProxy> action);

        void SetupFirmwareUpdateError(Action<int> action);

        void SetupOnReceiveKeyboardSerialNumber(Action<NemeioSerialNumber> action);

        Task SendFirmware(byte[] payload);

        Task GetKeyboardSerialNumber();

        void UpdateNrf(Uri initPacketPath, Uri firmwareBinaryPath);
    }

    public class NemeioKeystrokesReceivedEventArgs : EventArgs
    {
        public NemeioIndexKeystroke[] Keystrokes { get; set; }
    }
}
