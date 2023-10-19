using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class Escape : MonoBehaviourPunCallbacks, ITooltipable, IClickable {
    [SerializeField] private string escapeName;
    [SerializeField] private string description;
    [SerializeField] private string requirementsMetText;

    // [SerializeField] private List<string> requirementsText;
    // [SerializeField] private List<int> requirementsActive;
    // [SerializeField] private List<int> requirementsCount;
    private bool requirementsMet = true;
    [SerializeField] private List<Requirement> requirements;
    // [SerializeField] private List<ItemHandler> requirementsProvided;
    [SerializeField] private Storage requirementsStorage;

    [SerializeField] private CharacterTrigger trigger;

    private float lastUpdate;

    private void Start() {
        if(PhotonNetwork.IsMasterClient) {
            lastUpdate = 0f;
            CheckForRequirements();
        }
    }

    private void Update() {
        if(PhotonNetwork.IsMasterClient) {
            lastUpdate += Time.deltaTime;
            
            if(lastUpdate >= 1.0f) {
                CheckForRequirements();
                lastUpdate = 0f;
            }
        }
    }

    public void OnClick() {
        photonView.RPC("OnClickRPC", RpcTarget.MasterClient);
    }

    [PunRPC] private void OnClickRPC() {
        print("ESCAPING!");
        if(!requirementsMet) return;

        //list escapees/survivors and check if one escapee is infected
        bool isExtinction = false;
        foreach(GameObject character in trigger.charactersInTrigger) {
            if(character.GetComponent<Character>() && character.GetComponent<Character>().type == 0 && character.GetComponent<Character>().isAlive) isExtinction = true;
        }

        if(isExtinction) WinManager.current.Endgame("Extinction");
        else WinManager.current.Endgame("Escape");
    }

    private void CheckForRequirements() {
        if(requirementsStorage.item) {
            ItemHandler item = requirementsStorage.item;
            foreach(Requirement req in requirements) {
                if(item.itemName == req.itemName) {
                    bool isFuel = item.GetComponent<FuelCan>();
                    bool hasFuel = false;
                    if(isFuel) hasFuel = item.GetComponent<FuelCan>().hasFuel;

                    photonView.RPC("UpdateRequirementsRPC", RpcTarget.All, item.itemName, isFuel, hasFuel);
                    if((isFuel && hasFuel) || (!isFuel)) photonView.RPC("RemoveRequirementFromStorageRPC", RpcTarget.All);
                } 
            }
        }
    }

    //Called when adding a new req from storage
    [PunRPC] private void UpdateRequirementsRPC(string reqName, bool isFuel, bool hasFuel) {
        requirementsMet = true;

        foreach(Requirement req in requirements) {
            if(req.itemName == reqName) {
                if(isFuel && hasFuel) req.providedAmount ++;
                else if(!isFuel) req.providedAmount ++;
            }

            //Check if all requirements met
            if(req.providedAmount < req.requiredAmount) requirementsMet = false;
        }

        if(requirementsMet) GetComponent<IEscapable>().Ready();
    }

    [PunRPC] private void RemoveRequirementFromStorageRPC() {
        ItemHandler item = requirementsStorage.item;
        requirementsStorage.RemoveItem();
        Destroy(item.gameObject);
    }

    public string GetHeader() {
        return escapeName;
    }

    public string GetContent() {
        if(requirementsMet) return description + "\n\nALL REQUIREMENTS MET!\n" + requirementsMetText;
        else {
            string reqString = "";
            foreach(Requirement req in requirements) {
                reqString += "\n- " + req.itemName + ": " + req.providedAmount + "/" + req.requiredAmount;
            }
            return description + "\n\nREQUIREMENTS" + reqString;
        }
    }
}

[System.Serializable]
public class Requirement {
    public string itemName;
    public int providedAmount;
    public int requiredAmount;
}