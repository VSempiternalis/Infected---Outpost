using UnityEngine;
using Photon.Pun;
using System.Net.NetworkInformation;

public class Character : MonoBehaviourPunCallbacks {
    public GameObject lightView;
    public Transform campos;
    private Rigidbody rb;

    public int spawnInt;
    // private string type;
    // public bool isInfected;
    public int type; //0 - Infected, 1 - Human, 2 - Spectator

    // [Space(10)]
    // [Header("UI")]
    // private GameObject infectedUI;
    // private GameObject humanUI;
    // private GameObject spectatorUI;
    // private RectTransform staminaImg;

    [Space(10)]
    [Header("STATS")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier;
    private Vector3 currentVelocity;

    [Space(10)]
    [Header("HEAD")]
    [SerializeField] private Transform head;
    [SerializeField] private float headSpeed;
    private Quaternion targetHeadRot;

    private void Start() {
        // SetPanels();
        rb = GetComponent<Rigidbody>();
    }

    // private void Update() {
    //     if(photonView.IsMine) rpcSyncRotation(head.rotation.y);
    // }

    // [PunRPC] private void rpcSyncRotation(float yRot) {
    //     print(name + "RPC sync rot: " + yRot);
    //     head.rotation = Quaternion.Euler(0, yRot, 0);
    // }

    [PunRPC] private void rpcMoveHead(Vector3 lookPos) {
        // if(photonView.IsMine) {
            // print(name + "RPC movehead: ");

            if(rb == null) return;
            // Calculate the direction from the head to the pointer, disregarding the Y-axis
            Vector3 lookDirection = (lookPos - head.position);
            if(lookDirection == Vector3.zero) return;
            lookDirection.y = 0; // Disregard the Y-axis component

            // Calculate the target rotation
            targetHeadRot = Quaternion.LookRotation(lookDirection);

            // Rotate the head towards the target rotation with a delay
            head.rotation = Quaternion.Slerp(head.rotation, targetHeadRot, headSpeed * Time.deltaTime);
            // print(name + " rotation: " + head.rotation.eulerAngles);
        // }
    }

    public void MoveHead(Vector3 lookPos) {
        // print(name + " rpcMoveHead");
        photonView.RPC("rpcMoveHead", RpcTarget.All, lookPos);
        // rpcMoveHead(lookPos);
        // if(rb == null) return;

        // // Calculate the direction from the head to the pointer, disregarding the Y-axis
        // Vector3 lookDirection = (lookPos - head.position);
        // lookDirection.y = 0; // Disregard the Y-axis component
        // if(lookDirection == Vector3.zero) return;

        // // Calculate the target rotation
        // targetHeadRot = Quaternion.LookRotation(lookDirection);

        // // Rotate the head towards the target rotation with a delay
        // head.rotation = Quaternion.Slerp(head.rotation, targetHeadRot, headSpeed * Time.deltaTime);
    }

    public void MoveTo(Vector3 movePos) {
        if(rb == null) return;

        // Calculate the diagonal direction by combining input along both the X and Z axes.
        Vector3 moveDirection = movePos - transform.position;

        if (moveDirection != Vector3.zero) {
            moveDirection.Normalize();

            // Rotate the movement direction to match your isometric view (e.g., 45 degrees).
            // moveDirection = Quaternion.Euler(0, 45, 0) * moveDirection;

            // Apply the movement with adjusted speed
            Vector3 targetVelocity = moveDirection * (moveSpeed * sprintMultiplier);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 0.1f);
        } else {
            // If no input is detected, gradually slow down the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref currentVelocity, 0.1f);
        }
    }

    public void MoveWASD(Vector3 input) {
        if(rb == null) return;

        // Calculate the diagonal direction by combining input along both the X and Z axes.
        Vector3 moveDirection = new Vector3(input.x, 0, input.z);//.normalized;

        // Check if there's any input
        if (moveDirection != Vector3.zero) {
            // Rotate the movement direction to match your isometric view (e.g., 45 degrees).
            moveDirection = Quaternion.Euler(0, 45, 0) * moveDirection;

            // Apply the movement with adjusted speed
            Vector3 targetVelocity = moveDirection * (moveSpeed * sprintMultiplier);
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 0.1f);
        } else {
            // If no input is detected, gradually slow down the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref currentVelocity, 0.1f);
        }
    }

    public void SetPlayerType(int newType) {
        print(name + " Setting Player Type: " + newType);
        type = newType;

        uiManager.current.SetPanels(newType);
    }

    public void RemoveAI() {
        Destroy(GetComponent<aiInput>());
    }
}
