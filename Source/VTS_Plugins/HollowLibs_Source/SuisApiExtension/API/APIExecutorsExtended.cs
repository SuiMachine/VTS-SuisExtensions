using System.Collections.Generic;
using UnityEngine;

namespace SuisApiExtension.API
{
	public class APIExecutorsExtended : MonoBehaviour
	{
		internal static Dictionary<string, IAPIRequestCustomExecutor> CustomAPIExecutors = new Dictionary<string, IAPIRequestCustomExecutor>();
		public static bool RegisterCustomExecutor<T>(IAPIRequestCustomExecutor executor, bool reparentToAPI)
		{
			string name = nameof(T);
			if (!CustomAPIExecutors.ContainsKey(name))
			{
				VTSPluginExternals.LogMessage($"Registered executor  to register executor {name}");

				//There is some code here about reparenting as well in normal DLL
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