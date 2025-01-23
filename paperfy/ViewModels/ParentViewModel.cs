namespace Paperfy.ViewModels
{
    public class ParentViewModel : ViewModelBase
    {
        public MainViewModel Parent { get; }

        protected ParentViewModel(MainViewModel parent)
        {
            Parent = parent;
        }
    }
}


