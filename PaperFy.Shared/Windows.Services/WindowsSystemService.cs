using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Utilities;
using System.Reflection;
using System.Security.Principal;

namespace PaperFy.Shared.Windows.Services
{
    public class WindowsSystemService : IPlatformSystemService, IDisposable
    {
        public string ApplicationExecutablePath => Path.Combine(ApplicationPath, Assembly.GetEntryAssembly().GetName().Name + ".exe");

        public string ApplicationPath => AppDomain.CurrentDomain.BaseDirectory;

        public InstallationInformation InstallationType
        {
            get
            {
                if (!AppDomain.CurrentDomain.BaseDirectory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), StringComparison.OrdinalIgnoreCase) && !AppDomain.CurrentDomain.BaseDirectory.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), StringComparison.OrdinalIgnoreCase))
                {
                    return InstallationInformation.User;
                }
                return InstallationInformation.Administrator;
            }
        }

        public bool IsRunningAsAdministrator => new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

        public string Username => Environment.UserName;

        public void Dispose()
        {
            KeyboardUtilities.Stop();
            MouseUtilities.Stop();
        }

        public void StartKeyboardListener()
        {
            KeyboardUtilities.Listen();
        }

        public void StartMouseListener()
        {
            MouseUtilities.Listen();
        }

        public void StopKeyboardListener()
        {
            KeyboardUtilities.Stop();
        }

        public void StopMouseListener()
        {
            MouseUtilities.Stop();
        }
    }
}
