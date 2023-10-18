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

    [SerializeField] private TMP_Text connectingText;
    private bool isConnecting;

    private float timer = 0f;
    private float interval = 0.5f;

    [SerializeField] private TMP_InputField inputUsername;

    [SerializeField] private List<GameObject> ActivateOnAwake;
    [SerializeField] private List<GameObject> DeactivateOnAwake;

    [SerializeField] private GameObject connectingPanel;
    [SerializeField] private GameObject vlgPanel;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject serverBrowserPanel;
    [SerializeField] private GameObject roomPanel;

    [SerializeField] private TMP_Text versionText;

    [SerializeField] private ServerSettings serverSettings;
    [SerializeField] private TMP_Text serverRegion;

    private void Awake() {
        current = this;

        //Set version text
        versionText.text = "v" + Application.version;
    }

    private void Start() {
        connectingPanel.SetActive(false);
        mainPanel.SetActive(true);

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

    //Server connect to lobby connect
    public override void OnConnectedToMaster() {
        print("SERVER TO LOBBY / OnConnectedToMaster");
        base.OnConnectedToMaster();

        serverRegion.text = "REGION: " + serverSettings.DevRegion;

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
