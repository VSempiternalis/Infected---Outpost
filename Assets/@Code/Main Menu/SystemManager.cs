using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using Steamworks;
using System.Text;

public class SystemManager : MonoBehaviourPunCallbacks {
    public static SystemManager current;

    [SerializeField] private GameObject panelConnecting;
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject panelDebug;
    [SerializeField] private GameObject vlgPanel;
    [SerializeField] private GameObject serverBrowserPanel;
    [SerializeField] private GameObject roomPanel;

    [SerializeField] private TMP_Text connectingText;
    private bool isConnecting;

    private float timer = 0f;
    private float interval = 0.5f;

    [SerializeField] private TMP_InputField inputUsername;

    [SerializeField] private List<GameObject> ActivateOnAwake;
    [SerializeField] private List<GameObject> DeactivateOnAwake;

    [SerializeField] private TMP_Text versionText;

    [SerializeField] private ServerSettings serverSettings;
    [SerializeField] private TMP_Text serverRegion;

    [SerializeField] private int maxCCU;
    [SerializeField] private TMP_Text userCount;

    private ExitGames.Client.Photon.Hashtable myCustomProperties = new ExitGames.Client.Photon.Hashtable();

    [SerializeField] private TMP_Text warning;
    [SerializeField] private TMP_Text playersInGame;
    [SerializeField] private int maxPlayerCount;

    [Space(10)]
    [Header("STEAM AUTHENTICATION")]
    HAuthTicket hAuthTicket;
    string steamAuthSessionTicket; //I assume, the string converted version of the hAuthTicket

    private void Awake() {
        // SetUserCount();

        current = this;

        //Set version text
        versionText.text = "VERSION: v" + Application.version;

        warning.text = maxPlayerCount.ToString();
    }

    private void Start() {
        // print("STEAM MANAGER INIT: " + SteamManager.Initialized);

        //STEAM AUTHENTICATION
        if(SteamManager.Initialized) SteamAuthentication();

        panelConnecting.SetActive(false);
        panelMainMenu.SetActive(true);

        //if player in room
        if(PhotonNetwork.InRoom) {
            print("IS IN ROOM");
            vlgPanel.SetActive(false);
            serverBrowserPanel.SetActive(false);
            roomPanel.SetActive(true);
            LobbyManager.current.UpdatePlayerList();
        }

        //if player in lobby
        else if(PhotonNetwork.InLobby) {
            print("IS IN LOBBY");
            vlgPanel.SetActive(false);
            serverBrowserPanel.SetActive(true);
            roomPanel.SetActive(false);
        }

        //if player connected
        else if(PhotonNetwork.IsConnected) {
            print("IS CONNECTED");
            vlgPanel.SetActive(true);
            serverBrowserPanel.SetActive(false);
            roomPanel.SetActive(false);
        }
        
        else {
            foreach(GameObject thing in DeactivateOnAwake) {
                thing.SetActive(false);
            }

            foreach(GameObject thing in ActivateOnAwake) {
                thing.SetActive(true);
            }
        }
    }

    private void Update() {
        if(isConnecting && Time.time >= timer) {
            UpdateConnectingText();

            timer = Time.time + interval;
        }

        playersInGame.text = "USERS IN REGION: " + PhotonNetwork.CountOfPlayers;// + "/" + maxPlayerCount;
    }

    private void SteamAuthentication() {
        hAuthTicket = new HAuthTicket();

        //Set steamAuthSessionTicket and hAuthTicket
        steamAuthSessionTicket = GetSteamAuthTicket(out hAuthTicket);

        //Debug
        print("H AUTH TICKET: " + hAuthTicket);
        print("STEAM AUTH TICKET: " + steamAuthSessionTicket);

        //Photon Authorization
        PhotonNetwork.AuthValues = new AuthenticationValues();
        PhotonNetwork.AuthValues.UserId = SteamUser.GetSteamID().ToString();
        PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Steam;
        PhotonNetwork.AuthValues.AddAuthParameter("ticket", steamAuthSessionTicket);

        //Display Steam user name
    }

    // hAuthTicket should be saved so you can use it to cancel the ticket as soon as you are done with it
    public string GetSteamAuthTicket(out HAuthTicket hAuthTicket) {
        SteamNetworkingIdentity pSteamNetID = new SteamNetworkingIdentity();

        byte[] ticketByteArray = new byte[1024];
        uint ticketSize;
        hAuthTicket = SteamUser.GetAuthSessionTicket(ticketByteArray, ticketByteArray.Length, out ticketSize, ref pSteamNetID);
        System.Array.Resize(ref ticketByteArray, (int)ticketSize);
        StringBuilder sb = new StringBuilder();
        for(int i=0; i < ticketSize; i++) {
            sb.AppendFormat("{0:x2}", ticketByteArray[i]);
        }
        return sb.ToString();
    }

    private void CancelAuthTicket(HAuthTicket ticket) {
        if(ticket != null) SteamUser.CancelAuthTicket(ticket);  
    }

    public override void OnCustomAuthenticationFailed(string debugMessage) {
        base.OnCustomAuthenticationFailed(debugMessage);

        print("ON CUSTOM AUTH FAILED: " + debugMessage);
    }

    public void SetUserCount() {
        int ccu = PhotonNetwork.CountOfPlayers;
        print("CCU: " + ccu);
        Color color = Color.white;

        userCount.text = "USERS: " + ccu + "/" + maxCCU;
        if(ccu >= maxCCU) color = Color.red;
        else color = Color.green;
        color.a = 0.392f;
    
        userCount.color = color;
    }

    //Offline to server connect
    public void OnClickConnect() {
        print("CONNECTING TO SERVER");
        if(inputUsername.text.Length >= 1) {
            PhotonNetwork.NickName = inputUsername.text;
            connectingText.text = "CONNECTING";
            // PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            isConnecting = true;
        }
    }

    //Offline to server connect. Connects to specific region
    public void OnClickConnectTo(string region) {
        print("CONNECTING TO SERVER: " + region);
        if(inputUsername.text.Length >= 1) {
            serverSettings.DevRegion = region;
            serverSettings.AppSettings.FixedRegion = region;

            PhotonNetwork.NickName = inputUsername.text;
            connectingText.text = "CONNECTING";
            PhotonNetwork.ConnectUsingSettings();
            isConnecting = true;
        }
    }

    //Server connect to lobby connect
    public override void OnConnectedToMaster() {
        print("SERVER TO LOBBY / OnConnectedToMaster");
        base.OnConnectedToMaster();

        //FINISH STEAM AUTHENTICATION
        CancelAuthTicket(hAuthTicket);

        //DISPLAY PHOTON SERVER APPID
        string appID = serverSettings.AppSettings.AppIdRealtime;
        string appText = "";
        //Display only first five letters of appID
        for(int i = 0; i < 5; i++) {
            appText += appID[i];
        }

        serverRegion.text = "REGION: " + PhotonNetwork.CloudRegion + "\nSERVER: " + appText;
        // serverSettings.DevRegion

        //Set score to zero/0
        myCustomProperties["Score"] = 0;
        PhotonNetwork.LocalPlayer.CustomProperties = myCustomProperties;

        AudioManager.current.PlayUI(0);

        PhotonNetwork.JoinLobby();
            
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //lobby connect
    public override void OnJoinedLobby() {
        print("JOINED LOBBY");
        base.OnJoinedLobby();

        ToggleMainMenuOn(true);
    }

    private void UpdateConnectingText() {
        if(connectingText.text == "CONNECTING...") {
            connectingText.text = "CONNECTING";
        } else {
            connectingText.text += ".";
        }
    }

    private void ToggleMainMenuOn(bool isOn) {
        panelMainMenu.SetActive(isOn);
        // panelDebug.SetActive(isOn);
        panelConnecting.SetActive(!isOn);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
