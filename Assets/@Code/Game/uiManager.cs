using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    [Space(10)]
    [Header("VFX")]
    [SerializeField] private Volume volume;
    private Vignette vignette;
    [SerializeField] private VolumeProfile infectedVP;
    [SerializeField] private VolumeProfile humanVP;
    
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

    public void UpdateStaminaUI(int value, int maxValue) {
        float ratio = (float)value / (float)maxValue;
        // print(ratio);
        // staminaImg.sizeDelta = new Vector2(1920f*ratio, staminaImg.sizeDelta.y);
        staminaImg.localScale = new Vector2(ratio, staminaImg.localScale.y);
    }

    public void UpdateInfectCooldown(int newValue) {
        if(!infectCooldown.transform.parent.gameObject.activeSelf) infectCooldown.transform.parent.gameObject.SetActive(true);

        if(newValue > 0) {
            infectCooldown.text = "Resting... (" + newValue + ")";
            infectCooldown.fontSize = 24;
            infectCooldown.color = infectRestColor;

            //vfx
            vignette.intensity.value = 0.3f;
        } else {
            infectCooldown.text = "KILL!";
            infectCooldown.fontSize = 40;
            infectCooldown.color = infectColor;

            //vfx
            vignette.intensity.value = 0.5f;
        }
    }

    public void SetPanels(int type) {
        print("[CHARACTER] Setting panels. Type: " + type);
        volume.profile.TryGet(out vignette);

        if(type == 0) {
            playerType.text = "Infected";
            Color playerColor = new Color(0.647f, 0.188f, 0.188f, 1f);
            playerType.color = playerColor;
            staminaImg.GetComponent<UnityEngine.UI.Image>().color = playerColor;
            infectCooldown.gameObject.SetActive(true);
            //AUDIO
            // GetComponent<AudioHandler>().PlayClip(1);

            //vfx
            volume.profile = infectedVP;
        } else if(type == 1) {
            playerType.text = "Human";
            Color playerColor = new Color(0.923f, 0.929f, 0.914f, 1f);
            playerType.color = playerColor;
            staminaImg.GetComponent<UnityEngine.UI.Image>().color = playerColor;
            //AUDIO
            // GetComponent<AudioHandler>().PlayClip(0);

            //vfx
            volume.profile = humanVP;
        } else {
            playerType.text = "Spectator";
            Color playerColor = new Color(0.643f, 0.867f, 0.859f, 1f);
            playerType.color = playerColor;
            staminaImg.GetComponent<UnityEngine.UI.Image>().color = playerColor;
            infectCooldown.transform.parent.gameObject.SetActive(false);

            //vfx
            volume.profile = humanVP;
        }
    }

    public void ActivateTypePopupRPC(int type) {
        typePopup.SetActive(true);
        // Character player = Controller.current.character;

        //Activate type popper
        // playerType.transform.parent.gameObject.SetActive(true);
        SetPanels(type);
    }
}
