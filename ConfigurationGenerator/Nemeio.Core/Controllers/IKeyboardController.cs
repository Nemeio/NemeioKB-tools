using System.Threading.Tasks;
using Nemeio.Core.DataModels;
using Nemeio.Core.Services;
using Nemeio.Core.Services.Layouts;

namespace Nemeio.Core.Controllers
{
    public interface IKeyboardController
    {
        Task InitKeyboard(ComPort comPort);

        void Disconnect(ComPort comPort);

        void DisconnectAll();

        Task SetFallbackCommunication();

        Task SetDefaultCommunication();

        Task SwitchLayout();

        Task RefreshLayout();

        Task UpdateLayout(LayoutId id);

        Task CheckApplicationUpdate();

        Task RemoveLayoutFromKeyboard(LayoutId id);

        Keyboard GetKeyboard();
    }
}
