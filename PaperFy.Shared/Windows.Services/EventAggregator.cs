namespace PaperFy.Shared.Windows.Services
{
    public class EventAggregator
	{
		private static readonly object SyncObject = new object();

		public static EventAggregator Instance { get; } = new EventAggregator();

		private Dictionary<Type, List<object>> Listeners { get; } = new Dictionary<Type, List<object>>();

		private EventAggregator()
		{
		}

		public void Clear()
		{
			lock (SyncObject)
			{
				Listeners.Clear();
			}
		}

		public void Publish<T>(T args)
		{
			bool flag = false;
			List<object> value = null;
			lock (SyncObject)
			{
				flag = Listeners.TryGetValue(typeof(T), out value);
			}
			if (flag)
			{
				value?.ForEach(delegate (object i)
				{
					(i as Action<T>)?.Invoke(args);
				});
			}
		}

		public void Subscribe<T>(Action<T> callback)
		{
			lock (SyncObject)
			{
				if (!Listeners.ContainsKey(typeof(T)))
				{
					Listeners.Add(typeof(T), new List<object>());
				}
				Listeners[typeof(T)].Add(callback);
			}
		}

		public void Unsubscribe<T>(Action<T> callback)
		{
			lock (SyncObject)
			{
				if (Listeners.TryGetValue(typeof(T), out var value))
				{
					value.Remove(callback);
				}
			}
		}
	}
}
