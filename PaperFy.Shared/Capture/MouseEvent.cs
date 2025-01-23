namespace PaperFy.Shared.Capture
{
    public class MouseEvent
    {
        public string ApplicationBundle { get; }

        public string ApplicationName { get; }

        public MouseButton Button { get; }

        public long Timestamp { get; }

        public int X { get; }

        public int Y { get; }

        public MouseEvent(int x, int y, MouseButton button, string applicationName, string applicationBundle, long timestamp)
        {
            X = x;
            Y = y;
            Button = button;
            ApplicationName = applicationName;
            ApplicationBundle = applicationBundle;
            Timestamp = timestamp;
        }
    }
}

