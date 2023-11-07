using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;

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

    private void Awake() {
        SetUserCount();

        current = this;

        //Set version text
        versionText.text = "VERSION: v" + Application.version;
    }

    private void Start() {
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
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            isConnecting = true;
        }
    }

    //Offline to server connect
    public void OnClickConnectTo(string region) {
        print("CONNECTING TO SERVER: " + region);
        if(inputUsername.text.Length >= 1) {
            serverSettings.DevRegion = region;
            serverSettings.AppSettings.FixedRegion = region;

            PhotonNetwork.NickName = inputUsername.text;
            connectingText.text = "CONNECTING";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
            isConnecting = true;
        }
    }

    //Server connect to lobby connect
    public override void OnConnectedToMaster() {
        print("SERVER TO LOBBY / OnConnectedToMaster");
        base.OnConnectedToMaster();

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
