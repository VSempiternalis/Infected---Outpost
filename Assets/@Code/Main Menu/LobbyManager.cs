using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

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
        print("LOBBY MAN AWAKE");
    }

    private void Update() {
        if(PhotonNetwork.IsMasterClient && !buttonStart.activeSelf) buttonStart.SetActive(true);
        else if(!PhotonNetwork.IsMasterClient && buttonStart) buttonStart.SetActive(false);
    }

    public void OnClickHost() {
        print("ON CLICK HOST");
        if(inputRoomName.text.Length >= 1) PhotonNetwork.CreateRoom(inputRoomName.text, new RoomOptions(){MaxPlayers = 12});
    }

    public override void OnJoinedRoom() {
        print("ONJOINEDROOM");
        base.OnJoinedRoom();

        panelServerBrowser.SetActive(false);
        panelRoom.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;

        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomParent) {
        print("ON ROOM LIST UPDATE");
        base.OnRoomListUpdate(roomParent);

        if(Time.time >= nextUpdateTime) {
            UpdateRoomList(roomParent);
            nextUpdateTime = Time.time + updateInterval;
        }
    }

    private void UpdateRoomList(List<RoomInfo> list) {
        print("UPDATING ROOM LIST");
        //Destroy old room items
        foreach(RoomItem item in roomItemsList) {
            Destroy(item.gameObject);
        }
        roomItemsList.Clear();

        //Add new room items
        foreach(RoomInfo room in list) {
            RoomItem newRoom = Instantiate(roomItemPF, roomParent);
            newRoom.SetRoomName(room.Name);
            newRoom.SetPlayerCount(room.PlayerCount + "/" + room.MaxPlayers);
            // newRoom.SetPing(room.);
            roomItemsList.Add(newRoom);
        }

        roomParent.gameObject.SetActive(false);
        roomParent.gameObject.SetActive(true);
    }

    public void JoinRoom(string roomName) {
        print("JOIN ROOM");
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeave() {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom() {
        base.OnLeftRoom();

        // panelServerBrowser.SetActive(true);
        // panelRoom.SetActive(false);
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public void UpdatePlayerList() {
        print("Updating player list");
        if(PhotonNetwork.CurrentRoom == null) return;

        playerItemParent.gameObject.SetActive(false); //Dont remove

        foreach(PlayerItem item in playerItems) {
            Destroy(item.gameObject);
        }
        playerItems.Clear();

        foreach(KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players) {
            print("Adding player: " + player.Value.NickName);
            PlayerItem newPlayerItem = Instantiate(playerItemPF, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);
            playerItems.Add(newPlayerItem);
        }

        playerItemParent.gameObject.SetActive(true); //Dont remove

        //sfx
        AudioManager.current.PlayUI(3);
        print("Finished updating player list");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) {
        print("ON PLAYER ENTERED ROOM");
        base.OnPlayerEnteredRoom(newPlayer);

        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player newPlayer) {
        base.OnPlayerLeftRoom(newPlayer);

        UpdatePlayerList();
    }

    public void OnClickPlayButton() {
        print("CLICK PLAY BUTTON");
        PhotonNetwork.LoadLevel("SCENE - Outpost");
    }
}
