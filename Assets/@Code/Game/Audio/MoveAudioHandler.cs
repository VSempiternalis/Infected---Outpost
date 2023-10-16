using UnityEngine;
using System.Collections.Generic;

public class MoveAudioHandler : MonoBehaviour {
    private AudioSource source;
    // [SerializeField] private bool isRandomizedPitch;
    // [SerializeField] private float pitchRange;
    public string isPlaying = "Null";
    [SerializeField] private List<AudioClip> tileAudios;

    private void Start() {
        source = GetComponent<AudioSource>();
    }

    private void Update() {
        Play(isPlaying);
    }

    public void Play(string floorType) {
        if(source.isPlaying || floorType == "Null") return;
        List<AudioClip> currentAudios = new List<AudioClip>();
        int i = 0;

        if(floorType == "Tiles") {
            currentAudios = tileAudios;
        }

        i = Random.Range(0, currentAudios.Count - 1);
        source.PlayOneShot(currentAudios[i]);
    }
}
