using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Interface
{
    public interface IPlatformSystemService : IDisposable
    {
        string ApplicationExecutablePath { get; }

        string ApplicationPath { get; }

        InstallationInformation InstallationType { get; }

        bool IsRunningAsAdministrator { get; }

        string Username { get; }

        void StartKeyboardListener();

        void StartMouseListener();

        void StopKeyboardListener();

        void StopMouseListener();
    }
}