using UnityEngine;
using Mirror;
using System;
// using UnityEditor;
using UnityEngine.SceneManagement;
// using Steamworks;
using System.Collections.Generic;

public class CustomNetworkManager : NetworkManager {
    [Scene] [SerializeField] private string menuScene = string.Empty;

    // [Space(5)]
    // [Header("ROOM")]
    // [SerializeField] private NetworkRoomPlayer roomPlayerPF = null;

    public event Action OnClientConnected;
    public event Action OnClientDisconnected;

    //...
    
    [Space(5)]
    [Header("LOBBY")]
    [SerializeField] private GameObject panelLobby;
    [SerializeField] private GameObject panelLogin;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private LobbyManager lobMan;
    public int lobbyPlayerCount = 0;

    #region SERVER

    public override void OnServerConnect(NetworkConnectionToClient conn) {
        DebugManager.current.NewDebug("[SERVER] New client connected...");

        if(numPlayers >= maxConnections) {
            DebugManager.current.NewDebug("[SERVER] Maximum player count reached!");
            conn.Disconnect();
            return;
        }

        // AddNewLobbyPlayer();

        // if(SceneManager.GetActiveScene().name != menuScene) {
        //     DebugManager.current.NewDebug("[SERVER] Scene error!");
        //     //Replace ff with player joining as spectator. I guess
        //     conn.Disconnect();
        //     return;
        // }

        base.OnServerConnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        DebugManager.current.NewDebug("[SERVER] Client requesting addition...");
        DebugManager.current.NewDebug("[SERVER] Player count: " + (lobbyPlayerCount + 1));

        lobbyPlayerCount ++;

        // AddNewLobbyPlayer();

        //Add lobbyPlayer instance 
        // lobMan.SendPlayerName(lobbyPlayerCount.ToString());

        // if(SceneManager.GetActiveScene().name == menuScene) {
        //     // NetworkRoomPlayer roomPlayer = Instantiate(roomPlayerPF);
        //     // NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
        // }

        base.OnServerAddPlayer(conn);
    }

    private void AddNewLobbyPlayer() {
        // lobMan.SendPlayerName(conn.connectionId.ToString());
        
        DebugManager.current.NewDebug("[SERVER] Relisting players: " + NetworkServer.connections.Count);
        //Clear online list
        lobMan.ClearOnlineList();

        // Get a list of all connection IDs for connected clients, and spawn their lobbyPlayer instances
        foreach(KeyValuePair<int, NetworkConnectionToClient> kvp in NetworkServer.connections) {
            int connID = kvp.Key;
            NetworkConnectionToClient connection = kvp.Value;

            // Add new lobbyPlayer instances
            DebugManager.current.NewDebug("[SERVER] Recreating Lobby Player: " + connection.connectionId);
            lobMan.SendPlayerName(connID.ToString());
            Debug.Log("Connected Player: Connection ID = " + connID + ", Address = " + connection.address);
        }
    }
    
    #endregion SERVER

    //============================================================

    #region CLIENT

    public override void OnClientConnect() {
        DebugManager.current.NewDebug("[CLIENT] Client connect...");
        base.OnClientConnect();

        OnClientConnected?.Invoke();
        
        // AddNewLobbyPlayer(); 

        // lobMan.SendPlayerName();
    }

    public override void OnClientDisconnect() {
        DebugManager.current.NewDebug("[CLIENT] Client disconnect...");
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();

        // mainMenu.SetActive(true);
        // panelLobby.SetActive(false);
        ToggleLobby(false);
    }

    #endregion CLIENT

    //============================================================

    #region GENERAL

    public override void OnStartHost() {
        DebugManager.current.NewDebug("Lobby hosted...");

        base.OnStartHost();

        NetworkManager.singleton.StartServer();
    }

    public override void OnStartClient() {
        DebugManager.current.NewDebug("Starting client...");

        base.OnStartClient();

        // mainMenu.SetActive(false);
        // panelLobby.SetActive(true);
        ToggleLobby(true);
    }

    private void ToggleLobby(bool isOn) {
        panelLobby.SetActive(isOn);
        mainMenu.SetActive(!isOn);
        panelLogin.SetActive(!isOn);
    }

    #endregion GENERAL
}
