using UnityEngine;
using Mirror;
using System;
using UnityEditor;
using UnityEngine.SceneManagement;
// using Steamworks;
using Unity.VisualScripting.FullSerializer;

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
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private LobbyManager lobMan;

    #region SERVER

    public override void OnServerConnect(NetworkConnectionToClient conn) {
        DebugManager.current.NewDebug("[SERVER] New client connected...");

        if(numPlayers >= maxConnections) {
            DebugManager.current.NewDebug("[SERVER] Maximum player count reached!");
            conn.Disconnect();
            return;
        }

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

        if(SceneManager.GetActiveScene().name == menuScene) {
            // NetworkRoomPlayer roomPlayer = Instantiate(roomPlayerPF);
            // NetworkServer.AddPlayerForConnection(conn, roomPlayer.gameObject);
        }

        base.OnServerAddPlayer(conn);
    }

    #endregion SERVER

    //============================================================

    #region CLIENT

    public override void OnClientConnect() {
        DebugManager.current.NewDebug("[CLIENT] Client connect...");
        base.OnClientConnect();
        DebugManager.current.NewDebug("[CLIENT] Client connected?");

        OnClientConnected?.Invoke();

        // lobMan.SendPlayerName();
    }

    public override void OnClientDisconnect() {
        DebugManager.current.NewDebug("[CLIENT] Client disconnect...");
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();

        mainMenu.SetActive(true);
        panelLobby.SetActive(false);
    }

    #endregion CLIENT

    //============================================================

    #region GENERAL

    public override void OnStartHost() {
        DebugManager.current.NewDebug("Lobby hosted...");

        base.OnStartHost();
    }

    public override void OnStartClient() {
        DebugManager.current.NewDebug("Starting client...");

        base.OnStartClient();

        mainMenu.SetActive(false);
        panelLobby.SetActive(true);
        
        // 
    }

    #endregion GENERAL
}
