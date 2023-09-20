using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour {
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button continueButton;

    public string displayName;

    private const string playerPrefsNameKey = "PlayerName";

    private void Start() {
        SetUpInputField();
    }

    private void Update() {
        
    }

    private void SetUpInputField() {
        if(!PlayerPrefs.HasKey(playerPrefsNameKey)) return;

        string defaultName = PlayerPrefs.GetString(playerPrefsNameKey);
        nameInput.text = defaultName;
        SetPlayerName(defaultName);
    }

    public void SetPlayerName(string name) {
        continueButton.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName() {
        displayName = nameInput.text;
        PlayerPrefs.SetString(playerPrefsNameKey, displayName);
    }
}
