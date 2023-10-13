using UnityEngine;
using Photon.Pun;
using System.Net.NetworkInformation;

public class Character : MonoBehaviourPunCallbacks {
    public GameObject lightView;
    public Transform campos;
    private Rigidbody rb;
    public bool isPlayer;

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
    [Header("HEAD AND BODY")]
    [SerializeField] private Transform head;
    [SerializeField] private float headSpeed;
    private Quaternion targetHeadRot;
    [SerializeField] private Transform body;

    [Space(10)]
    [Header("ITEMS")]
    public ItemHandler onHandItem;
    public Transform hand;

    [Space(10)]
    [Header("INFECTED")]
    [SerializeField] private int infectCooldownOnStart;
    [SerializeField] private float infectCooldown;

    private void Start() {
        // SetPanels();
        rb = GetComponent<Rigidbody>();
        infectCooldownOnStart = GameMaster.current.infectCooldownTime;
    }

    private void Update() {
        if(type != 0 || !isPlayer) return;

        // print(name + "modifying infect cooldown: " + Mathf.CeilToInt(infectCooldown));
        if(infectCooldown > 0) infectCooldown -= Time.deltaTime; // Use Time.deltaTime to make it decrease by 1 every second
        else infectCooldown = 0;

        uiManager.current.UpdateInfectCooldown(Mathf.RoundToInt(infectCooldown));
    }

    // [PunRPC] private void rpcSyncRotation(float yRot) {
    //     print(name + "RPC sync rot: " + yRot);
    //     head.rotation = Quaternion.Euler(0, yRot, 0);
    // }

    public void Infect(string targetName) {
        print("Attempting to infect");
        if(type != 0 || infectCooldown > 0) return;
        infectCooldown = infectCooldownOnStart;
        photonView.RPC("InfectRPC", RpcTarget.All, targetName);
    }

    [PunRPC] private void InfectRPC(string targetName) {
        Character target = GameObject.Find(targetName).GetComponent<Character>();
        // if(target.type == 0) return;
        target.Kill();
    }

    public void Kill() { //Kill this character
        print("Killing: " + name);

        if(GetComponent<aiInput>()) GetComponent<aiInput>().enabled = false;
        else if(Controller.current.character == this) {
            print("local player is dead");
            Controller.current.SetToSpectator();
            SetPlayerType(2);

            Spawner.current.lights.SetActive(true);
        } 

        body.transform.Rotate(new Vector3(-90, 0, 0));
        body.transform.localPosition = new Vector3(0, 0.15f, 0);
        //use dead animation
        WinManager.current.AddKill(type);
        //sfx
    }

    public void DropItem(Vector3 dropPos) {
        photonView.RPC("DropItemRPC", RpcTarget.All, dropPos.x, dropPos.y, dropPos.z);
    }

    [PunRPC] private void DropItemRPC(float xPos, float yPos, float zPos) {;
        onHandItem.SetParent(null, new Vector3(xPos, yPos, zPos));
        onHandItem.isOwned = false;
        // onHandItem.transform.SetParent(null);
        // onHandItem.transform.position = new Vector3(xPos, yPos, zPos);
        // onHandItem.GetComponent<Rigidbody>().isKinematic = false;
        // onHandItem.GetComponent<BoxCollider>().isTrigger = false;
        onHandItem = null;
    }

    public void TakeItem(ItemHandler item) {
        print("Taking item " + item.name);
        if(item.isOwned) return;
        photonView.RPC("TakeItemRPC", RpcTarget.All, item.name);
    }

    [PunRPC] private void TakeItemRPC(string itemName) {
        ItemHandler item = GameObject.Find(itemName).GetComponent<ItemHandler>();
        print("Taking item rpc: " + itemName + ". item: " + item);
        onHandItem = item;
        onHandItem.SetParent(hand, Vector3.zero);
        onHandItem.isOwned = true;
        // item.transform.SetParent(hand);
        // item.transform.localPosition = Vector3.zero;
        // item.GetComponent<Rigidbody>().isKinematic = true;
        // onHandItem.GetComponent<BoxCollider>().isTrigger = true;

        // item.SetIsOwned(true);
    }

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

    public void SetIsPlayer() {
        photonView.RPC("SetIsPlayerRPC", RpcTarget.All);
    }

    [PunRPC] private void SetIsPlayerRPC() {
        isPlayer = true;
    }

    public void SetPlayerType(int newType) { //0 - Infected, 1 - Human, 2 - Spectator
        print("Set player type: " + newType);
        type = newType;
        if(type == 0) infectCooldown = infectCooldownOnStart;
        uiManager.current.SetPanels(newType);

        // photonView.RPC("SetPlayerTypeRPC", RpcTarget.All, type);
    }

    // [PunRPC] private void SetPlayerTypeRPC(int newType) {
    //     print(name + " Setting Player Type: " + newType);
    //     type = newType;
    // }

    public void RemoveAI() {
        Destroy(GetComponent<aiInput>());
    }
}
