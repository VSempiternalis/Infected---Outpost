using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class Spawner : MonoBehaviourPunCallbacks {
    public static Spawner current;

    //private GameObject[] playerPrefabs;
    [SerializeField] private Transform[] spawnPoints;
    //[SerializeField] private GameObject characterList;
    private List<int> spawnedList = new List<int>(); //list of ints for characters that have spawned

    //[SerializeField] private Sprite[] characterSprites;
    [SerializeField] public GameObject[] characterPrefabs; //ADDNEWCHARACTER
    [SerializeField] public GameObject[] itemPrefabs; //ADDNEWITEM
    // [SerializeField] private Transform items;
    [SerializeField] private Transform[] storages;
    private bool playersSpawned;
    private List<string> playerSpawnedList = new List<string>();

    private bool gameStart = false;
    private float nextSecUpdate = 1f;
    private float gameStartTime = 3f; //AI are spawned and players are activated on this second. Players cannot spawn after this

    private GameObject localPlayer;

    // [SerializeField] private GameObject loadingCanvas;
    // [SerializeField] private TMP_Text playerType;
    
    // [SerializeField] private AudioHandler loopAH;
    public TMP_Text loadText;

    private bool typesSet = false;

    public GameObject lights;

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
    }

    private void FixedUpdate() {
        if(Time.time >= nextSecUpdate) {
            nextSecUpdate = Mathf.FloorToInt(Time.time) + 1;

            if(PhotonNetwork.IsMasterClient && !gameStart) {

                //Check if players greater than prefabs (Results in endless loop)
                if(PhotonNetwork.PlayerList.Length > characterPrefabs.Length) {
                    print("Not enough character prefabs");
                    loadText.text = "ERROR: NOT ENOUGH CHARACTER PREFABS!";
                    gameStart = true;
                    return;
                }

                if(Time.time < gameStartTime) {
                    SpawnPlayers();
                } else if(Time.time > gameStartTime + 2) {
                    SetTypes();
                    SpawnAI();
                    SpawnItems();
                    
                    // Enabling AI
                    // foreach(aiInput ai in GameObject.FindObjectsOfType<aiInput>()) {
                    //     // if(ai.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer) ai.AddComponent<aiInput>();
                    //     // else { 
                    //     //     GameObject player = ai.gameObject;
                    //     //     Destroy(player.GetComponent<aiInput>());
                    //     //     player.GetComponent<PlayerInput>().enabled = true;
                    //     // }
                    // }

                    photonView.RPC("StartGameRPC", RpcTarget.All);
                    photonView.RPC("ActivateTypePopupRPC", RpcTarget.All);
                }
            }
        }
    }

    //============================================================

    private void GetPrefabs() {
        print("Getting Prefabs");
        // GameObject Abara = Resources.Load("Abara") as GameObject;
        // GameObject Adams = Resources.Load("Adams") as GameObject;
        // GameObject Agatha = Resources.Load("Agatha") as GameObject;
        // GameObject Bukowski = Resources.Load("Bukowski") as GameObject;
        // GameObject Corey = Resources.Load("Corey") as GameObject;
        // GameObject Haldeman = Resources.Load("Haldeman") as GameObject;
        // GameObject Isaac = Resources.Load("Isaac") as GameObject;
        // GameObject Jennica = Resources.Load("Jennica") as GameObject;
        // GameObject Macdaddy = Resources.Load("Macdaddy") as GameObject;
        // GameObject Miku = Resources.Load("Miku") as GameObject;
        // GameObject Orwell = Resources.Load("Orwell") as GameObject;
        // GameObject Young = Resources.Load("Young") as GameObject;

        // characterPrefabs = new GameObject[]{Abara, Adams, Agatha, Bukowski, Corey, Haldeman, Isaac, Jennica, Macdaddy, Miku, Orwell, Young};
        
        //ADDNEWCHARACTER
        // GameObject abara = Resources.Load("Character_Abara") as GameObject;
        // GameObject adams = Resources.Load("Character_Adams") as GameObject;
        // GameObject bukowski = Resources.Load("Character_Bukowski") as GameObject;

        // characterPrefabs = new GameObject[]{abara, adams, bukowski}; //ADDNEWCHARACTER

        print("Character prefab count: " + characterPrefabs.Length);
    }

    //============================================================

    private void SpawnPlayers() {
        // print("[SPAWNER] SpawnPlayers");
        loadText.text = "Spawning Players...";

        foreach(Player player in PhotonNetwork.PlayerList) {
            string playerName = player.NickName;
            if(playerSpawnedList.Contains(playerName)) return;
            playerSpawnedList.Add(playerName);
            while(true) {
                int randInt = Random.Range(0, characterPrefabs.Length); //12 ADDNEWCHARACTER
                if(!spawnedList.Contains(randInt)) {
                    spawnedList.Add(randInt);
                    photonView.RPC("GiveSpawn", RpcTarget.All, playerName, randInt);
                    break;
                }
            }
        }
    }

    [PunRPC] private void GiveSpawn(string playerName, int spawnInt) {
        print("[SPAWNER] Spawning Player: " + playerName);
        loadText.text = "Spawning Player: " + playerName;
        //[!] Return if player running RPC isn't supposed to get this spawnInt
        if(playerName != PhotonNetwork.LocalPlayer.NickName) return;
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
        loadText.text = "Setting player types...";

        if(typesSet) return;
        int randomHostIndex = Random.Range(0, spawnedList.Count);
        int hostInt = spawnedList[randomHostIndex];
        photonView.RPC("SetTypesRPC", RpcTarget.All, hostInt);
        typesSet = true;
    }

    [PunRPC] private void SetTypesRPC(int hostInt) {
        print("Set types RPC: " + hostInt);
        loadText.text = "Setting local player type...";
        if(localPlayer.GetComponent<Character>().spawnInt == hostInt) {
            localPlayer.GetComponent<Character>().SetPlayerType(0); //0 - Infected
        } else {
            localPlayer.GetComponent<Character>().SetPlayerType(1); //1 - Human
        }
    }

    //============================================================

    private void SpawnAI() {
        loadText.text = "Spawning AI";
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
        loadText.text = "Spawning Items";
        List<int> spawnInts = new List<int>(); //int of items already spawned
        
        //List of item ints
        // List<int> toSpawnInts = new List<int>();
        // for(int i = 0; i < items.childCount; i++) {
        //     toSpawnInts.Add(i);
        // }

        // //
        // for(int i = 0; i < items.childCount; i++) {
        //     items.GetChild(i);

        //     if(i >= storages.Length) return;

        //     int randInt = Random.Range(0, toSpawnInts.Count);
        // }
        
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
            // this.photonView.RPC("InsertItemRPC", RpcTarget.All, item.name + "(Clone)", spawnInt);
        }
    }

    // [PunRPC] private void InsertItemRPC(string itemName, int spawnInt) {
    //     print("Inserting item: " + itemName + " to " + spawnInt);
    //     Storage storage = storages[spawnInt].GetComponent<Storage>();
    //     ItemHandler item = GameObject.Find(itemName).GetComponent<ItemHandler>();

    //     storage.InsertItem(item);
    // }

    // [PunRPC] private void SpawnItem(string itemName, int spawnInt) {
    //     loadText.text = "Spawning Items";
    //     Transform spawnPoint = storages[spawnInt].transform;
    //     GameObject item = GameObject.Find(itemName);

    //     item.transform.parent = spawnPoint;
    //     item.GetComponent<SpriteRenderer>().spriteSortPoint = item.transform.parent.GetComponent<SpriteRenderer>().spriteSortPoint - 1;
    //     item.transform.localPosition = new Vector3(0, 0, 0);
    // }

    //============================================================

    [PunRPC] private void StartGameRPC() {
        gameStart = true;
        lights.SetActive(false);
        uiManager.current.loadingPanel.SetActive(false);
        WinManager.current.StartCountdown();
        WinManager.current.CountPlayers();

        //Destroy all inactive aiInputs
        foreach(aiInput ai in GameObject.FindObjectsOfType<aiInput>()) {
            if(!ai.enabled) Destroy(ai);
        }
    }

    [PunRPC] private void ActivateTypePopupRPC() {
        uiManager.current.ActivateTypePopupRPC(localPlayer.GetComponent<Character>().type);
    }
}
