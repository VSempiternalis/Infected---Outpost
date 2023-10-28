using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class GameMaster : MonoBehaviour {
    public static GameMaster current;
    [SerializeField] private List<GameObject> activateOnStart;

    public int infectCooldownTime;
    public int revolverBulletCountOnStart;
    public int flareTimeLeftOnStart;

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
    
    public void OnClickLeaveAndExit() {
        print("LEAVE AND EXIT");
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("SCENE - MainMenu");
        // PhotonNetwor
    }

    public void OnClickExit() {
        print("EXITING TO MAIN MENU");
        PhotonNetwork.LoadLevel("SCENE - MainMenu");
        // PhotonNetwor
    }
}
