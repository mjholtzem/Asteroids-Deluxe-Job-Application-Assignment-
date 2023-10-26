using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Just a basic singleton class taken from this repo https://github.com/UnityCommunity/UnitySingleton/blob/master/Assets/Scripts/Singleton.cs
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Component
{

	#region Fields

	/// <summary>
	/// The instance.
	/// </summary>
	private static T instance;

	#endregion

	#region Properties

	/// <summary>
	/// Gets the instance.
	/// </summary>
	/// <value>The instance.</value>
	public static T Instance
	{
		get
		{
			if(instance == null)
			{
				instance = FindObjectOfType<T>();
				if(instance == null)
				{
					GameObject obj = new GameObject();
					obj.name = typeof(T).Name;
					instance = obj.AddComponent<T>();
				}
			}
			return instance;
		}
	}

	#endregion

	#region Methods

	/// <summary>
	/// Use this for initialization.
	/// </summary>
	protected virtual void Awake()
	{
		if(instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
		else if(instance != this as T)
		{
			Destroy(gameObject);
		}
	}

	#endregion

}