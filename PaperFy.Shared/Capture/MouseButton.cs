using System.Text.Json.Serialization;

namespace PaperFy.Shared.Capture
{
    public enum MouseButton
    {
        [JsonPropertyName("left")]
        Left,
        [JsonPropertyName("right")]
        Right,
        [JsonPropertyName("middle")]
        Middle
    }
}


