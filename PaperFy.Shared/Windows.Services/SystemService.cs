using Avalonia;
using Avalonia.Input;
using PaperFy.Shared.AppManager;
using PaperFy.Shared.Common.Extensions;
using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Models;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PaperFy.Shared.Windows.Services
{
    public class SystemService
    {
        public const string ApplicationName = "PaperFy";

        private static readonly object SyncObject = new object();

        private static SystemService instance;

        public static SystemService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (SyncObject)
                    {
                        if (instance == null)
                        {
                            instance = new SystemService();
                        }
                    }
                }
                return instance;
            }
        }

        public string ApplicationExecutablePath => PlatformSystemService?.ApplicationExecutablePath ?? string.Empty;

        public string ApplicationPath => PlatformSystemService?.ApplicationPath ?? string.Empty;

        public Version ApplicationVersion => Assembly.GetEntryAssembly().GetName().Version;

        public long CurrentTimestamp => (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;


        public bool IsInstalledAsAdministrator
        {
            get
            {
                IPlatformSystemService platformSystemService = PlatformSystemService;
                if (platformSystemService == null)
                {
                    return false;
                }
                return platformSystemService.InstallationType == InstallationInformation.Administrator;
            }
        }

        public bool IsInstalledAsUser => !IsInstalledAsAdministrator;

        public bool IsRunningAsAdministrator => PlatformSystemService?.IsRunningAsAdministrator ?? false;

        public string LocalApplicationDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Paperfy");

        public TimeSpan MaximumDelayForRemovingLastAction => TimeSpan.FromMilliseconds(400.0);

        public double MaximumDoubleClickDistance => Math.Max(Application.Current?.PlatformSettings?.GetDoubleTapSize(PointerType.Mouse).Width ?? 20.0, Application.Current?.PlatformSettings?.GetDoubleTapSize(PointerType.Mouse).Height ?? 20.0);

        public TimeSpan MaximumDoubleClickTimeout => Application.Current?.PlatformSettings?.GetDoubleTapTime(PointerType.Mouse) ?? TimeSpan.FromMilliseconds(200.0);

        public string OSVersion => Environment.OSVersion.Version.ToString();

        public string SystemArchitecture => RuntimeInformation.OSArchitecture.ToString().ToLower();

        internal bool AreFileSystemOperationsAllowed { get; set; } = true;

        internal IPlatformSystemService PlatformSystemService { get; set; }

        private SystemService()
        {
            PlatformSystemService = ApplicationManager.PlatformSystemService;
        }

        public void DeleteDirectory(string path)
        {
            if (!AreFileSystemOperationsAllowed)
            {
                return;
            }
            try
            {
                Directory.Delete(path, recursive: true);
                
            }
            catch (Exception ex)
            {
            }
        }

        public void DeleteFile(string path)
        {
            if (!AreFileSystemOperationsAllowed)
            {
                return;
            }
            try
            {
                File.Delete(path);
            }
            catch (Exception exception)
            {
                
            }
        }

        public string GetSpecialKeyDescription(int specialKey)
        {
            Key key = (Key)specialKey;
            return key.ToString().AddSpacesBetweenCapitalLetters();
        }

        public bool IsSpecialKeyBackspace(int specialKey)
        {
            return specialKey == 2;
        }

        public bool IsSpecialKeyEnter(int specialKey)
        {
            return specialKey == 6;
        }

        public bool IsSpecialKeySpace(int specialKey)
        {
            return specialKey == 18;
        }

        public string ReadFile(string path)
        {
            if (!AreFileSystemOperationsAllowed)
            {
                return null;
            }
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        public T ReadFile<T>(string path)
        {
            if (!AreFileSystemOperationsAllowed)
            {
                return default(T);
            }
            return (T)(ReadFile(path)?.Deserialize(typeof(T)));
        }

        public byte[] ReadFileBytes(string path)
        {
            if (!AreFileSystemOperationsAllowed)
            {
                return null;
            }
            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception exception)
            {                
                throw;
            }
        }

        public void SaveFile(string path, byte[] contents)
        {
            if (!AreFileSystemOperationsAllowed)
            {
                return;
            }
            try
            {
                string directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                File.WriteAllBytes(path, contents);
            }
            catch (Exception exception)
            {
                
            }
        }

        public void SaveFile(string path, string contents)
        {
            if (!AreFileSystemOperationsAllowed)
            {
                return;
            }
            try
            {
                string directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
                File.WriteAllText(path, contents);
            }
            catch (Exception exception)
            {
                
            }
        }
    }
}
