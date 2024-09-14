using Newtonsoft.Json;
using SuisApiExtension.API;
using System.Threading.Tasks;
using UnityEngine;

namespace SuisApiExtension.Detour
{
	public static class VTubeStudioAPI_Detour
	{
		public static async void sendToSession<T>(APIBaseMessage<T> responseToSend) where T : IAPIMessage
		{
			var text = JsonConvert.SerializeObject(responseToSend);

			await Task.Run(delegate
			{
				sendToSession(responseToSend.websocketSessionID, text, sendAsync: false);
			});
		}

		public static void sendToSession(string sessionID, string responseToSend, bool sendAsync)
		{
			//This has content in normal DLL
			Debug.Log($"Send {sessionID} - {responseToSend}");
		}

		public static void SendCustomError(APICustomMessage payload, ErrorID errorID, string errorMessage)
		{
			//This has content in normal DLL
			Debug.LogError($"Error {errorID} - {errorMessage}");
		}

	}
}
