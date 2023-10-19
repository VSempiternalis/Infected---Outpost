using UnityEngine;

public class Snowcat : MonoBehaviour, IEscapable {
    private AudioHandler ah;

    [SerializeField] private GameObject lights1;
    [SerializeField] private GameObject lights2;

    private void Start() {
        ah = GetComponent<AudioHandler>();
    }

    public void Ready() {
        if(!GetComponent<AudioSource>().isPlaying) ah.Play(0);
        lights1.SetActive(true);
        lights2.SetActive(true);
    }
}
