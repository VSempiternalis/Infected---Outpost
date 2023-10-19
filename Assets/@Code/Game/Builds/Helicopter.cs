using UnityEngine;

public class Helicopter : MonoBehaviour, IEscapable {
    private AudioHandler ah;

    [SerializeField] private Transform rotors;
    [SerializeField] private float rotorRotationSpeed = 359.0f; // Adjust the speed as needed
    private bool isRotating = false;

    private void Start() {
        ah = GetComponent<AudioHandler>();
        isRotating = false;
        // isRotating = true;
    }

    private void Update() {
        if(isRotating) RotateRotors();
    }

    private void RotateRotors() {
        rotors.Rotate(Vector3.forward * rotorRotationSpeed * Time.deltaTime);
    }

    public void Ready() {
        if(!GetComponent<AudioSource>().isPlaying) ah.Play(0);
        isRotating = true;
    }
}
