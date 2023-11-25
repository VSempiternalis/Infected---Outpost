using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;

public class WinManager : MonoBehaviourPunCallbacks {
    public static WinManager current;
    private bool isGameOver;

    [Space(10)]
    [Header("GENERATOR")]
    public bool gameStart;
    [SerializeField] private int countdown;
    [Header("UI")]
    [SerializeField] private TMP_Text generatorTimer;
    [SerializeField] private AudioHandler generator1AH;
    [SerializeField] private AudioHandler generator2AH;

    [Space(10)]
    [Header("ENDGAME")]
    private int humanCount;
    private int infectedCount;
    private int spectatorCount;

    [Space(10)]
    [Header("ENDGAME UI")]
    [SerializeField] private GameObject endgameUI;
    [SerializeField] private GameObject infectedWinGeneratorShutdown;
    [SerializeField] private GameObject infectedWinAssimilation;
    [SerializeField] private GameObject infectedWinExtinction;
    [SerializeField] private GameObject humansWinDisinfection;
    [SerializeField] private GameObject humansWinEscape;

    [Space(10)]
    [Header("CHARACTERS AND SCORE")]
    private List<Character> players = new List<Character>();

    private int nextSecUpdate = 1;

    private void Awake() {
        current = this;
    }
    
    private void Start() {
        nextSecUpdate = Mathf.FloorToInt(Time.time) + 1;
    }

    private void FixedUpdate() {
        if(PhotonNetwork.IsMasterClient && Time.time >= nextSecUpdate) {
            nextSecUpdate = Mathf.FloorToInt(Time.time) + 1;

            if(gameStart) {
                if(!isGameOver && countdown < 1) Endgame("Generator shutdown");

                countdown --;
                photonView.RPC("UpdateGeneratorTimerUIRPC", RpcTarget.All, countdown);
                // UpdateGeneratorTimerUI();
            }
        }
    }

    // GENERATOR ========================================

    public void StartCountdown() {
        print("Starting countdown!");
        generatorTimer.transform.parent.gameObject.SetActive(true);
        gameStart = true;
        countdown = GameMaster.countdownOnStart;
    }

    public void AddFuel() {
        print("ADDFUEL PLAYER COUNT");
        print("humans left: " + humanCount);
        print("infected left: " + infectedCount);
        countdown += GameMaster.fuelTimeAdd;
    }

    [PunRPC] private void UpdateGeneratorTimerUIRPC(int newCountdown) {
        countdown = newCountdown;
        UpdateGeneratorTimerUI();
    }

    private void UpdateGeneratorTimerUI() {
        int mins = countdown / 60;
        int secs = countdown % 60;
        string minsString = "00";
        string secsString = "00";

        if(countdown < 60) mins = 0;

        if(mins < 10) minsString = "0" + mins;
        else minsString = mins + "";
        if(secs < 10) secsString = "0" + secs;
        else secsString = secs + "";

        generatorTimer.text = minsString + ":" + secsString;

        //Update text color
        if(countdown < 60) {
            if(generatorTimer.color == Color.red) return;
            
            generatorTimer.color = Color.red;

            //Timer sfx
            if(!GetComponent<AudioSource>().isPlaying) GetComponent<AudioHandler>().Play(0);
            
            //Shutdown audio
            generator1AH.Play(1); 
            generator2AH.Play(1);
            generator1AH.GetComponent<AudioSource>().loop = false;
            generator2AH.GetComponent<AudioSource>().loop = false;
        } else {
            if(generatorTimer.color == Color.green) return;

            generatorTimer.color = Color.green;

            //Timer sfx
            if(GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().Stop();
            
            //Generator Loop audio
            generator1AH.Play(0); 
            generator2AH.Play(0);
            generator1AH.GetComponent<AudioSource>().loop = true;
            generator2AH.GetComponent<AudioSource>().loop = true;
        }
    }

    // GENERATOR ========================================

    // OTHER ========================================

    public void AddKill(int type) {
        print("Adding kill. Type: " + type);
        if(type == 0) infectedCount --;
        else humanCount --;
        print("humans left: " + humanCount);
        print("infected left: " + infectedCount);

        //Check for endgame stuff
        if(PhotonNetwork.IsMasterClient) {
            if(infectedCount <= 0) Endgame("Disinfection");
            else if(humanCount <= 0) Endgame("Assimilation");
        }
    }

    //Master Client/Host
    public void CountPlayers() {
        foreach(GameObject character in GameObject.FindGameObjectsWithTag("Character")) {
            // print("Counting characters: " + character.name + " Type: " + character.GetComponent<Character>().type);
            if(character.GetComponent<Character>().isPlayer) {
                // print("Counting players: " + character.name);
                if(character.GetComponent<Character>().type == 0) infectedCount ++;
                else if(character.GetComponent<Character>().type == 1) humanCount ++;
                else if(character.GetComponent<Character>().type == 2) spectatorCount ++;

                Controller.current.playerList.Add(character.GetComponent<Character>());
                
                players.Add(character.GetComponent<Character>());
            }
        }
    }

    //Only on master
    public void Endgame(string reason) {
        print("Endgame: IsMaster: " + photonView.Owner.IsMasterClient);
        // print("END GAME: " + reason);
        photonView.RPC("AddScoreRPC", RpcTarget.MasterClient, reason);
        // AddScoreRPC(reason);
        photonView.RPC("EndgameRPC", RpcTarget.All, reason);

        //Set room to visible
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    [PunRPC] private void EndgameRPC(string reason) {
        if(isGameOver) return;
        if(endgameUI.activeSelf) return;
        endgameUI.SetActive(true);
        // endgameUI.GetComponent<Animator>().enabled = false;

        isGameOver = true;

        //INFECTED
        if(reason == "Generator shutdown") {
            infectedWinGeneratorShutdown.SetActive(true); //Yes I know it's supposed to be in UIManager but I cant be assed rn
            GetComponent<AudioHandler>().Play(2);
        } else if(reason == "Assimilation") {
            infectedWinAssimilation.SetActive(true);
            GetComponent<AudioHandler>().Play(2);
        } else if(reason == "Extinction") {
            infectedWinExtinction.SetActive(true);
            GetComponent<AudioHandler>().Play(2);
        }

        //HUMAN
        if(reason == "Escape") {
            humansWinEscape.SetActive(true);
            GetComponent<AudioHandler>().Play(1);
        } else if(reason == "Disinfection") {
            humansWinDisinfection.SetActive(true);
            GetComponent<AudioHandler>().Play(1);
        }
    }

    //Only on Master Client/Host
    [PunRPC] private void AddScoreRPC(string reason) {
        print("AddScoreRPC");
        //INFECTED
        if(reason == "Generator shutdown" || reason == "Assimilation") {
            foreach(Character player in players) {
                if(player.type == 0) AddScore(player);
            }
        }

        //HUMAN
        // if(reason == "Escape" || reason == "Disinfection") {
        //     foreach(Character player in players) {
        //         if(player.type == 1) AddScore(player);
        //     }
        // }
        if(reason == "Disinfection") {
            foreach(Character player in players) {
                if(player.type == 1 || player.isAlive) AddScore(player);
            }
        } else if(reason == "Escape") {

        }
    }

    private void AddScore(Character player) {
        ExitGames.Client.Photon.Hashtable hash = player.photonView.Owner.CustomProperties;
        hash["Score"] = (int)hash["Score"] + 1;
        print("AddScore: " + player.name + ". Score is now: " + (int)hash["Score"]);
        player.photonView.Owner.SetCustomProperties(hash);
    }
}
