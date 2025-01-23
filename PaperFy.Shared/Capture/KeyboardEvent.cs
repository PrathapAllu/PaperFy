using Avalonia.Input;
using PaperFy.Shared.Windows.Services;

namespace PaperFy.Shared.Capture
{
    public class KeyboardEvent
	{
		public string Character { get; internal set; }

		public int Code { get; internal set; }

		public KeyboardEventModifiers Modifiers { get; internal set; }

		public int? SpecialKey { get; internal set; }

		public long Timestamp { get; internal set; }

		public string AsText => GetCodeAsText();

		public bool IsCombination
		{
			get
			{
				if (!IsSpecialCharacter && !Modifiers.Alt)
				{
					return Modifiers.Control;
				}
				return true;
			}
		}

		public bool IsBackspace
		{
			get
			{
				if (SpecialKey.HasValue)
				{
					return SystemService.Instance.IsSpecialKeyBackspace(SpecialKey.Value);
				}
				return false;
			}
		}

		public bool IsEnter
		{
			get
			{
				if (SpecialKey.HasValue)
				{
					return SystemService.Instance.IsSpecialKeyEnter(SpecialKey.Value);
				}
				return false;
			}
		}

		public bool IsSpace
		{
			get
			{
				if (SpecialKey.HasValue)
				{
					return SystemService.Instance.IsSpecialKeySpace(SpecialKey.Value);
				}
				return false;
			}
		}

		public bool IsSpecialCharacter => SpecialKey.HasValue;

		public KeyboardEvent(KeyboardEventModifiers modifiers, string character, int code, int? specialKey, long timestamp)
		{
			Modifiers = modifiers;
			Character = character;
			Code = code;
			SpecialKey = specialKey;
			Timestamp = timestamp;
		}

		private string GetCodeAsText()
		{
			if (Code >= 34 && Code <= 43)
			{
				return ((Key)Code/*cast due to .constrained prefix*/).ToString().Substring(1);
			}
			return ((Key)Code/*cast due to .constrained prefix*/).ToString();
		}

		public static bool AreEqualWithoutTime(KeyboardEvent first, KeyboardEvent second)
		{
			if (first == null && second == null)
			{
				return true;
			}
			if (first == null || second == null)
			{
				return false;
			}
			if (first.Modifiers.Alt == second.Modifiers.Alt && first.Modifiers.Shift == second.Modifiers.Shift && first.Modifiers.Control == second.Modifiers.Control && first.Character == second.Character && first.Code == second.Code)
			{
				return first.SpecialKey == second.SpecialKey;
			}
			return false;
		}
	}
}
