using Photon.Pun;
using UnityEngine;

public class ItemHandler : MonoBehaviourPunCallbacks, IClickable, ITooltipable {
    public string itemName;
    [TextArea] [SerializeField] private string desc;

    [SerializeField] private Vector3 onhandRotation; //fuel (0, -90, 0). key (0, 0, 90). gun (90, 0, 0)

    // public bool isOwned;

    private AudioHandler ah;

    private IUsable usable;

    private void Start() {
        usable = GetComponent<IUsable>();
        // gameObject.SetActive(true);
    }

    private void Update() {
        
    }

    public void OnClick() {

    }

    public void SetParent(Transform newParent, Vector3 pos) {
        if(newParent != null) {
            transform.SetParent(newParent);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(onhandRotation);
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<BoxCollider>().isTrigger = true;

            // if(newParent.GetComponent<Character>()) isOwned = false;
        } else {
            transform.SetParent(null);
            // transform.position = pos;
            transform.position = new Vector3(pos.x, pos.y + 0.5f, pos.z);
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<BoxCollider>().isTrigger = false;
        }
    }

    // public void SetIsOwned(bool newIsOwned) {
    //     photonView.RPC("SetIsOwnedRPC", RpcTarget.All, newIsOwned);
    // }

    // [PunRPC] private void SetIsOwnedRPC(bool newIsOwned) {
    //     isOwned = newIsOwned;
    // }

    // public void SetParent(string parentName) {
    //     photonView.RPC("SetParentRPC", RpcTarget.All, parentName);
    // }

    // [PunRPC] private void SetParentRPC(string parentName) {
    //     transform.SetParent(GameObject.Find(parentName).transform);

    //     //reset position
    //     transform.position = Vector3.zero;
    // }

    //==================================================

    public string GetHeader() {
        return itemName;
    }

    public string GetContent() {
        print("Get Content");
        // string content = "Content";
        if(usable != null) return desc + "\n\n" + usable.GetContent();
        else return desc;
    }
}
