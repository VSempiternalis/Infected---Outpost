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
    [SerializeField] private MapSettings mapSettings;

    [SerializeField] private TMP_InputField password;

    private void Awake() {
        current = this;
        print("LOBBY MAN AWAKE");
    }

    private void Update() {
        if(PhotonNetwork.IsMasterClient) { //HOST
            if(!buttonStart.activeSelf) buttonStart.SetActive(true);
            if(!mapSettings.isInteractable) mapSettings.SetSettingsInteractable(true);
        } else if(!PhotonNetwork.IsMasterClient) { //CLIENT
            if(buttonStart.activeSelf) buttonStart.SetActive(false);
            if(mapSettings.isInteractable) mapSettings.SetSettingsInteractable(false);
        } 
    }

    public void OnClickHost() {
        print("ON CLICK HOST");
        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        table.Add("Password", password.text);    

        RoomOptions roomOptions = new RoomOptions(){MaxPlayers = 12, PublishUserId = true};
        roomOptions.CustomRoomProperties = table;
        roomOptions.CustomRoomPropertiesForLobby = new string[] {"Password"};
        // print("HOST PASSWORD: " + (string)roomOptions.CustomRoomProperties["Password"]);
        // if(inputRoomName.text.Length >= 1) PhotonNetwork.CreateRoom(inputRoomName.text, new RoomOptions(){MaxPlayers = 12, PublishUserId = true});
        
        if(inputRoomName.text.Length >= 1) PhotonNetwork.CreateRoom(inputRoomName.text, roomOptions);
    }

    public override void OnJoinedRoom() {
        print("ONJOINEDROOM");
        base.OnJoinedRoom();

        panelServerBrowser.SetActive(false);
        panelRoom.SetActive(true);
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        mapSettings.GetSettingsFromMaster();

        UpdatePlayerList();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) {
        print("ON ROOM LIST UPDATE");
        base.OnRoomListUpdate(roomList);
        UpdateRoomList(roomList);

        // if(Time.time >= nextUpdateTime) {
        //     UpdateRoomList(roomParent);
        //     nextUpdateTime = Time.time + updateInterval;
        // }
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
            if(room.IsOpen && room.IsVisible && room.MaxPlayers > 0) {
                RoomItem newRoom = Instantiate(roomItemPF, roomParent);
                newRoom.SetRoomName(room.Name);
                newRoom.SetPlayerCount(room.PlayerCount + "/" + room.MaxPlayers);
                // newRoom.SetPing(room.);
                newRoom.SetPassword((string)room.CustomProperties["Password"]);
                // print("PASSWORD: " + (string)room.CustomProperties["Password"]);
                roomItemsList.Add(newRoom);
            }
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

        SetScoreTo(0);
        // panelServerBrowser.SetActive(true);
        // panelRoom.SetActive(false);
    }

    public override void OnConnectedToMaster() {
        base.OnConnectedToMaster();

        // PhotonNetwork.JoinLobby();
    }

    public void UpdatePlayerList() {
        print("Updating player list");
        if(PhotonNetwork.CurrentRoom == null) return;

        playerItemParent.gameObject.SetActive(false); //Dont remove

        foreach(PlayerItem item in playerItems) {
            Destroy(item.gameObject);
        }
        playerItems.Clear();

        foreach(KeyValuePair<int, Player> kvp in PhotonNetwork.CurrentRoom.Players) {
            Player player = kvp.Value;
            print("Adding player: " + player.NickName);
            PlayerItem newPlayerItem = Instantiate(playerItemPF, playerItemParent);
            newPlayerItem.SetPlayerInfo(player);

            //Score
            int score = 0; // Default value if "Score" is not set
            if(player.CustomProperties.TryGetValue("Score", out var scoreObj) && scoreObj is int) score = (int)scoreObj;
            newPlayerItem.playerScore.text = score.ToString();

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
        mapSettings.Starting();
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("SCENE - Outpost");
    }

    //================================================================================

    // private void ResetScore() {
    //     ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
    //     hash["Score"] = 0;
    //     PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    // }

    private void SetScoreTo(int newScore) {
        print("SetLocalScoreTo: " + newScore);
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["Score"] = newScore;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
