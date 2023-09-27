using UnityEngine;
using TMPro;
using Photon.Pun;

public class ConnectingManager : MonoBehaviourPunCallbacks {
    [SerializeField] private TMP_Text connectingText;
    private bool isConnecting;

    private float timer = 0f;
    private float interval = 0.5f;

    [SerializeField] private TMP_InputField inputUsername;

    private void Update() {
        timer += Time.deltaTime;

        if(isConnecting && timer >= interval) {
            UpdateConnectingText();

            timer = 0f;
        }
    }

    public void OnClickConnect() {
        if(inputUsername.text.Length >= 1) {
            PhotonNetwork.NickName = inputUsername.text;
            connectingText.text = "CONNECTING";
            PhotonNetwork.ConnectUsingSettings();
            isConnecting = true;
        }
    }

    private void UpdateConnectingText() {
        if(connectingText.text == "CONNECTING...") {
            connectingText.text = "CONNECTING";
        } else {
            connectingText.text += ".";
        }
    }
}
