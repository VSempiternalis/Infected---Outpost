using UnityEngine;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {
    public static GameMaster current;
    [SerializeField] private List<GameObject> activateOnStart;

    private void Awake() {
        current = this;
    }

    private void Start() {
        foreach(GameObject thing in activateOnStart) {
            thing.SetActive(true);
        }
    }

    private void Update() {
        
    }
}
