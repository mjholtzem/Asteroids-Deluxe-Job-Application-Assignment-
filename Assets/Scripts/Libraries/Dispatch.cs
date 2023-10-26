using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// A simple event dispatching system that uses types instead of "ids"
/// https://gist.github.com/Ayrik/8c41d9c5d7784b12e49bf4dd0baa3d36#file-dispatch-cs
/// </summary>
public static class Dispatch
{
	public interface ITaggedEvent
	{
		string EventTag { get; }
	}

	public struct EventListener
	{
		public int priority;
		public string tag;

		// Used as a handle to remove listener once added
		public MethodInfo method;
		public object target;
	}

	public static Queue<EventListener> _eventPool = new Queue<EventListener>();

	public static Dictionary<Type, LinkedList<EventListener>> _listeners = new Dictionary<Type, LinkedList<EventListener>>();

	/// <summary>
	/// Listen for an event of type T
	/// </summary>
	/// <typeparam name="T">Type of event</typeparam>
	/// <param name="callback">Action to call when event is fired</param>
	public static void Listen<T>(Action<T> callback) => Listen(callback, 0, null);
	public static void Listen<T>(Action<T> callback, int priority) => Listen(callback, priority, null);
	public static void Listen<T>(Action<T> callback, string tag) => Listen(callback, 0, tag);
	public static void Listen<T>(Action<T> callback, int priority, string tag)
	{
		// Create full listener
		EventListener evt = _eventPool.Count == 0 ? new EventListener() : _eventPool.Dequeue();
		evt.method = callback.Method;
		evt.target = callback.Target;
		evt.tag = tag;
		evt.priority = priority;

		// Create listener list if none exists
		Type evtType = typeof(T);
		LinkedList<EventListener> listeners;
		if(!_listeners.TryGetValue(evtType, out listeners))
		{
			listeners = new LinkedList<EventListener>();
			_listeners.Add(evtType, listeners);
		}

		if(priority == 0 || listeners.First == null)
			listeners.AddLast(evt);
		else
		{
			var node = listeners.First;
			for(; node != null; node = node.Next)
			{
				if(priority >= node.Value.priority)
					break;
			}
			listeners.AddBefore(node, evt);
		}
	}

	/// <summary>
	/// Stop listening for event of type T
	/// </summary>
	/// <typeparam name="T">Type of event</typeparam>
	/// <param name="callback">Used as a handle to the listener</param>
	public static void Unlisten<T>(Action<T> callback, string tag = null)
	{
		Type evtType = typeof(T);
		LinkedList<EventListener> listeners;
		if(!_listeners.TryGetValue(evtType, out listeners))
			return;

		for(var node = listeners.First; node != null; node = node.Next)
		{
			if(node.Value.method == callback.Method && node.Value.target == callback.Target && node.Value.tag == tag)
			{
				listeners.Remove(node);
				break;
			}
		}
	}

	private static object[] __fireParams = new object[1]; // Save on garbage

	/// <summary>
	/// Fire an event using a tagged event. This sends the event only to the listeners listening to the specified tag.
	/// Listeners with no tag specified will also get the event.
	/// </summary>
	/// <typeparam name="T">Type of event</typeparam>
	/// <param name="evt">Instance of event</param>
	public static void FireTagged<T>(T evt) where T : ITaggedEvent => Fire(evt, evt.EventTag);

	/// <summary>
	/// Fire an event with an optional tag. If a tag is specified only listeners with that tag, or no tag specified will get it.
	/// </summary>
	/// <typeparam name="T">Type of event</typeparam>
	/// <param name="evt">Instance of event</param>
	/// <param name="tag">Filtering tag</param>
	public static void Fire<T>(T evt, string tag = null)
	{
		LinkedList<EventListener> listeners;
		if(!_listeners.TryGetValue(typeof(T), out listeners))
			return;

		for(var node = listeners.First; node != null; node = node.Next)
		{
			if(node.Value.tag == null || node.Value.tag == tag)
			{
				__fireParams[0] = evt;
				node.Value.method.Invoke(node.Value.target, __fireParams);
			}
		}
	}
}