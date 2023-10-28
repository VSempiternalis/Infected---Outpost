using UnityEngine;
using System.Collections.Generic;

public class MoveAudioHandler : MonoBehaviour {
    private AudioSource source;
    private bool isOn;
    // [SerializeField] private bool isRandomizedPitch;
    // [SerializeField] private float pitchRange;
    public string isPlaying = "Null";
    [SerializeField] private List<AudioClip> tileAudios;
    [SerializeField] private List<AudioClip> woodAudios;
    [SerializeField] private List<AudioClip> snowAudios;
    [SerializeField] private List<AudioClip> rockAudios;
    [SerializeField] private List<AudioClip> metalAudios;
    [SerializeField] private List<AudioClip> grassAudios;

    private float maxRaycastDist = 0.01f;

    private void Start() {
        source = GetComponent<AudioSource>();
    }

    private void Update() {
        if(isOn) SetIsPlaying();
        Play(isPlaying);
    }

    public void On() {
        isOn = true;
        source.loop = true;
    }

    public void Off() {
        isOn = false;
        isPlaying = "Null";
        source.Stop();
        source.loop = false;
    }

    public void SetIsPlaying() {
        if(transform.parent.parent.GetComponent<aiInput>()) Destroy(this);

        //Get floor type
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit[] hits = Physics.RaycastAll(ray, maxRaycastDist);
        string floorName = "FLOOR - Rock";
        // print("---");
        foreach(RaycastHit hit in hits) {
            string hitName = hit.collider.gameObject.name;
            // print("hitName: " + hitName);
            if(hitName.Contains("FLOOR")) {
                floorName = hitName;
                break;
            }
        }

        isPlaying = floorName;
    }

    public void Play(string floorType) {
        if(source.isPlaying || floorType == "Null") return;
        List<AudioClip> currentAudios = new List<AudioClip>();
        int i = 0;

        if(floorType.Contains("Tiles")) {
            currentAudios = tileAudios;
        } else if(floorType.Contains("Wood")) {
            currentAudios = woodAudios;
        } else if(floorType.Contains("Snow")) {
            currentAudios = snowAudios;
        } else if(floorType.Contains("Rock")) {
            currentAudios = rockAudios;
        } else if(floorType.Contains("Metal")) {
            currentAudios = metalAudios;
        } else if(floorType.Contains("Grass")) {
            currentAudios = grassAudios;
        }

        i = Random.Range(0, currentAudios.Count - 1);
        source.PlayOneShot(currentAudios[i]);
    }
}
