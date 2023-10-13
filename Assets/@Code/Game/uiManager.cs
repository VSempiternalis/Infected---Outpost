using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Microsoft.Unity.VisualStudio.Editor;

public class uiManager : MonoBehaviour {
    public static uiManager current;

    [Space(10)]
    [Header("UI")]
    public GameObject loadingPanel;
    [SerializeField] private TMP_Text playerType;
    [SerializeField] private GameObject typePopup;

    // [SerializeField] private GameObject infectedUI;
    // [SerializeField] private GameObject humanUI;
    // [SerializeField] private GameObject spectatorUI;
    // [SerializeField] private RectTransform infectedStaminaImg;
    // [SerializeField] private RectTransform humanStaminaImg;
    // [SerializeField] private RectTransform spectatorStaminaImg;
    [SerializeField] private RectTransform staminaImg;
    [SerializeField] private TMP_Text infectCooldown;
    [SerializeField] private Color infectRestColor;
    [SerializeField] private Color infectColor;
    
    private void Awake() {
        current = this;
        // loadingPanel.SetActive(true);
        typePopup.SetActive(false);
    }

    private void Start() {
        // infectedUI.SetActive(false);
        // humanUI.SetActive(false);
        // spectatorUI.SetActive(false);
    }

    private void Update() {
        
    }

    public void UpdateInfectCooldown(int newValue) {
        if(!infectCooldown.transform.parent.gameObject.activeSelf) infectCooldown.transform.parent.gameObject.SetActive(true);

        if(newValue > 0) {
            infectCooldown.text = "Resting... (" + newValue + ")";
            infectCooldown.fontSize = 24;
            infectCooldown.color = infectRestColor;
        } else {
            infectCooldown.text = "KILL!";
            infectCooldown.fontSize = 40;
            infectCooldown.color = infectColor;
        }
    }

    public void SetPanels(int type) {
        print("[CHARACTER] Setting panels. Type: " + type);

        if(type == 0) {
            playerType.text = "Infected";
            Color playerColor = new Color(0.647f, 0.188f, 0.188f, 1f);
            playerType.color = playerColor;
            staminaImg.GetComponent<UnityEngine.UI.Image>().color = playerColor;
            infectCooldown.gameObject.SetActive(true);
            //AUDIO
            // GetComponent<AudioHandler>().PlayClip(1);
        } else if(type == 1) {
            playerType.text = "Human";
            Color playerColor = new Color(0.923f, 0.929f, 0.914f, 1f);
            playerType.color = playerColor;
            staminaImg.GetComponent<UnityEngine.UI.Image>().color = playerColor;
            //AUDIO
            // GetComponent<AudioHandler>().PlayClip(0);
        } else {
            playerType.text = "Spectator";
            Color playerColor = new Color(0.643f, 0.867f, 0.859f, 1f);
            playerType.color = playerColor;
            staminaImg.GetComponent<UnityEngine.UI.Image>().color = playerColor;
            infectCooldown.transform.parent.gameObject.SetActive(false);
        }

        // Spawner.current.loadText.text = "Setting panels...";
        // infectedUI.SetActive(false);
        // humanUI.SetActive(false);
        // spectatorUI.SetActive(false);

        // if(type == 0) {
        //     infectedUI.SetActive(true);
        //     staminaImg = infectedStaminaImg;
        // } else if(type == 1) {
        //     humanUI.SetActive(true);
        //     staminaImg = humanStaminaImg;
        // } else if(type == 2) {
        //     spectatorUI.SetActive(true);
        //     staminaImg = spectatorStaminaImg;
        // }

        // loadingPanel.SetActive(false);
    }

    public void ActivateTypePopupRPC(int type) {
        typePopup.SetActive(true);
        // Character player = Controller.current.character;

        //Activate type popper
        // playerType.transform.parent.gameObject.SetActive(true);
        SetPanels(type);
    }
}
