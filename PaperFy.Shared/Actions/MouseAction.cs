using PaperFy.Shared.Capture;
using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Services;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.Actions
{
    public class MouseAction : DocumenterAction
    {
		public static readonly string[] ScribeApplicationLabels = new string[2] { "Scribe - 1 running window", "Scribe is currently capturing your Desktop" };

		private string targetText;

		[JsonIgnore]
		public string ApplicationBundle { get; private set; }

		[JsonIgnore]
		public string ApplicationName { get; private set; }

		[JsonPropertyName("button")]
		public MouseButton Button { get; set; }

		[JsonPropertyName("clickCount")]
		public uint ClickCount { get; set; } = 1u;

		[JsonPropertyName("target_text")]
		public string TargetText
		{
			get
			{
				return targetText;
			}
			set
			{
				targetText = value;
				base.Description = GenerateDescription();
			}
		}

		public override bool ContainsScribeLabel
		{
			get
			{
				if (!string.IsNullOrEmpty(targetText))
				{
					return ScribeApplicationLabels.Contains(targetText);
				}
				return false;
			}
		}

		public override bool IsKeyboardAction => false;

		public override bool IsMouseAction => true;

		public MouseAction(string targetText, long startTimestamp, long endTimestamp, Point position, MouseButton button, uint count, float? devicePixelRatio, Screenshot screenshot, string applicationName, string applicationBundle)
			: base(ActionKind.MouseClickAction, startTimestamp, endTimestamp, position)
		{
			ClickCount = count;
			Button = button;
			base.DevicePixelRatio = devicePixelRatio;
			base.Screenshot = screenshot;
			ApplicationName = applicationName;
			ApplicationBundle = applicationBundle;
			TargetText = targetText;
		}

		public override string GenerateDescription()
		{
			string text = "here";
			if (!string.IsNullOrEmpty(TargetText))
			{
				text = "\"" + targetText + "\"";
			}
			string text2 = ((ClickCount == 2) ? "Double" : ((ClickCount >= 3) ? "Triple" : string.Empty));
			switch (Button)
			{
				case MouseButton.Left:
					if (!string.IsNullOrEmpty(text2))
					{
						return text2 + "-click " + text;
					}
					return "Click " + text;
				case MouseButton.Right:
					if (!string.IsNullOrEmpty(text2))
					{
						return text2 + "-right click " + text;
					}
					return "Right click " + text;
				default:
					if (!string.IsNullOrEmpty(text2))
					{
						return text2 + "-middle click " + text;
					}
					return "Middle click " + text;
			}
		}

		public static bool IsDoubleClick(DocumenterAction first, DocumenterAction second)
		{
			MouseAction mouseAction = first as MouseAction;
			MouseAction mouseAction2 = second as MouseAction;
			if (mouseAction == null || mouseAction2 == null || mouseAction.Button != mouseAction2.Button)
			{
				return false;
			}
			if (Point.Distance(mouseAction.Position, mouseAction2.Position) > SystemService.Instance.MaximumDoubleClickDistance)
			{
				return false;
			}
			return second.EndTimestamp - first.EndTimestamp <= SystemService.Instance.MaximumDoubleClickTimeout.Milliseconds;
		}

		public static MouseAction Merge(MouseAction first, MouseAction second, string targetText = "")
		{
			return new MouseAction(targetText, first.StartTimestamp, second.EndTimestamp, second.Position, second.Button, Math.Min(first.ClickCount + second.ClickCount, 3u), second.DevicePixelRatio, first.Screenshot, second.ApplicationName ?? first.ApplicationName, second.ApplicationBundle ?? first.ApplicationBundle);
		}
	}
}
