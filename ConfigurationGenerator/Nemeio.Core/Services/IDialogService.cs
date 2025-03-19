using System;

namespace Nemeio.Core.Services
{
    public interface IDialogService
    {
        void Message(string title, string msg);

        void Error(string msg);

        void ShowConfigurator(Uri uri);

        bool ShowYesNoQuestion(string title, string message);
    }
}
