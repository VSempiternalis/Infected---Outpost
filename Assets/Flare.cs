using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Flare : MonoBehaviourPunCallbacks, IUsable {
    public bool isOn;
    // private int timeLeft;
    // [SerializeField] private int bulletCountOnStart;

    private bool isAiming;
    private Vector3 aimPos;

    [SerializeField] private ParticleSystem[] particles;
    [SerializeField] private GameObject lightFlash;

    private AudioHandler ah;

    private void Start() {
        // timeLeft = GameMaster.current.flareTimeLeftOnStart;
        ah = GetComponent<AudioHandler>();
    }

    private void Update() {
        
    }

    public void Use(GameObject target, int userType) {
        photonView.RPC("UseRPC", RpcTarget.All, target.name);
    }

    [PunRPC] private void UseRPC(string targetName) {
        print("Using flare");
        if(!isOn) { //Turn on
            isOn = true;

            //Particles
            foreach(ParticleSystem ps in particles) {
                ps.Play();
            }

            lightFlash.SetActive(true);

            StartCoroutine(DeactivateLightFlash());

            ah.PlayOneShot(0);
            ah.Play(1);
        }
    }

    private IEnumerator DeactivateLightFlash() {
        // Wait for the desired duration (e.g., half a second)
        yield return new WaitForSeconds(GameMaster.current.flareTimeLeftOnStart);

        // Deactivate lightFlash after the delay
        lightFlash.SetActive(false);

        //particles
        foreach(ParticleSystem ps in particles) {
            ps.Stop();
        }

        GetComponent<AudioSource>().Stop();
    }

    public string GetContent() {
        // string returnString = "This flare is off.";
        // if(isOn) returnString = timeLeft + " seconds left.";

        return "";
    }
}
