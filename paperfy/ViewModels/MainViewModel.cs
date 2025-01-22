using Paperfy.Views;

namespace Paperfy.ViewModels;

public class MainViewModel : ViewModelBase
{
    public CaptureControlsViewModel CaptureControlsViewModel { get; }

    public MainViewModel() 
    {
        CaptureControlsViewModel = new CaptureControlsViewModel();
    }
}
