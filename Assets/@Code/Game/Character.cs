using UnityEngine;
using Photon.Pun;
using Unity.VisualScripting;

public class Character : MonoBehaviourPunCallbacks {
    public GameObject lightView;
    public Transform campos;
    private Rigidbody rb;
    private bool isAlive = true;
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
    [SerializeField] private int infectCooldownLength;
    [SerializeField] private float infectCooldown;

    [Space(10)]
    [Header("ANIMATION")]
    // private string currentState;
    private Animator anim;
    public bool isAiming;
    public bool isShooting;

    private void Start() {
        // SetPanels();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        infectCooldownLength = GameMaster.current.infectCooldownTime;
        infectCooldown = 0;
    }

    private void Update() {
        //ANIMATION
        if(!isPlayer && PhotonNetwork.IsMasterClient) aiAnimCheck();
        else if(isPlayer) AnimCheck();

        if(type != 0 || !isPlayer) return;

        // print(name + "modifying infect cooldown: " + Mathf.CeilToInt(infectCooldown));
        if(infectCooldown > 0) infectCooldown -= Time.deltaTime; // Use Time.deltaTime to make it decrease by 1 every second
        else infectCooldown = 0;

        uiManager.current.UpdateInfectCooldown(Mathf.RoundToInt(infectCooldown));
    }

    private void FixedUpdate() {
        if(!isAlive) {
            if(body.transform.localPosition.y > -1.25f) {
                print("ooh: " + body.transform.localPosition.y);
                body.transform.localPosition = new Vector3(0, body.transform.localPosition.y - 0.025f, 0);
            } 
            else body.transform.localPosition = new Vector3(0, -1.25f, 0);
        } 
    }

    private void aiAnimCheck() {
        // if(isPlayer && Controller.current.character != this) return;

        // print("Anim check: " + Time.time);
        // if(isPlayer) print("velocity: " + rb.velocity.magnitude);

        // if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;
        // else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Shooting") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;

        if(!isAlive) ChangeAnimState("Dying");
        // else if(isShooting) ChangeAnimState("Shooting");
        // else if(isAiming) ChangeAnimState("IdlePistol");
        // else if(rb.velocity.magnitude > 10f) ChangeAnimState("Run");
        // else if(rb.velocity.magnitude > 0.1f && onHandItem != null) ChangeAnimState("WalkWithItem");
        else if(rb.velocity.magnitude > 0.1f) ChangeAnimState("Walk");
        else ChangeAnimState("Idle");
    }

    private void AnimCheck() {
        // if(!isPlayer && !PhotonNetwork.IsMasterClient) return;
        // if(!isAlive) {
        //     ChangeAnimState("Dying");
        //     return;
        // }

        if(isPlayer && Controller.current.character != this) return;

        // print("Anim check: " + Time.time);
        // if(isPlayer) print("velocity: " + rb.velocity.magnitude);

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;
        else if(anim.GetCurrentAnimatorStateInfo(0).IsName("Shooting") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return;

        if(!isAlive) ChangeAnimState("Dying");
        else if(isShooting) ChangeAnimState("Shooting");
        else if(isAiming) ChangeAnimState("IdlePistol");
        else if(rb.velocity.magnitude > 3f) ChangeAnimState("Run");
        else if(rb.velocity.magnitude > 0.1f && onHandItem != null) ChangeAnimState("WalkWithItem");
        else if(rb.velocity.magnitude > 0.1f) ChangeAnimState("Walk");
        else ChangeAnimState("Idle");
    }

    private void ChangeAnimState(string newState) {
        photonView.RPC("ChangeAnimStateRPC", RpcTarget.All, newState);
    }

    [PunRPC] private void ChangeAnimStateRPC(string newState) {
        if(!anim) return;

        // if(newState == "Dying") print(name + " is dying");
        if(anim.GetCurrentAnimatorStateInfo(0).IsName(newState)) return;
        anim.Play(newState);
        // currentState = newState;
    }

    // [PunRPC] private void rpcSyncRotation(float yRot) {
    //     print(name + "RPC sync rot: " + yRot);
    //     head.rotation = Quaternion.Euler(0, yRot, 0);
    // }

    public void Infect(string targetName) {
        print("Attempting to infect: " + Time.time);
        if(type != 0 || infectCooldown > 0 || targetName == name) return;
        infectCooldown = infectCooldownLength;
        ChangeAnimState("Attack");
        //sfx
        photonView.RPC("InfectRPC", RpcTarget.All, targetName);
    }

    [PunRPC] private void InfectRPC(string targetName) {
        Character target = GameObject.Find(targetName).GetComponent<Character>();
        // if(target.type == 0) return;

        //WEAR THEIR SKIN
        GameObject mimic = Instantiate(target.body.gameObject, transform);

        // Set the position and rotation of the copy to match the current character's body
        mimic.transform.localPosition = Vector3.zero; //body.transform.localPosition;
        mimic.transform.rotation = body.transform.rotation;

        // Destroy the current character's body
        Destroy(body.gameObject);

        // Set the copy as the new body for the current character
        body = mimic.transform;

        //WEAR THEIR HEAD
        head = mimic.transform.GetChild(2);

        //USE LIGHT
        head.GetChild(0).gameObject.SetActive(true);

        target.Kill();
    }

    public void Kill() { //Kill this character
        print("Killing: " + name);
        isAlive = false;

        if(GetComponent<aiInput>()) GetComponent<aiInput>().enabled = false;
        else if(Controller.current.character == this) {
            print("local player is dead");
            Controller.current.SetToSpectator();
            SetPlayerType(2);

            Spawner.current.lights.SetActive(true);
        } 

        // body.transform.Rotate(new Vector3(-90, 0, 0));
        // body.transform.localPosition = new Vector3(0, -1.25f, 0);
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

        MoveHead(movePos);
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
        if(type == 0) infectCooldown = 0;
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
