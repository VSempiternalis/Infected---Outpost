using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Gun : MonoBehaviourPunCallbacks, IUsable, IAimable {
    private int bulletCount;
    // [SerializeField] private int bulletCountOnStart;

    private bool isAiming;
    private Vector3 aimPos;

    [SerializeField] private ParticleSystem muzzleFlashPS;
    [SerializeField] private GameObject lightFlash;

    private void Start() {
        bulletCount = GameMaster.current.revolverBulletCountOnStart;
    }

    private void Update() {
        // isAiming = false;
        // GetComponent<LineRenderer>().enabled = false;
        // if(lightFlash.activeSelf) lightFlash.SetActive(false);
    }

    private void LateUpdate() {
        // isAiming = false;
        // if(!isAiming) GetComponent<LineRenderer>().enabled = false;
    }

    public void UnAim() {
        photonView.RPC("UnAimRPC", RpcTarget.All);
    }

    [PunRPC] private void UnAimRPC() {
        // print("UnAimRPC");
        isAiming = false;
        GetComponent<LineRenderer>().enabled = false;
    }

    public void Aim(Vector3 newAimPos) {
        // print("Aiming");
        photonView.RPC("AimRPC", RpcTarget.All, newAimPos.x, newAimPos.y, newAimPos.z);
    }

    [PunRPC] private void AimRPC(float xPos, float yPos, float zPos) {
        if(!isAiming) {
            print("Setting isAiming to true and playing sfx");
            isAiming = true;

            //sfx
            GetComponent<AudioHandler>().PlayOneShot(0);
        }
        
        aimPos = new Vector3(xPos, yPos, zPos);
        GetComponent<LineRenderer>().enabled = true;
        GetComponent<LineRenderer>().SetPosition(0, transform.GetChild(0).position); //start laser from muzzle position
        GetComponent<LineRenderer>().SetPosition(1, new Vector3(xPos, yPos, zPos));
    }

    public void Use(GameObject target, int userType) {
        print("Using gun");
        if(!GetComponent<LineRenderer>().enabled || userType == 0) return;
        photonView.RPC("UseRPC", RpcTarget.All, target.name);
    }

    [PunRPC] private void UseRPC(string targetName) {
        if(bulletCount < 1) {
            //sfx
            GetComponent<AudioHandler>().PlayOneShot(1);
            return;
        }
        bulletCount --;

        //sfx

        print("Using gun rpc. target: " + targetName);
        muzzleFlashPS.Play();
        lightFlash.SetActive(true);
        
        // Start a Coroutine to deactivate lightFlash after a short delay
        StartCoroutine(DeactivateLightFlash());

        GameObject target = GameObject.Find(targetName);
        if(target.GetComponent<Character>()) {
            print("trying to kill target: " + target.name);
            target.GetComponent<Character>().Kill();
        } else {
            
        }

        //sfx
        GetComponent<AudioHandler>().PlayOneShot(2);
            //particles
    }

    private IEnumerator DeactivateLightFlash() {
        // Wait for the desired duration (e.g., half a second)
        yield return new WaitForSeconds(0.1f);

        // Deactivate lightFlash after the delay
        lightFlash.SetActive(false);
    }

    public string GetContent() {
        return bulletCount + " bullets left in the chamber.";
    }
}
