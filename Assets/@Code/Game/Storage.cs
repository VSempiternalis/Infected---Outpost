using UnityEngine;
using Photon.Pun;

public class Storage : MonoBehaviourPunCallbacks, IClickable, ITooltipable {
    private DoorHandler door;

    public bool isLocked;
    public string keyName;
    // public string lockName;

    [Space(10)]
    [Header("ITEM")]
    public ItemHandler item; //item inside the storage

    [Space(10)]
    [Header("TOOLTIP")]
    [SerializeField] private string header;
    [SerializeField] private string desc;

    private void Start() {
        door = GetComponent<DoorHandler>();
    }

    private void Update() {
        // print("Item pos on update: " + item.transform.position);
    }

    public void ToggleOutline(bool isOn) {

    }

    public void OnClick() { //item on player when clicked
        if(isLocked) { // && item.itemName != keyName
            //play lock sfx
            return;
        }

        door.ChangeState();
    }

    public void InsertItem(ItemHandler newItem) {
        if(item != null) return;

        photonView.RPC("InsertItemRPC", RpcTarget.All, newItem.name);
    }

    [PunRPC] private void InsertItemRPC(string itemName) {
        if(itemName.Contains("(Clone)")) item = GameObject.Find(itemName).GetComponent<ItemHandler>();
        else item = GameObject.Find(itemName + "(Clone)").GetComponent<ItemHandler>();
        item.SetParent(transform, Vector3.zero);
        // item.transform.SetParent(transform);
        // item.transform.localPosition = Vector3.zero;
        // item.GetComponent<Rigidbody>().isKinematic = true;
        // item.GetComponent<BoxCollider>().isTrigger = true;
        // print("item pos: " + item.transform.position);
    }

    public ItemHandler RemoveItem() {
        print("Removing item: " + item.name);
        photonView.RPC("RemoveItemRPC", RpcTarget.All);

        return item;
    }

    [PunRPC] private void RemoveItemRPC() {
        ItemHandler removedItem = item;
        item = null;
    }

    //==================================================

    public string GetHeader() {
        return header;
    }

    public string GetContent() {
        string content = "Content";

        if(door.state == "Open" || door.state == "Opening") {
            string itemName = "Nothing";
            if(item != null) itemName = item.itemName;

            content = desc + "\n\nCONTAINS:\n- " + itemName;
        } else if(door.state == "Closed" || door.state == "Closing") {
            content = desc;
        }

        if(isLocked) content += "\n\nLOCKED. Requires " + keyName + " to open.";

        return content;
    }
}
