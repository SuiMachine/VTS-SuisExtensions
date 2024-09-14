using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance()
	{
		if (SingletonMonoBehaviour<T>._instance != null)
		{
			return SingletonMonoBehaviour<T>._instance;
		}
		Debug.LogError("[SingletonMonoBehaviour] Tried to access uninitialized singleton object for class \"" + typeof(T).FullName + "\".");
		return default(T);
	}

	public static string Name()
	{
		return typeof(T).FullName;
	}

	public static void InitializeSingleton(T newInstance = default(T))
	{
		if (!(newInstance != null))
		{
			Debug.LogError("[SingletonMonoBehaviour] Tried to initialize singleton object for class \"" + typeof(T).FullName + "\" with NULL instance.");
			return;
		}
		if (SingletonMonoBehaviour<T>._instance == null)
		{
			SingletonMonoBehaviour<T>._instance = newInstance;
			SingletonMonoBehaviour<SingletonMonoBehaviourManager>.Instance().Register<T>(typeof(T).FullName, SingletonMonoBehaviour<T>._instance);
			return;
		}
		Debug.LogError("[SingletonMonoBehaviour] Tried to re-initialize already initialized singleton object for class \"" + typeof(T).FullName + "\".");
	}

	private const string DEBUG_TAG = "[SingletonMonoBehaviour]";

	private static T _instance;
}
