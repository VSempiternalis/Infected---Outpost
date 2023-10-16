using UnityEngine;
using TMPro;

public class WinManager : MonoBehaviour {
    public static WinManager current;

    [Space(10)]
    [Header("GENERATOR")]
    public bool gameStart;
    [SerializeField] private int countdownOnStart; //value of countdown on start
    [SerializeField] private int countdown;
    [SerializeField] private int fuelTimeAdd; //num of secs fuel adds to countdown
    [Header("UI")]
    [SerializeField] private TMP_Text generatorTimer;
    // [SerializeField] private TMP_Text generatorTimer2; //Infected timer
    // [SerializeField] private TMP_Text generatorTimer3; //Spectator timer
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

    private int nextSecUpdate = 1;

    private void Awake() {
        current = this;
    }
    
    private void Start() {
        nextSecUpdate = Mathf.FloorToInt(Time.time) + 1;
    }

    private void FixedUpdate() {
        if(Time.time >= nextSecUpdate) {
            nextSecUpdate = Mathf.FloorToInt(Time.time) + 1;

            if(gameStart) {
                if(countdown < 1) Endgame("Generator shutdown");

                countdown --;
                UpdateGeneratorTimerUI();
            }
        }
    }

    // GENERATOR ========================================

    public void StartCountdown() {
        print("Starting countdown!");
        generatorTimer.transform.parent.gameObject.SetActive(true);
        gameStart = true;
        countdown = countdownOnStart;
    }

    public void AddFuel() {
        countdown += fuelTimeAdd;
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
        // generatorTimer2.text = minsString + ":" + secsString;
        // generatorTimer3.text = minsString + ":" + secsString;

        //Update text color
        if(countdown < 60) {
            if(generatorTimer.color == Color.red) return;
            
            generatorTimer.color = Color.red;
            // generatorTimer2.color = Color.red;
            // generatorTimer3.color = Color.red;

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
            // generatorTimer2.color = Color.green;
            // generatorTimer3.color = Color.green;

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
        if(type == 0) infectedCount --;
        else humanCount --;

        //Check for endgame stuff
        if(infectedCount <= 0) Endgame("Disinfection");
        else if(humanCount <= 0) Endgame("Assimilation");
    }

    public void CountPlayers() {
        foreach(GameObject character in GameObject.FindGameObjectsWithTag("Character")) {
            if(character.GetComponent<Character>().isPlayer) {
                print("Counting players: " + character.name);
                if(character.GetComponent<Character>().type == 0) infectedCount ++;
                else if(character.GetComponent<Character>().type == 1) humanCount ++;
                else if(character.GetComponent<Character>().type == 2) spectatorCount ++;

                Controller.current.playerList.Add(character.GetComponent<Character>());
            }
        }
    }

    public void Endgame(string reason) {
        print("END GAME: " + reason);
        // Debug.Log("Infected win!");
        //Application.Quit();
        if(endgameUI.activeSelf) return;
        endgameUI.SetActive(true);

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
        } if(reason == "Disinfection") {
            humansWinDisinfection.SetActive(true);
            GetComponent<AudioHandler>().Play(1);
        }
    }

    public void ExitGame() {

    }
}
