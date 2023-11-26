using UnityEngine;
using TMPro;
using System;

public class Keybind : MonoBehaviour {
    [SerializeField] private string keyName;
    [SerializeField] private string defaultValue;
    [SerializeField] private TMP_Text buttonText;

    private void Start() {
        buttonText = transform.GetChild(0).GetComponent<TMP_Text>();
        buttonText.text = PlayerPrefs.GetString(keyName, defaultValue);
    }

    private void Update() {
        if(buttonText.text == "---") {
            foreach(KeyCode keycode in Enum.GetValues(typeof(KeyCode))) {
                if(Input.GetKey(keycode)) {
                    buttonText.text = keycode.ToString();
                    PlayerPrefs.SetString(keyName, keycode.ToString());
                    // KeybindsManager.current.KeyChanged();
                }
            }
        }
    }

    public void SetKey() {
        buttonText.text = "---";
    }
}