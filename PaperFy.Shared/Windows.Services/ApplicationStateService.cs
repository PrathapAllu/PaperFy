using Avalonia.Controls.ApplicationLifetimes;

namespace PaperFy.Shared.Windows.Service
{
    public class ApplicationStateService
    {
        public IClassicDesktopStyleApplicationLifetime desktop;

        public ApplicationStateService(IClassicDesktopStyleApplicationLifetime desktop)
        {
            this.desktop = desktop;
        }
    }
}