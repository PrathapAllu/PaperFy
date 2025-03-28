﻿using PaperFy.Shared.AppManager;
using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Services;
using ReactiveUI;
using System.Windows.Input;

namespace Paperfy.ViewModels
{
    public class CaptureControlsViewModel : ParentViewModel
    {
        private bool isRecordingAudio { get; set; }

        private bool isPausedOrResume;
        public bool IsPauseVisible => !isPausedOrResume;
        public bool IsResumeVisible => isPausedOrResume;

        private bool isDocumentingCancelled { get; set; }

        private readonly IScreenCaptureService screenCaptureService;

        public ICommand CancelDocumentingCommand { get; }

        public ICommand PauseDocumentingCommand { get; }

        public ICommand ResumeDocumentingCommand { get; }

        public ICommand StartDocumentingCommand { get; }

        public ICommand StopDocumentingCommand { get; }

        public ICommand ToggleAudioDocumentingCommand { get; }


        public CaptureControlsViewModel(MainViewModel parent) : base(parent)
        {

            screenCaptureService = ApplicationManager.ScreenCaptureService;

            StartDocumentingCommand = ReactiveCommand.Create(StartDocumenting);
            StopDocumentingCommand = ReactiveCommand.Create(() => { });
            PauseDocumentingCommand = ReactiveCommand.Create(() =>
            {
                isPausedOrResume = true;
                this.RaisePropertyChanged(nameof(IsPauseVisible));
                this.RaisePropertyChanged(nameof(IsResumeVisible));
            });
            ResumeDocumentingCommand = ReactiveCommand.Create(() =>
            {
                isPausedOrResume = false;
                this.RaisePropertyChanged(nameof(IsPauseVisible));
                this.RaisePropertyChanged(nameof(IsResumeVisible));
            });

            CancelDocumentingCommand = ReactiveCommand.Create(() => { });
            ToggleAudioDocumentingCommand = ReactiveCommand.Create(() => { });
        }

        public void StartDocumenting()
        {
            base.Parent.Minimize();
            ApplicationManager.DocumenterService?.StartDocumenting(false);
            
        }
    }
}