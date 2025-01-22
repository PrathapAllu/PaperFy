namespace Paperfy.Models
{
    public class GlobalSettings
    {
        public static GlobalSettings Instance { get; private set; } = new GlobalSettings();
    }
}
