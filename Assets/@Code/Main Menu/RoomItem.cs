using UnityEngine;
using TMPro;
using WebSocketSharp;

public class RoomItem : MonoBehaviour {
    public TMP_Text roomName;
    public TMP_Text playerCount;
    public TMP_Text ping;
    public TMP_InputField passwordInput;
    public string password;

    private LobbyManager lm;

    // [SerializeField] private GameObject lockImg;

    private void Start() {
        lm = LobbyManager.current;

    }

    public void SetRoomName(string newName) {
        roomName.text = newName;
    }

    public void SetPlayerCount(string newCount) {
        playerCount.text = newCount;
    }

    public void SetPassword(string newPassword) {
        // print("Setting password to: " + newPassword);
        password = newPassword;

        if(newPassword.IsNullOrEmpty()) {
            // lockImg.SetActive(true);
            passwordInput.gameObject.SetActive(false);
        }
    }

    // public void SetPing(string newPing) {
    //     string pingText = "";
 
    //     ping.text = pingText;
    // }

    public void OnClickJoin() {
        if(password.IsNullOrEmpty()) {
            lm.JoinRoom(roomName.text);
        } else if(passwordInput.text == password) {
            lm.JoinRoom(roomName.text);
        }
    }
}
