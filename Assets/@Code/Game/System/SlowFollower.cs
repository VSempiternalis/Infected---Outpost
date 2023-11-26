using UnityEngine;

public class SlowFollower : MonoBehaviour {
    public Transform toFollow;
    // [SerializeField] private float moveSpeed;
    // private Rigidbody rb;
    [SerializeField] private float smoothSpeed = 5f; // Adjust the smoothing speed
    private Vector3 offset;

    private void Start() {
        // rb = GetComponent<Rigidbody>();
        if(toFollow) offset = transform.position - toFollow.position;
    }

    // private void FixedUpdate() {
    //     // Calculate the direction from the current object to the target object
    //     Vector3 direction = toFollow.position - transform.position;

    //     // Normalize the direction vector to get a unit vector
    //     direction.Normalize();

    //     // Apply a force in the direction of the target object
    //     rb.AddForce(direction * moveSpeed, ForceMode.Force);
    // }

    private void LateUpdate() {
        if(!toFollow) return;
        
        Vector3 targetPosition = toFollow.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
