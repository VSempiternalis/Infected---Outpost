using UnityEngine;
using TMPro;

public class uiManager : MonoBehaviour {
    public static uiManager current;

    [Space(10)]
    [Header("UI")]
    public GameObject loadingPanel;
    [SerializeField] private TMP_Text playerType;
    [SerializeField] private GameObject typePopup;

    [SerializeField] private GameObject infectedUI;
    [SerializeField] private GameObject humanUI;
    [SerializeField] private GameObject spectatorUI;
    [SerializeField] private RectTransform infectedStaminaImg;
    [SerializeField] private RectTransform humanStaminaImg;
    [SerializeField] private RectTransform spectatorStaminaImg;
    [SerializeField] private RectTransform staminaImg;
    
    private void Awake() {
        current = this;
        // loadingPanel.SetActive(true);
        typePopup.SetActive(false);
    }

    private void Start() {
        infectedUI.SetActive(false);
        humanUI.SetActive(false);
        spectatorUI.SetActive(false);
    }

    private void Update() {
        
    }

    public void SetPanels(int type) {
        print("[CHARACTER] Setting panels. Type: " + type);
        Spawner.current.loadText.text = "Setting panels...";

        if(type == 0) {
            infectedUI.SetActive(true);
            staminaImg = infectedStaminaImg;
        } else if(type == 1) {
            humanUI.SetActive(true);
            staminaImg = humanStaminaImg;
        } else if(type == 2) {
            spectatorUI.SetActive(true);
            staminaImg = spectatorStaminaImg;
        }

        loadingPanel.SetActive(false);
    }

    public void ActivateTypePopupRPC(int type) {
        typePopup.SetActive(true);
        Character player = GameObject.Find("LocalPlayer").GetComponent<Character>();

        //Activate type popper
        // playerType.transform.parent.gameObject.SetActive(true);

        if(player.GetComponent<Character>().type == 0) {
            playerType.text = "Infected";
            playerType.color = new Color(0.6470588f, 0.1882353f, 0.1882353f, 0.282353f);
            //AUDIO
            // GetComponent<AudioHandler>().PlayClip(1);
        } else if(player.GetComponent<Character>().type == 1) {
            playerType.text = "Human";
            playerType.color = new Color(0.9215687f, 0.9294118f, 0.9137256f, 0.282353f);
            //AUDIO
            // GetComponent<AudioHandler>().PlayClip(0);
        } else {
            playerType.text = "Spectator";
            playerType.color = new Color(0.9215687f, 0.9294118f, 0.9137256f, 0.282353f);
        }
    }
}
