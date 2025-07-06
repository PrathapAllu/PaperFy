using Avalonia.Controls;
using Paperfy.ViewModels;

namespace Paperfy.Views;

public partial class ImageEditorWindow : Window
{
    public ImageEditorWindow()
    {
        InitializeComponent();
    }

    private void ExportComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            var viewModel = DataContext as ImageEditorViewModel;

            switch (selectedItem.Tag?.ToString())
            {
                case "Single":
                    viewModel?.ExportSingleImageCommand.Execute(null);
                    break;
                case "Document":
                    viewModel?.ExportToDocumentCommand.Execute(null);
                    break;
            }

            // Reset selection
            comboBox.SelectedItem = null;
        }
    }
}