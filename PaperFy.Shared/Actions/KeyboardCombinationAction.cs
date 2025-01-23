using PaperFy.Shared.Capture;
using PaperFy.Shared.Windows.Models;
using PaperFy.Shared.Windows.Services;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.Actions
{
    public class KeyboardCombinationAction : KeyboardAction
	{
		[JsonIgnore]
		public const int FuzzyTimestampEqualityDistance = 100;

		[JsonIgnore]
		public bool IsCombined { get; private set; }

		[JsonIgnore]
		public KeyboardEvent KeyboardEvent { get; set; }

		[JsonIgnore]
		public bool HasNonShiftModifiers
		{
			get
			{
				if (!KeyboardEvent.Modifiers.Alt)
				{
					return KeyboardEvent.Modifiers.Control;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsBackspace => KeyboardEvent.IsBackspace;

		[JsonIgnore]
		public bool IsEnter => KeyboardEvent.IsEnter;

		[JsonIgnore]
		public bool IsSpace => KeyboardEvent.IsSpace;

		public KeyboardCombinationAction(KeyboardEvent keyboardEvent, long startTimestamp, Point position)
			: base(ActionKind.KeyboardCombinationAction, startTimestamp, keyboardEvent.Timestamp, position)
		{
			KeyboardEvent = keyboardEvent;
			base.Description = GenerateDescription();
		}

		public override string GenerateDescription()
		{
			_ = base.EndTimestamp;
			_ = base.StartTimestamp;
			string text = "Press";
			if (IsCombined && base.EndTimestamp - base.StartTimestamp > SystemService.Instance.MaximumDoubleClickTimeout.Milliseconds)
			{
				text = "Press and hold";
			}
			string value = string.Join(" + ", KeyboardEvent.Modifiers.AsStrings.Select((string i) => "[[" + i + "]]").ToList());
			if (string.IsNullOrEmpty(value))
			{
				return text + " [[" + (KeyboardEvent.IsSpecialCharacter ? SystemService.Instance.GetSpecialKeyDescription(KeyboardEvent.SpecialKey.Value) : KeyboardEvent.Character) + "]]";
			}
			if (!KeyboardEvent.IsSpecialCharacter)
			{
				return $"{text} {value} + [[{KeyboardEvent.AsText}]]";
			}
			return $"Press {value} + [[{SystemService.Instance.GetSpecialKeyDescription(KeyboardEvent.SpecialKey.Value)}]]";
		}

		public bool IsMergeable(DocumenterAction next)
		{
			if (next.Kind != ActionKind.KeyboardCombinationAction)
			{
				return false;
			}
			KeyboardCombinationAction keyboardCombinationAction = next as KeyboardCombinationAction;
			if (KeyboardEvent.AreEqualWithoutTime(KeyboardEvent, keyboardCombinationAction.KeyboardEvent))
			{
				return Math.Abs(base.EndTimestamp - keyboardCombinationAction.EndTimestamp) < 100;
			}
			return false;
		}

		public void CombineWith(KeyboardCombinationAction action)
		{
			base.EndTimestamp = action.EndTimestamp;
			IsCombined = true;
			base.Description = GenerateDescription();
		}
	}
}
