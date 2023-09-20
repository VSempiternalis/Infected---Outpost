using System.Reflection;
using UnityEngine;

public class Controller : MonoBehaviour {
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
    private Transform head;
    [SerializeField] private float headSpeed;
    private Quaternion targetHeadRot;

    private void Start() {
        mainCamera = Camera.main;
        head = player.GetComponent<Character>().head;
    }

    private void Update() {
        GetInput();
        MovePointer();
        MoveHead();
    }

    private void FixedUpdate() {
        Movement();
    }

    private void MovePointer() {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Perform a raycast to see if it hits something in the game world
        if (Physics.Raycast(ray, out hit)) {
            // Move the object to the point where the ray intersects with the world
            pointer.position = hit.point;
        }
    }

    private void MoveHead() {
        // Calculate the direction from the head to the pointer, disregarding the Y-axis
        Vector3 lookDirection = pointer.position - head.position;
        lookDirection.y = 0; // Disregard the Y-axis component

        // Calculate the target rotation
        targetHeadRot = Quaternion.LookRotation(lookDirection);

        // Rotate the head towards the target rotation with a delay
        head.rotation = Quaternion.Slerp(head.rotation, targetHeadRot, headSpeed * Time.deltaTime);
    }

    private void GetInput() {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        
        // Check for sprinting input (Left Shift)
        if(Input.GetKey(KeyCode.LeftShift)) {
            input *= sprintMultiplier;
        }
    }

    private void Movement() {
        // Calculate the diagonal direction by combining input along both the X and Z axes.
        Vector3 moveDirection = new Vector3(input.x, 0, input.z);//.normalized;

        // Check if there's any input
        if (moveDirection != Vector3.zero) {
            // Rotate the movement direction to match your isometric view (e.g., 45 degrees).
            moveDirection = Quaternion.Euler(0, 45, 0) * moveDirection;

            // Apply the movement with adjusted speed
            Vector3 targetVelocity = moveDirection * (moveSpeed * sprintMultiplier);
            player.velocity = Vector3.SmoothDamp(player.velocity, targetVelocity, ref currentVelocity, 0.1f);
        } else {
            // If no input is detected, gradually slow down the character
            player.velocity = Vector3.SmoothDamp(player.velocity, Vector3.zero, ref currentVelocity, 0.1f);
        }
    }
}