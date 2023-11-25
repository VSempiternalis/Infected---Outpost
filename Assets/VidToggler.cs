using UnityEngine;
using UnityEngine.Video;

public class VidToggler : MonoBehaviour {
    [SerializeField] private VideoPlayer vp;

    public void ToggleVP() {
        if(vp.isPlaying) vp.Pause();
        else vp.Play();
    }
}
