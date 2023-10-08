using UnityEngine;
using UnityEngine.AI;

public class Controller : MonoBehaviour {
    public static Controller current;
    private Character character;

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
    private GameObject pointed; //object player cursor is pointing at

    private void Awake() {
        current = this;
    }

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if(player == null) return;

        GetInput();
        MovePointer();
        MoveHead();
    }

    private void FixedUpdate() {
        if(player == null) return;
        
        Movement();
    }

    public void SetPlayer(Rigidbody newPlayer) {
        player = newPlayer;
        character = newPlayer.GetComponent<Character>();
        // agent = player.GetComponent<NavMeshAgent>();
        // head = player.GetComponent<Character>().head;
        mainCamera.transform.parent.position = player.GetComponent<Character>().campos.position;
        mainCamera.transform.parent.GetComponent<SlowFollower>().toFollow = player.GetComponent<Character>().campos;
    }

    private void MovePointer() {
        if(pointed && pointed.GetComponent<Outline>()) pointed.GetComponent<Outline>().OutlineWidth = 0;
        if(pointed) print("Pointed: " + pointed + " has outline: " + (pointed.GetComponent<Outline>()? "Yes":"No"));
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a raycast to see if it hits something in the game world
        if (Physics.Raycast(ray, out hit)) {
            // Move the object to the point where the ray intersects with the world
            pointer.position = hit.point;

            //Set pointed
            pointed = hit.collider.gameObject;

            //Outline
            if(pointed.GetComponent<Outline>()) {
                pointed.GetComponent<Outline>().OutlineWidth = 3; //3 is standard outline width
            }
        }
    }

    private void MoveHead() {
        character.MoveHead(pointer.position);
    }

    private void GetInput() {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // Check for sprinting input (Left Shift)
        if(Input.GetKey(KeyCode.LeftShift)) {
            input *= sprintMultiplier;
        }
    }

    private void Movement() {
        character.MoveWASD(input);
    }
}