using PaperFy.Shared.Capture;
using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Services;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PaperFy.Shared.Actions
{
    public class KeyboardSequenceAction : KeyboardAction
	{
		private const string EmptyPattern = "^\\s*(Type)?\\s*$";

		[JsonIgnore]
		internal List<KeyboardEvent> KeyboardEvents { get; } = new List<KeyboardEvent>();

		[JsonIgnore]
		public bool IsBackspaceAllowed => (double)KeyboardEvents.Count((KeyboardEvent e) => e.IsBackspace) < (double)KeyboardEvents.Count / 2.0;

		[JsonIgnore]
		public bool IsEmpty
		{
			get
			{
				if (!string.IsNullOrEmpty(base.Description))
				{
					return Regex.IsMatch(base.Description, "^\\s*(Type)?\\s*$");
				}
				return true;
			}
		}

		public KeyboardSequenceAction(KeyboardEvent keyboardEvent, long startTimestamp, Point position)
			: base(ActionKind.KeyboardSequenceAction, startTimestamp, keyboardEvent.Timestamp, position)
		{
			KeyboardEvents.Add(keyboardEvent);
			base.Description = "Type " + keyboardEvent.Character;
		}

		public KeyboardSequenceAction(List<KeyboardEvent> keyboardEvents)
			: base(ActionKind.KeyboardSequenceAction, 0L, 0L, Point.Empty)
		{
			if (keyboardEvents != null && keyboardEvents.Count > 0)
			{
				KeyboardEvents.AddRange(keyboardEvents);
				base.StartTimestamp = KeyboardEvents.First().Timestamp;
				base.EndTimestamp = KeyboardEvents.Last().Timestamp;
			}
			base.Description = GenerateDescription();
		}

		public override string GenerateDescription()
		{
			if (KeyboardEvents.Count > 0)
			{
				string characters = string.Empty;
				List<string> events = new List<string>();
				KeyboardEvents.ForEach(delegate (KeyboardEvent e)
				{
					if (e.IsSpecialCharacter)
					{
						events.Add(characters);
						characters = string.Empty;
						if (e.IsSpace)
						{
							events.Add(" ");
						}
						else
						{
							events.Add("[[" + SystemService.Instance.GetSpecialKeyDescription(e.SpecialKey.Value) + "]]");
						}
					}
					else
					{
						characters += e.Character;
					}
				});
				if (!string.IsNullOrEmpty(characters))
				{
					events.Add(characters);
				}
				return "Type " + string.Join(" ", events.Where((string e) => !string.IsNullOrEmpty(e)));
			}
			return string.Empty;
		}

		public bool IsMergeable(DocumenterAction next)
		{
			if ((KeyboardEvents.Count > 0 && KeyboardEvents.Last().IsEnter) || next.IsMouseAction)
			{
				return false;
			}
			if (next.Kind == ActionKind.KeyboardSequenceAction)
			{
				return true;
			}
			if (next.Kind == ActionKind.KeyboardCombinationAction)
			{
				KeyboardCombinationAction keyboardCombinationAction = (KeyboardCombinationAction)next;
				if (keyboardCombinationAction.IsEnter || keyboardCombinationAction.IsSpace || (keyboardCombinationAction.IsBackspace && IsBackspaceAllowed))
				{
					return true;
				}
			}
			return false;
		}
    }
}
