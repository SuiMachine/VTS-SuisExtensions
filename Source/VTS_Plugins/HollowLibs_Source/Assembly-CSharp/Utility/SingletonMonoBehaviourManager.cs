using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoBehaviourManager : SingletonMonoBehaviour<SingletonMonoBehaviourManager>
{
	private const string DEBUG_TAG = "[SingletonMonoBehaviourManager]";

	[SerializeField] private List<SingletonMonoBehaviourEntry> Singletons = new List<SingletonMonoBehaviourEntry>();

	private Dictionary<string, SingletonMonoBehaviourEntry> SingletonsDictionary = new Dictionary<string, SingletonMonoBehaviourEntry>();

	private void Awake()
	{
		SingletonMonoBehaviour<SingletonMonoBehaviourManager>.InitializeSingleton(this);
	}

	public void Register<T>(string newSingletonName, T newSingletonInstance) where T : MonoBehaviour
	{
		bool flag = false;
		bool flag2 = false;
		if (newSingletonInstance != null && newSingletonName != null && !newSingletonName.IsNullOrEmptyOrWhitespace())
		{
			flag2 = this.SingletonsDictionary.ContainsKey(newSingletonName);
			if (!flag2)
			{
				SingletonMonoBehaviourEntry singletonMonoBehaviourEntry = new SingletonMonoBehaviourEntry(newSingletonName, newSingletonInstance);
				this.SingletonsDictionary.Add(newSingletonName, singletonMonoBehaviourEntry);
				this.Singletons.Add(singletonMonoBehaviourEntry);
				flag = true;
			}
		}
		if (!flag)
		{
			if (newSingletonName == null || newSingletonInstance == null)
			{
				Debug.LogError("[SingletonMonoBehaviourManager] Failed to add singleton: name or instance is null.");
				return;
			}
			if (flag2)
			{
				Debug.LogError("[SingletonMonoBehaviourManager] Tried to add singleton \"" + newSingletonName + "\" but it was already added.");
				return;
			}
			Debug.LogError("[SingletonMonoBehaviourManager] Tried to add singleton \"" + newSingletonName + "\" but failed.");
		}
	}
}
