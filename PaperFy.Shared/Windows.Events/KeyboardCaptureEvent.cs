using PaperFy.Shared.Capture;

namespace PaperFy.Shared.Windows.Events
{
    public class KeyboardCaptureEvent
	{
		public KeyboardEvent KeyboardEvent { get; }

		public KeyboardCaptureEvent(KeyboardEvent keyboardEvent)
		{
			KeyboardEvent = keyboardEvent;
		}
	}
}
