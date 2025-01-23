namespace Paperfy.Models
{
    public class LocalSettings
    {

        public bool IsVoiceRecordingOn { get; set; }

        public static LocalSettings Instance { get; private set; } = new LocalSettings();

        public string markerColor { get; set; }

        public bool IsAppDocumenting { get; set; } = false;
    }
}
