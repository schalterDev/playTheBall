using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook : LobbyHook {

	public static int numberPlayers;
	private string name;


	public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) {
		MenuSound.stopMusic ();

		LobbyPlayer lobby = lobbyPlayer.GetComponent<LobbyPlayer> ();
		PlayerInfoController playerInfo = gamePlayer.GetComponent<PlayerInfoController> ();

		playerInfo.playerName = lobby.playerName;

		numberPlayers = manager.numPlayers;
	}

}
