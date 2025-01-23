using PaperFy.Shared.Windows.Models;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.Actions
{
	public abstract class DocumenterAction
	{
		[JsonPropertyName("description")]
		public string Description { get; set; }

		[JsonPropertyName("devicePixelRatio")]
		public float? DevicePixelRatio { get; set; }

		[JsonPropertyName("end_timestamp")]
		public long EndTimestamp { get; set; }

		[JsonPropertyName("kind")]
		public ActionKind Kind { get; set; }

		[JsonPropertyName("position")]
		public Point Position { get; set; }

		[JsonIgnore]
		public Screenshot Screenshot { get; set; }

		[JsonPropertyName("start_timestamp")]
		public long StartTimestamp { get; set; }

		[JsonIgnore]
		public bool IsUploadWaiting
		{
			get
			{
				Screenshot screenshot = Screenshot;
				if (screenshot == null)
				{
					return false;
				}
				return screenshot.UploadStatus == ScreenshotUploadStatus.Waiting;
			}
		}

		[JsonPropertyName("screenshot_upload_status")]
		public ScreenshotUploadStatus? UploadStatus => Screenshot?.UploadStatus;

		[JsonPropertyName("screenshot_url")]
		public string Url => Screenshot?.Url;

		[JsonIgnore]
		public abstract bool ContainsScribeLabel { get; }

		[JsonIgnore]
		public abstract bool IsKeyboardAction { get; }

		[JsonIgnore]
		public abstract bool IsMouseAction { get; }

		protected DocumenterAction(ActionKind kind, long startTimestamp, long endTimestamp, Point position)
		{
			Kind = kind;
			StartTimestamp = startTimestamp;
			EndTimestamp = endTimestamp;
			Position = position;
		}

		public abstract string GenerateDescription();

		public static bool CanMergeIntoSequence(DocumenterAction first, DocumenterAction second)
		{
			if (first.Kind != ActionKind.KeyboardSequenceAction || second.IsMouseAction)
			{
				return false;
			}
			return ((KeyboardSequenceAction)first).IsMergeable(second);
		}
	}
}
