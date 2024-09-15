using System.Collections.Generic;
using UnityEngine;

namespace SuisApiExtension.API
{
	public class APIExecutorsExtended : MonoBehaviour
	{
		public static APIExecutorsExtended Instance { get; private set; }

		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(this);
			}
		}


		public static Dictionary<string, IAPIRequestCustomExecutor> CustomAPIExecutors = new Dictionary<string, IAPIRequestCustomExecutor>();
		public static bool RegisterCustomExecutor<T>(IAPIRequestCustomExecutor executor, bool reparentToAPI)
		{
			var name = typeof(T).Name;
			if (!CustomAPIExecutors.ContainsKey(name))
			{
				Plugin.LogMessage($"Registered executor  to register executor {name}");

				CustomAPIExecutors.Add(name, executor);
				if (reparentToAPI)
				{
					executor.transform.SetParent(APIExecutorsExtended.Instance.transform, true);
					executor.transform.localPosition = Vector3.zero;
					executor.transform.localRotation = Quaternion.identity;
					executor.transform.localScale = Vector3.one;
				}

				return true;
			}
			else
			{
				Plugin.LogMessage($"Trying to register executor {name}");
				return false;
			}
		}




		public static bool UnregisterCustomExecutor(string name) => CustomAPIExecutors.Remove(name);

		public bool ProcessRequest()
		{
			return false;
		}
	}
}
