using TMPro;
using UnityEngine;

public class DebugManager : MonoBehaviour {
    public static DebugManager current;

    [SerializeField] private Transform debugList;
    [SerializeField] private GameObject debugPF;

    private void Awake() {
        current = this;
    }

    private void Start() {
        NewDebug("Starting Console...");
    }

    private void Update() {
        // NewDebug("Updating...");
    }

    public void NewDebug(string newText) {
        GameObject newDebug = Instantiate(debugPF, debugList);
        // GameObject newDebug = Instantiate(debugPF);
        // newDebug.transform.SetParent(debugList);

        newDebug.transform.GetChild(0).GetComponent<TMP_Text>().text = "[" + Mathf.Round(Time.time) + "] " + newText;
        newDebug.SetActive(true);
    }
}
