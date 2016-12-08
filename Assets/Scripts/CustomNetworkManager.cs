using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class CustomNetworkManager : NetworkManager
{
	public override void OnServerConnect(NetworkConnection connection)
	{
		// Voilà
		Debug.Log("OnServerConnect");
	}


}
	