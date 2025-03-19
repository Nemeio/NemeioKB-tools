using Nemeio.Core.DataModels;
using Nemeio.Core.JsonModels;
using Nemeio.Core.Services.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    public enum KeyboardState
    {
        Available,
        Updating
    }

    public class Keyboard
    {
        private readonly IOsKeyboardProxy _osKeyboardProxy;
        private readonly IKeyboardComm _keyboardComm;
        private KeepAlive _keepAlive;
        private BatteryChecker _batteryChecker;
        private KeyboardUpdateChecker _keyboardKeyboardUpdateChecker;
        private readonly KeyboardCrashLogger _crashLogger;
        private LayoutId[] _kbLayouts;
        private LayoutId[] _lastSyncedLayoutsOnKb;
        private LayoutId[] _defaultLayouts;
        private bool _callRefresh = false;
        private readonly IUpdateService _updateService;
        private readonly IKbInfoDisplayer _kbInfoDisplayer;

        public Firmware Firmware { get; }
        public Layout Layout { get; private set; }
        public KeyboardState State { get; private set; }
        public VersionProxy Stm32VersionProxy { get; private set; }
        public VersionProxy NrfVersionProxy { get; private set; }

        private INemeioHttpService _httpService;

        public Keyboard(ComPort comPort, IOsKeyboardProxy osKeyboardProxy, IKbInfoDisplayer infoDisplayer, IUpdateService updateSrv, INemeioHttpService httpSrv, Action<BatteryLevel> setBatteryLevel, Action<NemeioIndexKeystroke[]> keyPressed)
        {
            State = KeyboardState.Available;

            _httpService = httpSrv;
            _osKeyboardProxy = osKeyboardProxy;

            _crashLogger = new KeyboardCrashLogger();
            _updateService = updateSrv;
            _kbInfoDisplayer = infoDisplayer;

            _keyboardComm = _osKeyboardProxy.GetKeyboardComm(comPort);
            _keyboardComm.SetupOnLayoutIdsReceived(SyncLayouts);
            _keyboardComm.SetupCheckBatteryLevel(setBatteryLevel);
            _keyboardComm.SetupOnCrashInformationReceived(_crashLogger.WriteKeyboardCrashLog);
            //_keyboardComm.SetupGetKeyboardVersion(async (stm, nrf) => await ReceiveVersionFromKeyboard(stm, nrf));
            _keyboardComm.SetupFirmwareUpdateError(FirmwareUpdateFailed);
            _keyboardComm.SetupOnKeyPressed(keyPressed);
            _keyboardComm.SendGetLayoutIds();

            _keepAlive = new KeepAlive(_keyboardComm);
            _batteryChecker = new BatteryChecker(_keyboardComm);
            _keyboardKeyboardUpdateChecker = new KeyboardUpdateChecker(this, _keyboardComm);
        }

        private async Task SyncLayouts(LayoutId[] kbLayoutIds)
        {
            var activeOsLayout = _osKeyboardProxy.GetCurrentOsLayout();
            var osInstalledLayouts = _osKeyboardProxy.GetInstalledLayouts();
            await DeleteExtraLayouts(kbLayoutIds, osInstalledLayouts);
            await SendMissingLayouts(kbLayoutIds, activeOsLayout, osInstalledLayouts);

            if (!_callRefresh)
                await SetCurrentLayout(kbLayoutIds, activeOsLayout);

            _lastSyncedLayoutsOnKb = osInstalledLayouts.Select(l => l.LayoutId).ToArray();
            _defaultLayouts = osInstalledLayouts.Where(x => x.LayoutInfo.Default).Select(x => x.LayoutId).ToArray();
            _keyboardComm.SetupSetOsLayout(ConfigChanged);

            _callRefresh = false;
        }

        private async Task DeleteExtraLayouts(LayoutId[] kbLayoutIds, IEnumerable<Layout> osInstalledLayouts) => 
            await kbLayoutIds
                .Except(osInstalledLayouts.Where(x => x.Enable).Select(l => l.LayoutId))
                .ForEachAsync(l => _keyboardComm.DeleteLayout(l));

        private async Task SendMissingLayouts(LayoutId[] layoutIds, Layout activeOsLayout,
            IEnumerable<Layout> osInstalledLayouts) => await osInstalledLayouts
                .Where(l => !layoutIds.Contains(l.LayoutId) && l.LayoutId != activeOsLayout.LayoutId && l.Enable)
                .ForEachAsync((l) => _keyboardComm.SendLayout(l));

        private async Task SetCurrentLayout(IEnumerable<LayoutId> kbLayoutIds, Layout activeOsLayout)
        {
            if (kbLayoutIds.Contains(activeOsLayout.LayoutId))
            {
                await _keyboardComm.ApplyLayout(activeOsLayout.LayoutId);
            }
            else
            {
                await _keyboardComm.SendLayout(activeOsLayout);
            }

            Layout = _osKeyboardProxy.GetLayoutById(activeOsLayout.LayoutId);
        }

        private void ConfigChanged(LayoutId layoutId)
        {
            Layout = _osKeyboardProxy.GetLayoutById(layoutId);

            _osKeyboardProxy.SetLayoutFromKeyboardToOs(layoutId);
        }

        public async Task RefreshLayoutFromDatabaseToKeyboard()
        {
            _callRefresh = true;
            _osKeyboardProxy.RefreshLayouts();

            await _keyboardComm.SendGetLayoutIds();
        }

        public async Task UpdateLayoutFromDatabase(LayoutId id)
        {
            _callRefresh = true;
            _osKeyboardProxy.RefreshLayouts();

            await _keyboardComm.DeleteLayout(id);
            await _keyboardComm.SendGetLayoutIds();
            await _keyboardComm.ApplyLayout(id);
        }

        public async Task DeleteLayout(LayoutId id) => await _keyboardComm.DeleteLayout(id);

        public async Task SetLayoutFromOsToKeyboard()
        {
            if (_lastSyncedLayoutsOnKb == null)
            {
                _lastSyncedLayoutsOnKb = new LayoutId[0];
            }

            await SyncLayouts(_lastSyncedLayoutsOnKb);
        }

        public async Task HandlePotentialCrashInformation() => await _keyboardComm.SendGetCrashInformation();

        public void Disconnect()
        {
            _keepAlive?.Stop();
            _batteryChecker?.Stop();
            _keyboardComm.Close();
        }

        internal LayoutId GetDefaultLayout() => _defaultLayouts.First();

        internal async Task SetHid(LayoutId layoutId)
        {
            await _keyboardComm.ApplyLayout(layoutId);
            await _keyboardComm.SetHid();
        }

        internal async Task LeaveHid() => await _keyboardComm.LeaveHid();

        async Task ReceiveVersionFromKeyboard(VersionProxy stmVersionProxy, VersionProxy nrfVersionProxy)
        {
            Stm32VersionProxy = stmVersionProxy;
            NrfVersionProxy = nrfVersionProxy;

            try
            {
                var infos = await _httpService.CheckForUpdates();
                var smtNeedUpdate = CheckIfNeedUpdate(infos.Cpu, stmVersionProxy);
                var nrfNeedUpdate = CheckIfNeedUpdate(infos.Ble, nrfVersionProxy);

                if (smtNeedUpdate || nrfNeedUpdate)
                {
                    _kbInfoDisplayer.ShowNotification("update_available_title", "keyboard_update_available_message");
                }
            }
            catch (HttpRequestException)
            {
                //  Nothing to do here
                //  KeyboardUpdateChecker will verify automatically for a new update later
            }
        }

        private bool CheckIfNeedUpdate(VersionModel cloudVersion, VersionProxy currentVersionProxy)
        {
            var version = new VersionProxy(cloudVersion.Version);
            var needUpdate = version.IsHigherThan(currentVersionProxy);

            if (needUpdate)
            {
                _updateService.AddUpdate(
                    new Update(
                        cloudVersion.Url.ToString(), 
                        version, 
                        UpdateType.Stm32,
                        cloudVersion.Checksum
                    )
                );
            }

            return needUpdate;
        }

        public async Task SendUpdateFile(byte[] file) => await _keyboardComm.SendFirmware(file);

        public void StartUpdate() => State = KeyboardState.Updating;

        public void EndUpdate() => State = KeyboardState.Available;

        public bool IsAvailable() => State == KeyboardState.Available;

        public async Task RequestVersions() => await _keyboardComm.GetFirmwareVersions();

        public async Task RequestSerialNumber() => await _keyboardComm.GetKeyboardSerialNumber();

        void FirmwareUpdateFailed(int errorCode)
        {
            EndUpdate();

            _updateService.FirmwareUpdateFailed(errorCode);
        }

        public void UpdateNrf(Uri initPacketPath, Uri firmwareBinaryPath) => _keyboardComm.UpdateNrf(initPacketPath, firmwareBinaryPath);
    }
}
