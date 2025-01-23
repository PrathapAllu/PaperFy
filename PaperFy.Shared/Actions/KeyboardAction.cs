using PaperFy.Shared.Capture;
using PaperFy.Shared.Windows.Models;

namespace PaperFy.Shared.Actions
{
	public abstract class KeyboardAction : DocumenterAction
	{
		public override bool ContainsScribeLabel => false;

		public override bool IsKeyboardAction => true;

		public override bool IsMouseAction => false;

		protected KeyboardAction(ActionKind kind, long startTimestamp, long endTimestamp, Point position)
			: base(kind, startTimestamp, endTimestamp, position)
		{
		}

		public static KeyboardSequenceAction MergeSequence(KeyboardSequenceAction first, KeyboardAction second)
		{
			List<KeyboardEvent> list = new List<KeyboardEvent>(first.KeyboardEvents);
			if (second.Kind == ActionKind.KeyboardCombinationAction && second is KeyboardCombinationAction keyboardCombinationAction)
			{
				if (keyboardCombinationAction.IsBackspace && list.Count > 0)
				{
					list.RemoveAt(list.Count - 1);
				}
				else
				{
					list.Add(keyboardCombinationAction.KeyboardEvent);
				}
			}
			else
			{
				list.AddRange((second as KeyboardSequenceAction).KeyboardEvents);
			}
			return new KeyboardSequenceAction(list)
			{
				StartTimestamp = first.StartTimestamp,
				EndTimestamp = second.EndTimestamp,
				Screenshot = second.Screenshot,
				Position = second.Position
			};
		}
	}
}
