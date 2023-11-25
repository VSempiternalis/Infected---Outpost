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

    private LineRenderer lr;

    private void Start() {
        bulletCount = GameMaster.revolverBulletCountOnStart;
        lr = GetComponent<LineRenderer>();
    }

    public void UnAim() {
        photonView.RPC("UnAimRPC", RpcTarget.All);
    }

    [PunRPC] private void UnAimRPC() {
        isAiming = false;
        lr.enabled = false;
    }

    public void Aim(Vector3 newAimPos) {
        photonView.RPC("AimRPC", RpcTarget.All, newAimPos.x, newAimPos.y, newAimPos.z);
    }

    [PunRPC] private void AimRPC(float xPos, float yPos, float zPos) {
        if(!isAiming) {
            isAiming = true;

            //sfx
            GetComponent<AudioHandler>().PlayOneShot(0);
        }
        
        aimPos = new Vector3(xPos, yPos, zPos);
        lr.enabled = true;
        lr.SetPosition(0, transform.GetChild(0).position); //start laser from muzzle position
        
        //old
        lr.SetPosition(1, new Vector3(xPos, yPos, zPos));

        //new
        // RaycastHit hit;
        // if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(0).TransformDirection(Vector3.right), out hit, Mathf.Infinity)) {
        //     lr.SetPosition(1, hit.point);
        //     // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        // } else {
        //     // Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        // }
    }

    public void Use(GameObject target, int userType) {
        print("Using gun");
        if(!lr.enabled || userType == 0) return;
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
        print("Getting gun content");
        return bulletCount + " bullets left in the chamber.";
    }
}
