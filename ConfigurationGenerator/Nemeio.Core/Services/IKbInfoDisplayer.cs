using Nemeio.Core.DataModels;

namespace Nemeio.Core.Services
{
    public interface IKbInfoDisplayer
    {
        void ShowNotification(string title, string message);

        void UpdateConnectionState(ConnectionStatus connectionStatus);

        void UpdateBatteryLevel(BatteryLevel level);

        void ShowUpgrading();
    }
}
