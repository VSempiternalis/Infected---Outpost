using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class GameMaster : MonoBehaviour {
    public static GameMaster current;
    [SerializeField] private List<GameObject> activateOnStart;

    public int infectCooldownTime;

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

    public void OnClickExit() {
        print("EXITING TO MAIN MENU");
        PhotonNetwork.LoadLevel("SCENE - MainMenu");
        // PhotonNetwor
    }
}
