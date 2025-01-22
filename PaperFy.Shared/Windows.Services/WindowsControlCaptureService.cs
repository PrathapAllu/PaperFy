using PaperFy.Shared.Interface;
using PaperFy.Shared.Windows.Models;
using System.Windows.Automation;

namespace PaperFy.Shared.Windows.Services
{
    public class WindowsControlCaptureService : IControlCaptureService
    {
        public string GetLabelAtPosition(Point point)
        {
            if (!Settings.Instance.CaptureLabels)
            {

                return null;
            }
            try
            {
                System.Windows.Automation.AutomationElement automationElement = AutomationElement.FromPoint(new Point(point.X, point.Y));
                _ = automationElement.Current.IsPassword;
                if (automationElement?.Current.Name == "Chrome Legacy Window")
                {
                    TreeWalker controlViewWalker = TreeWalker.ControlViewWalker;
                    AutomationElement automationElement2 = controlViewWalker.GetFirstChild(automationElement);
                  
                    while (automationElement2 != null && automationElement2.Current.ControlType != ControlType.Edit)
                    {
                        automationElement2 = controlViewWalker.GetNextSibling(automationElement2);
                    }
                    return IControlCaptureService.GetTruncatedLabel(automationElement2?.Current.Name ?? "");
                }
                _ = automationElement.Current.ControlType;
                _ = ControlType.ComboBox;
                if (automationElement != null)
                {
                    return IControlCaptureService.GetTruncatedLabel(automationElement.Current.Name);
                }

                return null;
            }
            catch (Exception ex)
            {
                
            }
        
            return null;
        }

        public bool IsPasswordAtPosition(Point point)
        {
            try
            {
                return AutomationElement.FromPoint(new Point(point.X, point.Y))?.Current.IsPassword ?? false;
            }
            catch (Exception exception)
            {
                
            }
            return false;
        }
    }
}


