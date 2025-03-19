using Nemeio.Core.DataModels;
using Nemeio.Core.Services.Layouts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nemeio.Core.Services
{
    public interface IOsKeyboardProxy
    {
        Layout GetCurrentOsLayout();

        IEnumerable<Layout> GetInstalledLayouts();

        IKeyboardComm GetKeyboardComm(ComPort comPort);

        void RefreshLayouts();

        void SetLayoutFromKeyboardToOs(LayoutId layoutId);

        Layout GetLayoutById(LayoutId id);

        void Close();

        void StopSendKey();

        void PostHidStringKeys(string[] keys);

        Task Init(Action<ComPort> onNemeioKeyboardAdded, Action<ComPort> onNemeioKeyboardRemoved, Func<Task> onOsLayoutChanged);
    }
}
