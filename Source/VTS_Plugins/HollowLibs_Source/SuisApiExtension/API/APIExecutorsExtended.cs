using System.Collections.Generic;
using UnityEngine;

namespace SuisApiExtension.API
{
	public class APIExecutorsExtended : MonoBehaviour
	{
		public static Dictionary<string, IAPIRequestCustomExecutor> CustomAPIExecutors = new Dictionary<string, IAPIRequestCustomExecutor>();
		public static bool RegisterCustomExecutor<T>(string name, IAPIRequestCustomExecutor executor)
		{
			if (!CustomAPIExecutors.ContainsKey(name))
			{
				VTSPluginExternals.LogMessage($"Registered executor  to register executor {name}");

				CustomAPIExecutors.Add(name, executor);
				return true;
			}
			else
			{
				VTSPluginExternals.LogMessage($"Trying to register executor {name}");
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