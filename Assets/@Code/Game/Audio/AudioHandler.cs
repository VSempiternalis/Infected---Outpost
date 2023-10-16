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
        print("AH Playing: " + 0);
        if(i < audios.Count) {
            source.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
            source.clip = audios[i];
            source.Play();
        }
    }

    public void PlayOneShot(int i) {
        if(i < audios.Count) {
            source.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
            source.PlayOneShot(audios[i]);
        }
    }

    public void PlayRandom() {
        if(!source.isPlaying) {
            int i = Random.Range(0, audios.Count - 1);
            source.PlayOneShot(audios[i]);
        }
    }
}
