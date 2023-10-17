using UnityEngine;
using Photon.Pun;

public class Key : MonoBehaviourPunCallbacks, IUsable {
    [SerializeField] private string desc;
    // [SerializeField] private string keyName;

    private void Start() {
        
    }

    private void Update() {
        
    }

    public void Use(GameObject target, int type) {
        photonView.RPC("UseRPC", RpcTarget.All, target.name);
    }

    [PunRPC] private void UseRPC(string targetName) {
        print("Using key");
        GameObject target = GameObject.Find(targetName);
        if(target.GetComponent<Storage>() && target.GetComponent<Storage>().keyName == GetComponent<ItemHandler>().itemName) target.GetComponent<DoorHandler>().ChangeState();

        //sfx
        GetComponent<AudioHandler>().PlayOneShot(0);
    }

    public string GetContent() {
        return desc;
    }
}
