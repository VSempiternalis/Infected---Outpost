using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class GameMaster : MonoBehaviour {
    public static GameMaster current;
    [SerializeField] private List<GameObject> activateOnStart;

    public static int infectCooldownTime;
    public static int revolverBulletCountOnStart;
    public static int flareTimeLeftOnStart;
    public static int countdownOnStart; //value of countdown on start
    public static int fuelTimeAdd; //num of secs fuel adds to countdown

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

    // public void SetGeneratorTime(int newVal) {
    //     photonView.RPC("SetGeneratorTimeRPC", RpcTarget.All, newVal);
    // }

    // public void SetInfectCooldownTime(int newVal) {
    //     photonView.RPC("SetInfectCooldownRPC", RpcTarget.All, newVal);
    // }
    
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
