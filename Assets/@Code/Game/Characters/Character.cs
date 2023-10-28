using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPunCallbacks { //, ITooltipable
    [SerializeField] private string characterName;
    [SerializeField] private string characterDesc;
    public GameObject lightView;
    public Transform campos;
    private Rigidbody rb;
    public bool isAlive = true;
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

    [Space(10)]
    [Header("AUDIO")]
    private AudioHandler ah; //0 - Infect, 1 - nil, 2 - Drop, 3 - Take, 4 - Scream
    [SerializeField] private MoveAudioHandler moveAH; //0 - Infect, 1 - Run, 2 - Drop, 3 - Take
    [SerializeField] private AudioHandler screamAH; //0 - Scream

    private void Start() {
        // SetPanels();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        infectCooldownLength = GameMaster.current.infectCooldownTime;
        infectCooldown = 0;
        ah = GetComponent<AudioHandler>();
    }

    private void Update() {
        //ANIMATION
        if(!isPlayer && PhotonNetwork.IsMasterClient) aiAnimCheck();
        else if(isPlayer) AnimCheck();

        if(type != 0 || !isPlayer || !photonView.AmOwner) return;

        if(infectCooldown > 0) infectCooldown -= Time.deltaTime; // Use Time.deltaTime to make it decrease by 1 every second
        else infectCooldown = 0;

        uiManager.current.UpdateInfectCooldown(Mathf.RoundToInt(infectCooldown));
    }

    private void FixedUpdate() {
        if(!isAlive) {
            if(body.transform.localPosition.y > -0.75f) {
                // print("ooh: " + body.transform.localPosition.y);
                body.transform.localPosition = new Vector3(0, body.transform.localPosition.y - 0.025f, 0);
            } 
            else body.transform.localPosition = new Vector3(0, -0.75f, 0);
        } 
    }

    private void aiAnimCheck() {
        if(!isAlive) ChangeAnimState("Dying");
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
        else if(rb.velocity.magnitude > 3.5f) ChangeAnimState("Run");
        else if(rb.velocity.magnitude > 0.1f && onHandItem != null) ChangeAnimState("WalkWithItem");
        else if(rb.velocity.magnitude > 0.1f) ChangeAnimState("Walk");
        else ChangeAnimState("Idle");
    }

    private void ChangeAnimState(string newState) {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName(newState)) return;
        
        photonView.RPC("ChangeAnimStateRPC", RpcTarget.All, newState);
    }

    [PunRPC] private void ChangeAnimStateRPC(string newState) {
        if(!anim) return;

        anim.Play(newState);

        //sfx
        if(newState == "Run") {
            moveAH.On();
            // moveAH.SetIsPlaying();
            // GetComponent<AudioSource>().loop = true;
        } else {
            moveAH.Off();
            // moveAH.isPlaying = "Null";
            // GetComponent<AudioSource>().Stop();
            // GetComponent<AudioSource>().loop = false;
        }
    }

    // [PunRPC] private void rpcSyncRotation(float yRot) {
    //     print(name + "RPC sync rot: " + yRot);
    //     head.rotation = Quaternion.Euler(0, yRot, 0);
    // }

    public void Infect(string targetName) {
        print("Attempting to infect: " + Time.time);
        if(type != 0 || infectCooldown > 0 || targetName == name || onHandItem != null) return;
        infectCooldown = infectCooldownLength;

        photonView.RPC("InfectRPC", RpcTarget.All, targetName);

        ChangeAnimState("Attack");
        
        //sfx
        ah.PlayOneShot(0);
    }

    [PunRPC] private void InfectRPC(string targetName) {
        print("Infect RPC");
        Character target = GameObject.Find(targetName).GetComponent<Character>();
        if(!target.isAlive) return;

        BodyHandler targetBody = target.body.GetComponent<BodyHandler>();

        //Turn old face off
        body.GetComponent<BodyHandler>().face.SetActive(false);

        //Turn new face on
        string tgtFaceName = targetBody.face.name;
        foreach(Transform face in body) {
            if(face.name == tgtFaceName) {
                face.gameObject.SetActive(true);
                body.GetComponent<BodyHandler>().face = face.gameObject;
            }
        }

        //Turn old hair off
        body.GetComponent<BodyHandler>().hair.SetActive(false);

        //Turn new hair on
        if(targetBody.hair) {
            string tgtHairName = targetBody.hair.name;
            foreach(Transform hair in body.GetComponent<BodyHandler>().actualHead) {
                if(hair.name == tgtHairName) {
                    hair.gameObject.SetActive(true);
                    body.GetComponent<BodyHandler>().hair = hair.gameObject;
                }
            }
        }

        //USE LIGHT
        // lightView = targetBody.lightView;
        // lightView.SetActive(true);

        target.Kill();

        //sfx
        print("infected sfx");
        ah.PlayOneShot(0);
    }

    public void Kill() { //Kill this character
        print("Killing: " + name);
        isAlive = false;

        if(isPlayer) WinManager.current.AddKill(type);

        if(GetComponent<aiInput>()) GetComponent<aiInput>().enabled = false;
        else if(Controller.current.character == this) {
            print("local player is dead");
            Controller.current.SetToSpectator();
            SetPlayerType(2);

            Spawner.current.lights.SetActive(true);
        }

        //Drop item
        if(onHandItem) DropItem(transform.position);

        // body.transform.Rotate(new Vector3(-90, 0, 0));
        // body.transform.localPosition = new Vector3(0, -1.25f, 0);
        //use dead animation

        //deactivate collider
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<BoxCollider>().enabled = true;

        //sfx
        screamAH.PlayOneShot(0);
    }

    public void DropItem(Vector3 dropPos) {
        photonView.RPC("DropItemRPC", RpcTarget.All, dropPos.x, dropPos.y, dropPos.z);
    }

    [PunRPC] private void DropItemRPC(float xPos, float yPos, float zPos) {
        if(onHandItem.GetComponent<Outline>()) onHandItem.GetComponent<Outline>().OutlineWidth = 3;
        onHandItem.SetParent(null, new Vector3(xPos, yPos, zPos));
        // onHandItem.isOwned = false;
        // onHandItem.transform.SetParent(null);
        // onHandItem.transform.position = new Vector3(xPos, yPos, zPos);
        // onHandItem.GetComponent<Rigidbody>().isKinematic = false;
        // onHandItem.GetComponent<BoxCollider>().isTrigger = false;
        onHandItem = null;

        //sfx
        ah.PlayOneShot(2);
    }

    public void TakeItem(ItemHandler item) {
        print("Taking item " + item.name);
        if(item.transform.parent && item.transform.parent.GetComponent<Character>()) {
            print("ITEM IS OWNED BY: " + item.transform.parent.name);
            return;
        } 
        photonView.RPC("TakeItemRPC", RpcTarget.All, item.name);
    }

    [PunRPC] private void TakeItemRPC(string itemName) {
        ItemHandler item = GameObject.Find(itemName).GetComponent<ItemHandler>();
        print("Taking item rpc: " + itemName + ". item: " + item);
        onHandItem = item;
        if(onHandItem.GetComponent<Outline>()) onHandItem.GetComponent<Outline>().OutlineWidth = 0;
        onHandItem.SetParent(hand, Vector3.zero);
        // onHandItem.isOwned = true;
        // item.transform.SetParent(hand);
        // item.transform.localPosition = Vector3.zero;
        // item.GetComponent<Rigidbody>().isKinematic = true;
        // onHandItem.GetComponent<BoxCollider>().isTrigger = true;

        // item.SetIsOwned(true);

        //sfx
        ah.PlayOneShot(3);
    }

    public void MoveHead(Vector3 lookPos) {
        MovingTheHead(lookPos);
        // photonView.RPC("rpcMoveHead", RpcTarget.All, lookPos);
    }

    [PunRPC] private void rpcMoveHead(Vector3 lookPos) {
        MovingTheHead(lookPos);
    }

    private void MovingTheHead(Vector3 lookPos) {
        // print("Moving head: " + head);
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
            Vector3 targetVelocity = moveDirection * (moveSpeed * sprintMultiplier) * Time.deltaTime;
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
            //Normalize direction when not running
            if (Mathf.Abs(input.x) <= 1 && Mathf.Abs(input.z) <= 1) moveDirection.Normalize();
            
            // Rotate the movement direction to match your isometric view (e.g., 45 degrees).
            moveDirection = Quaternion.Euler(0, 45, 0) * moveDirection;

            // Apply the movement with adjusted speed
            Vector3 targetVelocity = moveDirection * (moveSpeed * sprintMultiplier)  * Time.deltaTime;
            rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref currentVelocity, 0.1f);
        } else {
            // If no input is detected, gradually slow down the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, Vector3.zero, ref currentVelocity, 0.1f);
        }
    }

    public void SetIsPlayer() {
        print(name + "SetIsPlayer (Local)");
        photonView.RPC("SetIsPlayerRPC", RpcTarget.All);
    }

    [PunRPC] private void SetIsPlayerRPC() {
        print(name + "SetIsPlayerRPC");
        isPlayer = true;
    }

    public void SetPlayerType(int newType) { //0 - Infected, 1 - Human, 2 - Spectator
        print("Set player type: " + newType);
        photonView.RPC("SetPlayerTypeRPC", RpcTarget.All, newType);
    }

    [PunRPC] private void SetPlayerTypeRPC(int newType) {
        print(name + " Setting Player Type rpc: " + newType);
        type = newType;

        Spawner.current.AddTypeSet();

        if(!photonView.IsMine) return;

        if(type == 0) {
            infectCooldown = 0;
            AudioManager.current.PlayAmb(0);
        } else {
            AudioManager.current.PlayAmb(1);
        }

        uiManager.current.SetPanels(newType);
    }

    public void RemoveAI() {
        Destroy(GetComponent<aiInput>());
    }

    //

    // public string GetHeader() {
    //     return characterName;
    // }

    // public string GetContent() {
    //     return characterDesc;
    // }
}
