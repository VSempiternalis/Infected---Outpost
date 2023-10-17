using UnityEngine;
using Photon.Pun;

public class DoorHandler : MonoBehaviourPunCallbacks {
    public string state; //open, closed, opening, closing
    [SerializeField] private Transform pivot;
    [SerializeField] private float rotationSpeed; 
    [SerializeField] private int openXRot = 0; //60
    [SerializeField] private int closedXRot = 0; //0
    [SerializeField] private int openYRot = 0; //60
    [SerializeField] private int closedYRot = 0; //0
    [SerializeField] private int openZRot = 0; //10
    [SerializeField] private int closedZRot = 0; //5
    // private float initialXRot = -90;

    private float limit = 1f;


    [Space(10)]
    [Header("AUDIO")]
    private AudioHandler ah;
    
    private void Start() {
        // initialXRot = pivot.transform.localRotation.eulerAngles.x;
        ah = GetComponent<AudioHandler>();
    }

    private void Update() {
        if(state == "Closing") {
            if(pivot.transform.localRotation.eulerAngles.y >= closedYRot - limit && pivot.transform.localRotation.eulerAngles.y <= closedYRot + limit) {
                state = "Closed";
                pivot.transform.localRotation = Quaternion.Euler(closedXRot, closedYRot, closedZRot);
            } else {
                pivot.transform.localRotation = Quaternion.Slerp(pivot.transform.localRotation, Quaternion.Euler(closedXRot, closedYRot, closedZRot), rotationSpeed*Time.deltaTime);
            }
        } else if(state == "Opening") {
            if(pivot.transform.localRotation.eulerAngles.y >= openYRot - limit && pivot.transform.localRotation.eulerAngles.y <= openYRot + limit) {
                state = "Open";
                pivot.transform.localRotation = Quaternion.Euler(openXRot, openYRot, openZRot);
            } else {
                pivot.transform.localRotation = Quaternion.Slerp(pivot.transform.localRotation, Quaternion.Euler(openXRot, openYRot, openZRot), rotationSpeed*Time.deltaTime);
            }
        }
    }

    public void ChangeState() {
        // print("changing door state");
        photonView.RPC("ChangeStateRPC", RpcTarget.All, state);
    }

    // public void OnClick(GameObject item) { //item on player when clicked
    //     if(isLocked && item.name != keyName) {
    //         //play lock sfx
    //         return;
    //     }

    //     photonView.RPC("ChangeStateRPC", RpcTarget.All);
    // }

    [PunRPC] public void ChangeStateRPC(string currentState) {
        if(currentState == "Open") {
            state = "Closing";
            ah.Play(0);
        } else if(currentState == "Closed") {
            state = "Opening";
            ah.Play(1);
        } 
        // else if(state == "Opening" && pivot.transform.localRotation.eulerAngles.y > openYRot) {
        //     state = "Closing";
        //     // audioHandler.Play(0);
        // } else if(state == "Closing" && pivot.transform.localRotation.eulerAngles.y < closedYRot) {
        //     state = "Opening";
        //     // audioHandler.Play(1);
        // }
    }

    public string GetHeader() {
        return "DOOR";
    }

    public string GetText() {
        return "Click to open/close";
    }
}
