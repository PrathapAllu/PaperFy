using PaperFy.Shared.Capture;

namespace PaperFy.Shared.Windows.Events
{
    public class MouseCaptureEvent
	{
		public MouseEvent MouseEvent { get; }

		public MouseCaptureEvent(MouseEvent mouseEvent)
		{
			MouseEvent = mouseEvent;
		}
	}
}
