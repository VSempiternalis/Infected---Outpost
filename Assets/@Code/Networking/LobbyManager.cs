using UnityEngine;
using Mirror;
// using Steamworks;
using TMPro;

public class LobbyManager : NetworkBehaviour {
    [SerializeField] private Transform onlineList;
    [SerializeField] private GameObject lobbyPlayerPF;

    #region SERVER

    [Command] private void CmdSendPlayerName(string playerName) {
        print("in cmd");
        DebugManager.current.NewDebug("[CMD] CmdSendPlayerName...");
        RpcSetPlayerName(playerName);
    }

    #endregion SERVER

    //========================================

    #region CLIENT

    [ClientRpc] private void RpcSetPlayerName(string playerName) {
        DebugManager.current.NewDebug("[RPC] RpcSetPlayerName: " + playerName);
        CreateLobbyPlayer(playerName);
    }

    [ClientRpc] private void RpcClearOnlineList() {
        foreach(Transform child in onlineList) {
            Destroy(child);
        }
    }

    #endregion CLIENT

    //========================================

    #region GENERAL

    private void Start() {
        DebugManager.current.NewDebug("Starting Lobby Manager...");

        // onlineList = ;
    }

    //Only called when joins a lobby
    public override void OnStartLocalPlayer() {
        DebugManager.current.NewDebug("[LOCAL] Starting...");
        base.OnStartLocalPlayer();

        onlineList = GameObject.Find("IMG - Online").transform;
        
        // lobbyPlayerPF = GameObject.Find("LobbyPlayer");

        // Check if this script is running on the local player
        // if(isLocalPlayer) {
        //     CmdSendPlayerName(GetPlayerName());
        // }
    }

    public void ClearOnlineList() {
        RpcClearOnlineList();
    }

    public void SendPlayerName(string playerName) {
        DebugManager.current.NewDebug("[LM] Sending Player Name...");
        //Set and get actual player name later

        // RpcSetPlayerName(playerName);
        print("Trying to cmd");
        CmdSendPlayerName(playerName);
        print("Attempted cmd");
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

        print("PLAYER COUNT: " + GameObject.FindObjectOfType<CustomNetworkManager>().lobbyPlayerCount);
        playerName = (GameObject.FindObjectOfType<CustomNetworkManager>().numPlayers).ToString();

        return playerName;
    }

    #endregion GENERAL
}
