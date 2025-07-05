using PaperFy.Shared.AppManager;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Paperfy.ViewModels
{
    public class ImageEditorViewModel : ViewModelBase
    {
        private ObservableCollection<byte[]> _images;

        private byte[] _selectedImage;

        public byte[] SelectedImage
        {
            get => _selectedImage;
            set => this.RaiseAndSetIfChanged(ref _selectedImage, value);
        }

        public ICommand SelectImageCommand { get; }

        public ObservableCollection<byte[]> Images
        {
            get => _images;
            set => this.RaiseAndSetIfChanged(ref _images, value);
        }

        public ImageEditorViewModel()
        {
            LoadImages();
            // Set first image as selected by default
            if (Images?.Any() == true)
                SelectedImage = Images.First();

            SelectImageCommand = ReactiveCommand.Create<byte[]>(image => SelectedImage = image);
        }

        private void LoadImages()
        {
            var markedImages = ApplicationManager.IimageProcessor.GetMarkedImages();
            Images = new ObservableCollection<byte[]>(markedImages.Select(x => x.image));
        }
    }
}
