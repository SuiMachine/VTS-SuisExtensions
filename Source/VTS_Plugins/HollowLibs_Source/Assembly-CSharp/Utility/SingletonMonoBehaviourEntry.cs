using System;
using UnityEngine;

[Serializable]
public class SingletonMonoBehaviourEntry
{
	public SingletonMonoBehaviourEntry(string singletonName, MonoBehaviour singletonInstance)
	{
		this.SingletonName = singletonName;
		this.SingletonEntry = singletonInstance;
	}

	public string SingletonName;
	public MonoBehaviour SingletonEntry;
}
