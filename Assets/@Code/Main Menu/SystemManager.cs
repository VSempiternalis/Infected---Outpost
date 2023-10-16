using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;

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

    [SerializeField] private TMP_Text versionText;

    private void Awake() {
        current = this;

        foreach(GameObject thing in DeactivateOnAwake) {
            thing.SetActive(false);
        }

        foreach(GameObject thing in ActivateOnAwake) {
            thing.SetActive(true);
        }

        // ToggleMainMenuOn(false);

        //Set version text
        versionText.text = "v" + Application.version;
    }

    private void Update() {
        if(isConnecting && Time.time >= timer) {
            UpdateConnectingText();

            timer = Time.time + interval;
        }
    }

    //Offline to server connect
    public void OnClickConnect() {
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
        base.OnConnectedToMaster();

        AudioManager.current.PlayUI(0);

        PhotonNetwork.JoinLobby();
    }

    //lobby connect
    public override void OnJoinedLobby() {
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
