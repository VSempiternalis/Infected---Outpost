using UnityEngine;
using Photon.Pun;

public class FuelCan : MonoBehaviourPunCallbacks, IUsable { //
    public bool hasFuel;

    private void Start() {
        
    }

    private void Update() {
        
    }

    public void Use(GameObject target, int type) {
        if(!hasFuel || target.GetComponent<IFuelable>() == null) return;
        photonView.RPC("UseRPC", RpcTarget.All, target.name);
    }

    [PunRPC] private void UseRPC(string targetName) {
        hasFuel = false;
        GameObject target = GameObject.Find(targetName);
        if(target.GetComponent<Generator>()) WinManager.current.AddFuel();

        //sfx
        GetComponent<AudioHandler>().PlayOneShot(0);
    }

    public string GetContent() {
        if(hasFuel) return "Contains fuel.";
        else return "Empty.";
    }
}
