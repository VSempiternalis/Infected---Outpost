using UnityEngine;
using System.Collections.Generic;

public class AudioHandler : MonoBehaviour {
    private AudioSource source;
    // [SerializeField] private bool isRandomizedPitch;
    [SerializeField] private float pitchRange;
    [SerializeField] private List<AudioClip> audios;

    private void Start() {
        source = GetComponent<AudioSource>();
    }

    private void Update() {
        
    }

    public void Play(int i) {
        if(i < audios.Count) {
            source.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
            source.PlayOneShot(audios[i]);
        }
    }
}
