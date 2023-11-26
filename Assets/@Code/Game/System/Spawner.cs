using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;

public class Spawner : MonoBehaviourPunCallbacks {
    public static Spawner current;

    [SerializeField] private Transform[] spawnPoints;
    private List<int> spawnedList = new List<int>(); //list of ints for characters that have spawned

    [SerializeField] public GameObject[] characterPrefabs; //ADDNEWCHARACTER
    [SerializeField] public GameObject[] itemPrefabs; //ADDNEWITEM
    [SerializeField] private Transform[] storages;
    private bool playersSpawned;
    private List<string> playerSpawnedList = new List<string>();

    private bool gameStart = false;
    private float nextSecUpdate = 1f;
    private float gameStartTime = 3f; //AI are spawned and players are activated on this second. Players cannot spawn after this

    private GameObject localPlayer;

    public TMP_Text loadText;

    public GameObject lights;

    [SerializeField] private TMP_Text ping;

    private int playersInGame;
    private string playersInGameString;
    private bool isAllPlayersInGame;
    private int typesSet;
    // private int typesSet;
    private bool allTypesSet;

    [SerializeField] private List<AudioSource> generators;

    [SerializeField] private TMP_Text playersInGameCounter;
    [SerializeField] private TMP_Text playersInGameStringUI;

    [Header("DEBUG")]
    [SerializeField] private TMP_Text isMasterText;
    [SerializeField] private TMP_Text gameStartText;
    [SerializeField] private TMP_Text isAllPlayersInGameText;
    [SerializeField] private TMP_Text allTypesSetText;
    [SerializeField] private TMP_Text playersSpawnedText;

    private void Awake() {
        current = this;
        uiManager.current.loadingPanel.SetActive(true);
    }

    private void Start() {
        print("[Spawner] Start");
        // GetPrefabs();
        nextSecUpdate = Time.time + 1f;
        gameStartTime = Time.time + gameStartTime;
        //[0] Check if character already has a player
        //[1] Embody player into character
        //[2] rpc other players that this character is taken

        //Ambience audio
        // loopAH.PlayClip(1);
        print("Calling InGameRPC: " + PhotonNetwork.LocalPlayer.NickName);
        photonView.RPC("InGameRPC", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.NickName);
    }

    private void Update() {
        //PING
        int pingNum = PhotonNetwork.GetPing();
        Color color = Color.white;
        if(pingNum > 99) color = Color.red;

        ping.text = "PING: " + pingNum;
        ping.color = color;

        //DEBUG
        isMasterText.text = "IS MASTER: " + (PhotonNetwork.IsMasterClient? "true" : "false");
        gameStartText.text = "GAME START: " + (gameStart? "true" : "false");
        isAllPlayersInGameText.text = "ALL PLAYERS IN GAME: " + (isAllPlayersInGame? "true" : "false");
        allTypesSetText.text = "ALL TYPES SET: " + (allTypesSet? "true" : "false");
        playersSpawnedText.text = "PLAYERS SPAWNED: " + (playersSpawned? "true" : "false");

    }

    private void FixedUpdate() {
        if(Time.time >= nextSecUpdate) {
            nextSecUpdate = Mathf.FloorToInt(Time.time) + 1;

            if(PhotonNetwork.IsMasterClient && !gameStart && isAllPlayersInGame && !allTypesSet && !playersSpawned) {

                //Check if players greater than prefabs (Results in endless loop)
                if(PhotonNetwork.PlayerList.Length > characterPrefabs.Length) {
                    print("Not enough character prefabs");
                    loadText.text = "ERROR: NOT ENOUGH CHARACTER PREFABS!";
                    gameStart = true;
                    return;

                    // PhotonNetwork.PlayerList[0].SetCustomProperties
                }
                    SpawnPlayers();
                    SetTypes();
                    SpawnAI();
                    SpawnItems();

                    // photonView.RPC("StartGameRPC", RpcTarget.All);
                    // photonView.RPC("ActivateTypePopupRPC", RpcTarget.All);

                // if(Time.time < gameStartTime) {
                //     SpawnPlayers();
                // } else if(Time.time > gameStartTime + 2) {
                //     SetTypes();
                //     SpawnAI();
                //     SpawnItems();

                //     photonView.RPC("StartGameRPC", RpcTarget.All);
                //     photonView.RPC("ActivateTypePopupRPC", RpcTarget.All);
                // }
            } else if(!gameStart && allTypesSet) {
                photonView.RPC("StartGameRPC", RpcTarget.All);
                photonView.RPC("ActivateTypePopupRPC", RpcTarget.All);
            }
        }
    }

    //============================================================

    //Called to master client when local player is in game scene
    [PunRPC] private void InGameRPC(string newPlayerName) {
        playersInGame ++;
        playersInGameString += newPlayerName + "\n";
        print("InGameRPC: " + newPlayerName + ". Players in game: " + playersInGame + " / Players in room: " + PhotonNetwork.CurrentRoom.PlayerCount);
        photonView.RPC("UpdatePlayerInGameCounterRPC", RpcTarget.All, playersInGame, playersInGameString);

        if(playersInGame == PhotonNetwork.CurrentRoom.PlayerCount) {
            isAllPlayersInGame = true;
        }
    }

    [PunRPC] private void UpdatePlayerInGameCounterRPC(int playersInGame, string playersInGameString) {
        playersInGameCounter.text = playersInGame + "/" + PhotonNetwork.CurrentRoom.PlayerCount;
        playersInGameStringUI.text = playersInGameString;
    }

    // private void GetPrefabs() {
    //     print("Getting Prefabs");
    //     print("Character prefab count: " + characterPrefabs.Length);
    // }

    //============================================================

    private void SpawnPlayers() {
        print("[SPAWNER] SpawnPlayers");
        loadText.text = "Spawning Players...";

        foreach(Player player in PhotonNetwork.PlayerList) {
            print("!Spawning player: " + player.NickName + " ID: " + player.UserId);
            string playerName = player.UserId;
            if(playerSpawnedList.Contains(playerName)) return;
            playerSpawnedList.Add(playerName);
            while(true) {
                int randInt = Random.Range(0, characterPrefabs.Length); //12 ADDNEWCHARACTER
                if(!spawnedList.Contains(randInt)) {
                    spawnedList.Add(randInt);
                    photonView.RPC("GiveSpawnRPC", RpcTarget.All, playerName, randInt);
                    break;
                }
            }
        }

        playersSpawned = true;
    }

    [PunRPC] private void GiveSpawnRPC(string playerName, int spawnInt) {
        // loadText.text = "Spawning Player: " + playerName;
        //[!] Return if player running RPC isn't supposed to get this spawnInt
        if(playerName != PhotonNetwork.LocalPlayer.UserId) return;
        print("[SPAWNER] Spawning Player: " + playerName);
        //Control random character
        //GameObject player = characterList.transform.GetChild(spawnInt).gameObject;
        //Instantiate random character
        string prefabName = characterPrefabs[spawnInt].name;
        GameObject player = PhotonNetwork.Instantiate(prefabName, spawnPoints[spawnInt].position, Quaternion.identity);
        player.SetActive(true);
        //player.GetComponent<aiInput>().enabled = false;

        // SETUP ==================
        //player.GetComponent<SpriteRenderer>().sprite = characterSprites[spawnInt];
        //[] Remove aiInput, add PlayerInput
        // Destroy(player.GetComponent<aiInput>());
        player.GetComponent<Character>().RemoveAI();
        // Destroy(player.GetComponent<NavMeshAgent>());
        //[] Set controller to this player
        Controller.current.SetPlayer(player.GetComponent<Rigidbody>());
        // player.AddComponent<PlayerInput>();
        player.GetComponent<Character>().spawnInt = spawnInt;
        player.GetComponent<Character>().SetIsPlayer();

        //[] Activate vision
        player.GetComponent<Character>().lightView.SetActive(true);
        // player.transform.GetChild(1).gameObject.SetActive(true);
        // player.transform.GetChild(2).gameObject.SetActive(true);
        // player.name = "LocalPlayer";
        localPlayer = player;
    }

    //============================================================
 
    private void SetTypes() {
        print("Setting types");
        // loadText.text = "Setting player types...";

        if(allTypesSet) return;
        int randomHostIndex = Random.Range(0, spawnedList.Count);
        int hostInt = spawnedList[randomHostIndex];
        photonView.RPC("SetTypesRPC", RpcTarget.All, hostInt);
        // allTypesSet = true;
    }

    [PunRPC] private void SetTypesRPC(int hostInt) {
        print("Set types RPC: " + hostInt);
        // loadText.text = "Setting local player type...";
        // print("!localPlayer: ");
        // print(localPlayer);
        if(localPlayer.GetComponent<Character>().spawnInt == hostInt) {
            localPlayer.GetComponent<Character>().SetPlayerType(0); //0 - Infected
        } else {
            localPlayer.GetComponent<Character>().SetPlayerType(1); //1 - Human
        }
    }

    public void AddTypeSet() {
        print("Add Types Set");
        typesSet ++;
        print("Types Set: " + typesSet);
        if(typesSet == playersInGame) allTypesSet = true;
    }

    //============================================================

    private void SpawnAI() {
        // loadText.text = "Spawning AI";
        int i = 0;
        for(i = 0; i < characterPrefabs.Length; i++) { 
            //Spawn character if not already controlled by player
            if(!spawnedList.Contains(i)) {
                print("Spawning AI: " + i);
                spawnedList.Add(i);
                string prefabName = characterPrefabs[i].name;
                GameObject npc = PhotonNetwork.Instantiate(prefabName, spawnPoints[i].position, Quaternion.identity);
                npc.GetComponent<aiInput>().enabled = true;
                npc.SetActive(true);
                // Destroy(npc.GetComponent<PopupHandler>());
            }
        }
    }

    //============================================================

    private void SpawnItems() {
        // loadText.text = "Spawning Items";
        List<int> spawnInts = new List<int>(); //int of items already spawned
        
        foreach(GameObject item in itemPrefabs) {
            int spawnInt;

            //End loop when there are more items than storage
            if(spawnInts.Count >= storages.Length) return;

            while(true) {
                spawnInt = Random.Range(0, storages.Length);
                if(!spawnInts.Contains(spawnInt)) {
                    spawnInts.Add(spawnInt);

                    break;
                }
            }
            PhotonNetwork.Instantiate(item.name, storages[spawnInt].position, Quaternion.identity);
            storages[spawnInt].GetComponent<Storage>().InsertItem(item.GetComponent<ItemHandler>());
        }
    }

    //============================================================

    [PunRPC] private void StartGameRPC() {
        gameStart = true;
        // lights.SetActive(false);
        uiManager.current.loadingPanel.SetActive(false);
        WinManager.current.StartCountdown();
        WinManager.current.CountPlayers();

        //Destroy all inactive aiInputs
        foreach(aiInput ai in GameObject.FindObjectsOfType<aiInput>()) {
            if(!ai.enabled) Destroy(ai);
        }

        //Activate all generators
        // foreach(AudioSource generatorAS in generators) {
        //     generatorAS.Play();
        // }
    }

    [PunRPC] private void ActivateTypePopupRPC() {
        uiManager.current.ActivateTypePopupRPC(localPlayer.GetComponent<Character>().type);
    }
}
