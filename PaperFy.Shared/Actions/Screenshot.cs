using System.Text.Json.Serialization;

namespace PaperFy.Shared.Actions
{
	public class Screenshot
	{
		[JsonPropertyName("screenshot_upload_status")]
		public ScreenshotUploadStatus? UploadStatus { get; set; }

		[JsonPropertyName("screenshot_url")]
		public string Url { get; set; }

		public Screenshot(string url, ScreenshotUploadStatus uploadStatus)
		{
			Url = url;
			UploadStatus = uploadStatus;
		}
	}
}
