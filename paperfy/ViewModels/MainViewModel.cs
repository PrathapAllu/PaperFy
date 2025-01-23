using Paperfy.ViewModels;
using Paperfy.Views;
using PaperFy.Shared.Windows.Service;

namespace Paperfy.ViewModels 
{
    public class MainViewModel : ViewModelBase
    {
        public CaptureControlsViewModel CaptureControlsViewModel { get; }

        public MainViewModel(ApplicationStateService applicationStateService, string[] arguments)
        {
            CaptureControlsViewModel = new CaptureControlsViewModel(this);
        }
    }
}


