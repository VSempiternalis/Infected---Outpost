using UnityEngine;
using System.Collections.Generic;
// using Photon.Voice.Unity;

public class Controller : MonoBehaviour {
    public static Controller current;
    public Character character;
    public int characterType;

    [Space(10)]
    [Header("STAMINA")]
    // private bool isRunning = false;
    private int stamina;
    private int staminaMax = 360;
    [SerializeField] private int staminaGain = 1;
    [SerializeField] private int staminaLoss = 1;

    [Space(10)]
    [Header("MOVEMENT")]
    [SerializeField] private Rigidbody player;
    [SerializeField] private float moveSpeed;
    
    [SerializeField] private float sprintMultiplier = 2f; // Speed multiplier when sprinting
    private Vector3 input;
    private Vector3 currentVelocity; // Current velocity of the character

    [Space(10)]
    [Header("POINTER")]
    [SerializeField] private Transform pointer;
    private Camera mainCamera;

    [Space(10)]
    [Header("SELECTION")]
    [SerializeField] private float reachDist; //how far the player can reach
    private GameObject pointed; //object player cursor is pointing at

    [Space(10)]
    [Header("SPECTATOR")]
    // public bool isSpectator;
    public List<Character> playerList;
    private int followInt;

    [Space(10)]
    [Header("VOICE CHAT")]
    // [SerializeField] private Recorder recorder;
    [SerializeField] private GameObject speakerIMG;

    [Space(10)]
    // [Header("INFECTED")]
    // private bool isInfected;
    [SerializeField] private Follower playerDot; //Dot that follows player pos in map

    [Space(10)]
    [Header("KEYBINDS")]
    private KeyCode Key_MoveUp;
    private KeyCode Key_MoveDown;
    private KeyCode Key_MoveLeft;
    private KeyCode Key_MoveRight;
    private KeyCode Key_ToggleMap;
    private KeyCode Key_Run;
    private KeyCode Key_UseItem;
    private KeyCode Key_GrabDropModifier;
    // private KeyCode Key_Aim;
    private KeyCode Key_ProxChat;

    [SerializeField] private AudioListener oldAudioListener;

    private void Awake() {
        current = this;
    }

    private void Start() {
        mainCamera = Camera.main;

        stamina = staminaMax;

        GetKeybinds();
    }

    private void Update() {
        if(player == null || !WinManager.current.gameStart) return;

        else if(characterType == 2) GetSpectatorInput();

        else {
            GetInput();
            MovePointer();
            MoveHead();
        }

        // if(Input.GetKeyDown(KeyCode.P)) WinManager.current.Endgame("Escape");
    }

    private void FixedUpdate() {
        if(player == null) return;
        
        Movement();
    }

    private void GetKeybinds() {
        Key_MoveUp = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_MoveUp", "W"));
        Key_MoveDown = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_MoveDown", "S"));
        Key_MoveLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_MoveLeft", "A"));
        Key_MoveRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_MoveRight", "D"));
        Key_ToggleMap = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_ToggleMap", "Tab"));
        Key_Run = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_Run", "LeftShift"));
        Key_UseItem = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_UseItem", "Mouse0"));
        Key_GrabDropModifier = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_GrabDropModifier", "LeftControl"));
        Key_ProxChat = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Key_ProxChat", "Space"));
    }

    private void GetSpectatorInput() {
        if(Input.GetKeyDown(Key_MoveLeft)) {
            print("SPECTATOR A");
            if(followInt <= 0) followInt = playerList.Count - 1;
            else followInt --;
        
            SetCamera(playerList[followInt].GetComponent<Rigidbody>());
            uiManager.current.SetFollowing(playerList[followInt].photonView.Owner.NickName, playerList[followInt].GetComponent<Character>().type);
        } else if(Input.GetKeyDown(Key_MoveRight)) {
            print("SPECTATOR D");
            if(followInt >= (playerList.Count - 1)) followInt = 0;
            else followInt ++;
        
            SetCamera(playerList[followInt].GetComponent<Rigidbody>());
            uiManager.current.SetFollowing(playerList[followInt].photonView.Owner.NickName, playerList[followInt].GetComponent<Character>().type);
        }

        if(Input.GetKeyDown(KeyCode.Escape)) uiManager.current.ToggleEsc();

        if(Input.GetKeyDown(Key_ToggleMap)) uiManager.current.ToggleMap();
    }

    public void SetToSpectator() {
        characterType = 2;
        followInt = 0;
    }

    public void SetPlayer(Rigidbody newPlayer) {
        player = newPlayer;
        character = newPlayer.GetComponent<Character>();
        playerDot.toFollow = player.transform;

        oldAudioListener.gameObject.SetActive(false);
        player.gameObject.AddComponent<AudioListener>();

        // characterType = character.type;
        // print("Setting player: " + character.name + ". Type: " + characterType);
        // agent = player.GetComponent<NavMeshAgent>();
        // head = player.GetComponent<Character>().head;
        SetCamera(newPlayer);
    }

    private void SetCamera(Rigidbody parent) {
        print("Setting camera to " + name);
        // mainCamera.transform.parent.position = parent.GetComponent<Character>().campos.position;
        mainCamera.transform.parent.GetComponent<SlowFollower>().toFollow = parent.GetComponent<Character>().campos;
    }

    private void MovePointer() {
        if(pointed && !pointed.GetComponent<ItemHandler>() && pointed.GetComponent<Outline>()) pointed.GetComponent<Outline>().OutlineWidth = 0;
        TooltipSystem.Hide();

        // if(pointed) print("Pointed: " + pointed + " has outline: " + (pointed.GetComponent<Outline>()? "Yes":"No"));
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a raycast to see if it hits something in the game world
        if (Physics.Raycast(ray, out hit)) {
            bool isWithinReach = Vector3.Distance(character.transform.position, hit.point) <= reachDist;

            //Move the pointer
            pointer.position = hit.point;

            //Set pointed
            pointed = hit.collider.gameObject;
            // print(pointed.name);

            //Outline
            if(!pointed.GetComponent<ItemHandler>() && pointed.GetComponent<Outline>() && isWithinReach) {
                pointed.GetComponent<Outline>().OutlineWidth = 3; //3 is standard outline width
            }

            //INTERACTION
            //Aiming and unaiming
            if(Input.GetMouseButton(1)) { //Holding right click
                if(character.onHandItem && character.onHandItem.GetComponent<IAimable>() != null) {
                    character.onHandItem.GetComponent<IAimable>().Aim(hit.point);
                    character.isAiming = true;

                    //Shooting
                    if(Input.GetMouseButtonDown(0)) {
                        character.onHandItem.GetComponent<IUsable>().Use(pointed, character.type);
                    }
                } 
            } else if(Input.GetMouseButtonUp(1)) { //Releasing right click
                if(character.onHandItem && character.onHandItem.GetComponent<IAimable>() != null) {
                    character.onHandItem.GetComponent<IAimable>().UnAim();
                    character.isAiming = false;
                }
            } //FLARE
            else if(Input.GetMouseButtonDown(0) && character.onHandItem && character.onHandItem.GetComponent<Flare>() && !character.onHandItem.GetComponent<Flare>().isOn && !Input.GetKey(Key_GrabDropModifier) && !pointed.GetComponent<Storage>()) {
                character.onHandItem.GetComponent<IUsable>().Use(pointed, character.type);
            }
            
            //Pointed is within reach
            else if(Vector3.Distance(character.transform.position, hit.point) <= reachDist) {
                if(Input.GetMouseButtonDown(0)) {
                    if(pointed.GetComponent<IClickable>() != null) {
                        if(Input.GetKey(Key_GrabDropModifier)) {
                            //Ctrl + Click item on ground
                            if(pointed.GetComponent<ItemHandler>() != null) {
                                //Switch items
                                if(character.onHandItem) {
                                    character.DropItem(hit.point);
                                } //Take item
                                character.TakeItem(pointed.GetComponent<ItemHandler>());
                            } 
                            //Ctrl + Click storage
                            else if(pointed.GetComponent<Storage>() != null) {
                                //Has door and is open/opening
                                if(pointed.GetComponent<DoorHandler>() && (pointed.GetComponent<DoorHandler>().state == "Open" || pointed.GetComponent<DoorHandler>().state == "Opening")) {
                                    StorageInteraction(pointed.GetComponent<Storage>());
                                }
                                //No door 
                                else if(pointed.GetComponent<DoorHandler>() == null) {
                                    StorageInteraction(pointed.GetComponent<Storage>());
                                }
                            }
                        } 
                        //Click on clickable: interact
                        else if(character.onHandItem) {
                            if(pointed.GetComponent<Storage>() && !pointed.GetComponent<Storage>().isLocked) pointed.GetComponent<IClickable>().OnClick();
                            else character.onHandItem.GetComponent<IUsable>().Use(pointed, character.type);
                        }
                        else if(pointed.GetComponent<Storage>() && pointed.GetComponent<Storage>().isLocked) AudioManager.current.PlayUI(0);
                        else pointed.GetComponent<IClickable>().OnClick();
                    }
                    //Infected kills target
                    else if(pointed.GetComponent<Character>() && pointed.GetComponent<Character>().type == 1) {
                        character.Infect(pointed.name);
                    }
                    //Drop item on floor
                    else if(pointed.GetComponent<IClickable>() == null && pointed.layer != 9) {
                        if(Input.GetKey(Key_GrabDropModifier) && Vector3.Distance(character.transform.position, hit.point) <= reachDist && character.onHandItem != null) {
                            character.DropItem(hit.point);
                        }
                    }

                    //Switch item from storage      
                    else if(pointed.GetComponent<Storage>()) {
                        ItemHandler oldItem = null;
                        if(pointed.GetComponent<Storage>().item) oldItem = pointed.GetComponent<Storage>().item;

                        character.TakeItem(pointed.GetComponent<Storage>().item);
                        pointed.GetComponent<Storage>().RemoveItem();

                        pointed.GetComponent<Storage>().InsertItem(oldItem);
                    }
                }

                //Tooltip
                if(pointed.GetComponent<ITooltipable>() != null) {
                    TooltipSystem.Show(pointed.GetComponent<ITooltipable>().GetHeader(), pointed.GetComponent<ITooltipable>().GetContent());
                }
            }
        }
    }

    private void StorageInteraction(Storage storage) {
        print("Storage interaction");
        if(character.onHandItem) {
            //Player: item, Storage: item
            if(storage.item) {
                print("SWITCHING! Player: item, Storage: item " + Time.time);
                ItemHandler oldItem = character.onHandItem;
                character.TakeItem(pointed.GetComponent<Storage>().item);
                pointed.GetComponent<Storage>().RemoveItem();

                oldItem.SetParent(storage.transform, Vector3.zero);
                storage.InsertItem(oldItem);
            } 

            //Player: item, Storage: no item
            else {
                print("STORING! Player: item, Storage: no item " + Time.time);
                character.onHandItem.SetParent(storage.transform, Vector3.zero);
                storage.InsertItem(character.onHandItem);
                if(character.onHandItem.GetComponent<Outline>()) character.onHandItem.GetComponent<Outline>().OutlineWidth = 3;

                //Outline on if gun
                if(character.onHandItem.GetComponent<Gun>()) character.onHandItem.GetComponent<Outline>().enabled = true;

                character.onHandItem = null;
            }
        }
        
        
        else {
            //Player: no item, Storage: item
            if(storage.item) {
                print("TAKING! Player: no item, Storage: item " + Time.time);
                character.TakeItem(pointed.GetComponent<Storage>().item);
                pointed.GetComponent<Storage>().RemoveItem();
            } 

            //no items
            else {

            }
        }
    }

    private void StoreItem(Storage storage) {
        ItemHandler oldItem = null;
        if(pointed.GetComponent<Storage>().item) oldItem = pointed.GetComponent<Storage>().item;
        character.TakeItem(pointed.GetComponent<Storage>().item);
        pointed.GetComponent<Storage>().RemoveItem();
        pointed.GetComponent<Storage>().InsertItem(oldItem);
    }

    private void MoveHead() {
        character.MoveHead(pointer.position);
    }

    private void GetInput() {
        //No move while aiming
        if(character.onHandItem && character.onHandItem.GetComponent<Gun>() && Input.GetMouseButton(1)) {
            input = Vector3.zero;
            return;
        }

        // input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        //VERTICAL
        if(Input.GetKey(Key_MoveUp)) input.z = 1;
        else if(Input.GetKey(Key_MoveDown)) input.z = -1;
        else input.z = 0;
        //HORIZONTAL
        if(Input.GetKey(Key_MoveRight)) input.x = 1;
        else if(Input.GetKey(Key_MoveLeft)) input.x = -1;
        else input.x = 0;

        // Check for sprinting input (Left Shift)
        if(Input.GetKey(Key_Run) && stamina > 0) {
            input *= sprintMultiplier;
        }

        if(Input.GetKeyDown(KeyCode.Escape)) uiManager.current.ToggleEsc();

        if(Input.GetKeyDown(Key_ToggleMap)) uiManager.current.ToggleMap();

        //Proximity chat (CURRENTLY OFF)
        // bool isTransmitting = false;
        // if(Input.GetKey(Key_ProxChat)) isTransmitting = true;
        // recorder.TransmitEnabled = isTransmitting;
        // speakerIMG.SetActive(isTransmitting);
    }

    private void Movement() {
        character.MoveWASD(input);

        if(stamina <= staminaMax) {
            //player running and no stamina
            if(Input.GetKey(Key_Run) && stamina == 0) stamina -= staminaLoss;
            //player running and has stamina
            else if(player.velocity.magnitude >= 3.5f && stamina > 0) stamina -= staminaLoss;
            //player stationary and stamina less than max
            // else if(player.velocity.magnitude < 0.5f && stamina < staminaMax) stamina += staminaGain;
            //player walking and stamina less than max
            else if(player.velocity.magnitude < 3.5f && stamina < staminaMax) stamina += staminaGain;

            if(stamina < 0) stamina = 0;
            else if(stamina >= staminaMax) stamina = staminaMax;

            uiManager.current.UpdateStaminaUI(stamina, staminaMax);
        }
    }
}