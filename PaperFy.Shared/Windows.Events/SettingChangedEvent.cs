namespace PaperFy.Shared.Windows.Events
{
	public class SettingChangedEvent
	{
		public string Name { get; }

		public object Value { get; }

		public SettingChangedEvent(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
}
