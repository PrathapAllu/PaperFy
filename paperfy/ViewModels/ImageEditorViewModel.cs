using PaperFy.Shared.AppManager;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace Paperfy.ViewModels
{
    public class ImageEditorViewModel : ViewModelBase
    {
        private ObservableCollection<byte[]> _images;

        public ObservableCollection<byte[]> Images
        {
            get => _images;
            set => this.RaiseAndSetIfChanged(ref _images, value);
        }

        public ImageEditorViewModel()
        {
            LoadImages();
        }

        private void LoadImages()
        {
            var markedImages = ApplicationManager.IimageProcessor.GetMarkedImages();
            Images = new ObservableCollection<byte[]>(markedImages.Select(x => x.image));
        }
    }
}
