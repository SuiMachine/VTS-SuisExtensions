using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityMainThreadDispatcher : MonoBehaviour
{
	private static readonly Queue<Action> _executionQueue = new Queue<Action>();
	private static UnityMainThreadDispatcher _instance = null;

	public static UnityMainThreadDispatcher Instance()
	{
		if (!UnityMainThreadDispatcher.Exists())
		{
			throw new Exception("[UnityMainThreadDispatcher] Could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
		}
		return UnityMainThreadDispatcher._instance;
	}

	public static bool Exists() => UnityMainThreadDispatcher._instance != null;

	public void Enqueue(IEnumerator action)
	{
		Queue<Action> executionQueue = UnityMainThreadDispatcher._executionQueue;
		lock (executionQueue)
		{
			UnityMainThreadDispatcher._executionQueue.Enqueue(delegate
			{
				this.StartCoroutine(action);
			});
		}
	}

	public void Enqueue(Action action)
	{
		this.Enqueue(this.ActionWrapper(action));
	}

	private IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
		yield break;
	}
}