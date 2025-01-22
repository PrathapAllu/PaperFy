using ReactiveUI;
using System.Windows.Input;

namespace Paperfy.ViewModels
{
    public class CaptureControlsViewModel : ParentViewModel
    {
        private bool isRecordingAudio { get; set; }

        private bool isPausedOrResume { get; set; }

        private bool isDocumentingCancelled { get; set; }


        public ICommand CancelDocumentingCommand { get; }

        public ICommand PauseDocumentingCommand { get; }

        public ICommand ResumeDocumentingCommand { get; }

        public ICommand StartDocumentingCommand { get; }

        public ICommand StopDocumentingCommand { get; }

        public ICommand ToggleAudioDocumentingCommand { get; }


        public CaptureControlsViewModel(MainViewModel parent) : base(parent)
        {
            StartDocumentingCommand = ReactiveCommand.Create(StartDocumenting);
            StopDocumentingCommand = ReactiveCommand.Create(() => { });
            PauseDocumentingCommand = ReactiveCommand.Create(() => { });
            ResumeDocumentingCommand = ReactiveCommand.Create(() => { });
            CancelDocumentingCommand = ReactiveCommand.Create(() => { });
            ToggleAudioDocumentingCommand = ReactiveCommand.Create(() => { });
        }

        public void StartDocumenting()
        { }
    }
}