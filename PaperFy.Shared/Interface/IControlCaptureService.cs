using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Interface
{
    public interface IControlCaptureService
    {
        const int MaximumLabelLength = 144;

        string GetLabelAtPosition(Point point);

        bool IsPasswordAtPosition(Point point);

        static string GetTruncatedLabel(string label)
        {
            if (label != null && label.Length > 144)
            {
                return label.Substring(0, 144) + "...";
            }
            return label;
        }
    }
}