using Newtonsoft.Json.Linq;
using System;

namespace SuisApiExtension.API
{
	[Serializable]
	public class APICustomMessage
	{
		[NonSerialized] public string websocketSessionID;
		[NonSerialized] public AuthenticatedSession sessionAuthInfo;

		public string apiName;
		public string apiVersion;
		public long timestamp;
		public string messageType;
		public string requestID;
		public JObject data;
	}
}
