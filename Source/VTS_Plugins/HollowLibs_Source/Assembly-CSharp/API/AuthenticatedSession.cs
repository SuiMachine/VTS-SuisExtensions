using System;

[Serializable]
public class AuthenticatedSession
{
	public AuthenticatedSession()
	{
	}

	public AuthenticatedSession(string sessionID, string authToken, string pluginName, string developerName, string pluginID)
	{
		this.sessionID = sessionID;
		this.authToken = authToken;
		this.pluginName = pluginName;
		this.developerName = developerName;
		this.pluginID = pluginID;
	}

	public string pluginID;
	public string sessionID;
	public string authToken;
	public string pluginName;
	public string developerName;
	public string pluginOrigin;
	public bool isAuthenticated;
	public bool requiresAuthentication;
}
