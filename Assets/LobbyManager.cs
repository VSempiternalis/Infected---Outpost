using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using Unity.VisualScripting;
using Steamworks;
using UnityEngine.PlayerLoop;

public class LobbyManager : MonoBehaviourPunCallbacks {
    public static LobbyManager current;

    [SerializeField] private TMP_InputField inputRoomName;
    [SerializeField] private GameObject panelServerBrowser;
    [SerializeField] private GameObject panelRoom;
    [SerializeField] private TMP_Text roomName;

    [SerializeField] private RoomItem roomItemPF;
    private List<RoomItem> roomItemsList = new List<RoomItem>();
    [SerializeField] private Transform roomParent;

    public float updateInterval = 1.5f;
    private float nextUpdateTime;

    private List<PlayerItem> playerItems = new List<PlayerItem>();
    [SerializeField] private PlayerItem playerItemPF;
    [SerializeField] private Transform playerItemParent;

    [SerializeField] private GameObject buttonStart;

    private void Awake() {
        current = this;
    }

    private void Update() {
        if(PhotonNetwork.IsMasterClient) buttonStart.SetActive(true);
        else buttonStart.SetActive(false);
    }

    public void OnClickHost() {
        if(inputRoomName.text.Length >= 1) PhotonNetwork.CreateRoom(inputRoomName.text, new RoomOptions(){MaxPlayers = 12});
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();

        panelServerBrowser.SetActive(false);
        panelRoom.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomParent) {
        base.OnRoomListUpdate(roomParent);

        if(Time.time >= nextUpdateTime) {
            UpdateRoomList(roomParent);
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list) {
        //Destroy old room items
        foreach(RoomItem item in roomItemsList) {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        //Add new room items
        foreach(RoomInfo room in list) {
            RoomItem newRoom = Instantiate(roomItemPF, roomParent);
            newRoom.SetRoomName(room.Name);
            roomItemsList.Add(newRoom);
        }

        roomParent.gameObject.SetActive(false);
        roomParent.gameObject.SetActive(true);
    }

    public void JoinRoom(string roomName) {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeave() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        panelServerBrowser.SetActive(true);
        panelRoom.SetActive(false);
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    private void UpdatePlayerList() {
        if(PhotonNetwork.CurrentRoom == null) return;

        playerItemParent.gameObject.SetActive(false);

        foreach(PlayerItem item in playerItems) {
            Destroy(item.gameObject);
        }
        playerItems.Clear();

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
            PlayerItem newPlayerItem = Instantiate(playerItemPF, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItems.Add(newPlayerItem);
        }
        playerItemParent.gameObject.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        base.OnPlayerEnteredRoom(newPlayer);

        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player newPlayer) {
        base.OnPlayerLeftRoom(newPlayer);

        UpdatePlayerList();
    }

    public void OnClickPlayButton() {
        PhotonNetwork.LoadLevel("SCENE - Outpost");
    }
}
