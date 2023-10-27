using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class RoomItem : MonoBehaviour {
    public TMP_Text roomName;
    public TMP_Text playerCount;
    public TMP_Text ping;

    private LobbyManager lm;

    private void Start() {
        lm = LobbyManager.current;
    }

    public void SetRoomName(string newName) {
        roomName.text = newName;
    }

    public void SetPlayerCount(string newCount) {
        playerCount.text = newCount;
    }

    // public void SetPing(string newPing) {
    //     string pingText = "";
 
    //     ping.text = pingText;
    // }

    public void OnClickJoin() {
        lm.JoinRoom(roomName.text);
    }
}
