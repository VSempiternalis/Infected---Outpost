using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager current;
    [SerializeField] private AudioHandler uiHandler;
    [SerializeField] private AudioHandler ambienceHandler;

    private void Awake() {
        current = this;
    }

    private void Start() {
        
    }

    private void Update() {
        
    }

    public void PlayUI(int i) {
        uiHandler.Play(i);
    }

    public void PlayAmb(int i) {
        print("Playing ambience: " + i);
        ambienceHandler.Play(i);
    }
}

// [System.Serializable]
// public class Sound {
//     public string name;
//     public AudioClip clip;
// }
