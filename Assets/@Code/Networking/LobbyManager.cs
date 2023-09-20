using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using Unity.VisualScripting;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private Transform onlineList;
    [SerializeField] private GameObject lobbyPlayerPF;

    #region SERVER

    [Command] private void CmdSendPlayerName(string playerName) {
        DebugManager.current.NewDebug("[CMD] CmdSendPlayerName...");
        RpcSetPlayerName(playerName);
    }

    #endregion SERVER

    //========================================

    #region CLIENT

    [ClientRpc] private void RpcSetPlayerName(string playerName) {
        DebugManager.current.NewDebug("[RPC] RpcSetPlayerName...");
        CreateLobbyPlayer(playerName);
    }

    #endregion CLIENT

    //========================================

    #region GENERAL

    private void Start() {
        DebugManager.current.NewDebug("Starting...");

        onlineList = GameObject.Find("IMG - Online").transform;
    }

    public override void OnStartLocalPlayer() {
        DebugManager.current.NewDebug("[LOCAL] Starting...");
        base.OnStartLocalPlayer();
        
        // lobbyPlayerPF = GameObject.Find("LobbyPlayer");

        // Check if this script is running on the local player
        if(isLocalPlayer) {
            CmdSendPlayerName(GetPlayerName());
        }
    }

    public void SendPlayerName() {
        CmdSendPlayerName(GetPlayerName());
    }

    private void CreateLobbyPlayer(string playerName) {
        DebugManager.current.NewDebug("Creating lobby player...");
        //Add lobby player to online list
        GameObject newLobbyPlayer =  Instantiate(lobbyPlayerPF, onlineList);
        newLobbyPlayer.transform.GetChild(0).GetComponent<TMP_Text>().text = playerName;
        
        newLobbyPlayer.SetActive(true);
    }

    private string GetPlayerName() {
        string playerName = "Unknown";
        
        // DebugManager.current.NewDebug("Fetching Steam name...");
        // //Get Steam name if available
        // if(SteamManager.Initialized && !string.IsNullOrEmpty(SteamFriends.GetPersonaName())) {
        //     playerName = SteamFriends.GetPersonaName();
        //     DebugManager.current.NewDebug("Steam name found: " + SteamFriends.GetPersonaName());
        // } else {
        //     DebugManager.current.NewDebug("Steam name not found! Player unknown...");
        // }

        print("PLAYER COUNT: " + GameObject.FindObjectOfType<CustomNetworkManager>().numPlayers);
        playerName = (GameObject.FindObjectOfType<CustomNetworkManager>().numPlayers).ToString();

        return playerName;
    }

    #endregion GENERAL
}
