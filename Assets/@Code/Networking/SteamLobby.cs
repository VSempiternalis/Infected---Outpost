using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;

public class SteamLobby : MonoBehaviour {
    private CustomNetworkManager netman;
    // [SerializeField] private GameObject hostButton = null;
    [SerializeField] private GameObject panelLobby;
    [SerializeField] private GameObject mainMenu;

    //LOBBY
    [SerializeField] private Transform onlineList;
    [SerializeField] private GameObject lobbyPlayerPF;

    //CALLBACKS (Functions called when something happens on Steam)
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private const string hostAddressKey = "HostAddress";

    private void Start() {
        netman = GetComponent<CustomNetworkManager>();

        if(Application.isEditor) {
            DebugManager.current.NewDebug("[UNITY EDITOR] Running in Unity Editor. Steam-related code will not execute.");
            return;
        }

        if(SteamManager.Initialized) {
            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        } 
        // else {
        //     DebugManager.current.NewDebug("[UNITY EDITOR] Running in Unity Editor. Steam-related code will not execute.");
        //     return;
        // }
    }
    
    //When main menu host button pressed
    public void HostLobby() {
        DebugManager.current.NewDebug("[STEAM] Checking Steam...");
        // hostButton.SetActive(false);

        if(SteamManager.Initialized) {
            DebugManager.current.NewDebug("[STEAM] Steam active. Creating lobby...");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, netman.maxConnections);
        } else {
            DebugManager.current.NewDebug("[STEAM] Steam manager inactive. Returning...");
        }
    }

    private void OnLobbyCreated(LobbyCreated_t callback) {
        //Error check
        if(callback.m_eResult != EResult.k_EResultOK) {
            DebugManager.current.NewDebug("[STEAM] Error while creating lobby. Returning...");
            return;
        }

        DebugManager.current.NewDebug("[STEAM] Lobby created!");

        netman.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());

        // mainMenu.SetActive(false);
        // panelLobby.SetActive(true);
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback) {
        DebugManager.current.NewDebug("[STEAM] Joining Lobby...");
        
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback) {
        DebugManager.current.NewDebug("[STEAM] Entered Lobby...");

        //Deactivate main menu and activate lobby ui
        // mainMenu.SetActive(false);
        // panelLobby.SetActive(true);

        // //Add lobby player to online list
        // GameObject newLobbyPlayer =  Instantiate(lobbyPlayerPF, onlineList);
        // newLobbyPlayer.transform.GetChild(0).GetComponent<TMP_Text>().text = "Name not found";
        
        // //Get Steam name if available
        // if(!string.IsNullOrEmpty(SteamFriends.GetPersonaName())) newLobbyPlayer.transform.GetChild(0).GetComponent<TMP_Text>().text = SteamFriends.GetPersonaName();
        
        // newLobbyPlayer.SetActive(true);

        //====================

        //Check if network server is already active
        if(NetworkServer.active) {
            DebugManager.current.NewDebug("[STEAM] Server already active. Returning...");
            return;
        }

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), hostAddressKey);

        netman.networkAddress = hostAddress;
        netman.StartClient();

        // hostButton.SetActive(false);
    }
}
