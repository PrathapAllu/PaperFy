using System.Collections.Generic;

namespace PaperFy.Shared.Capture
{
	public class KeyboardEventModifiers
	{
		public bool Alt { get; }

		public bool Control { get; }

		public bool Shift { get; }

		public bool Windows { get; }

		public IEnumerable<string> AsStrings
		{
			get
			{
				List<string> list = new List<string>();
				if (Alt)
				{
					list.Add("Alt");
				}
				if (Shift)
				{
					list.Add("Shift");
				}
				if (Control)
				{
					list.Add("Ctrl");
				}
				return list;
			}
		}

		public KeyboardEventModifiers(bool alt, bool control, bool shift, bool windows)
		{
			Alt = alt;
			Control = control;
			Shift = shift;
			Windows = windows;
		}
	}
}
