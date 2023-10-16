using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Scripting;

public class Escape : MonoBehaviour, ITooltipable, IClickable {
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
        lastUpdate = 0f;
        CheckForRequirements();
    }

    private void Update() {
        lastUpdate += Time.deltaTime;
        
        if(lastUpdate >= 1.0f) {
            CheckForRequirements();
            lastUpdate = 0f;
        }
    }

    public void OnClick() {
        print("ESCAPING!");

        //list escapees/survivors and check if one escapee is infected
        bool isExtinction = false;
        foreach(GameObject character in trigger.charactersInTrigger) {
            if(character.GetComponent<Character>() && character.GetComponent<Character>().type == 0) isExtinction = true;
        }

        if(isExtinction) WinManager.current.Endgame("Extinction");
        else WinManager.current.Endgame("Escape");
    }

    private void CheckForRequirements() {
        // print("Checking for requirements...");
        requirementsMet = true;

        if(requirementsStorage.item) {
            // print("Storage has item!");
            ItemHandler item = requirementsStorage.item;
            foreach(Requirement req in requirements) {
                // print("Checking: " + item.itemName + " / " + req.itemName);
                if(item.itemName == req.itemName) {
                    if(item.GetComponent<FuelCan>() && !item.GetComponent<FuelCan>().hasFuel) req.providedAmount ++;
                    else if(!item.GetComponent<FuelCan>()) req.providedAmount ++;

                    //Remove item
                    requirementsStorage.RemoveItem();
                    Destroy(item.gameObject);
                } 
            }
        }

        //Check if all requirements met
        foreach(Requirement req in requirements) {
            if(req.providedAmount < req.requiredAmount) requirementsMet = false;
        }
        // print("Requirements met: " + requirementsMet);

        if(requirementsMet) {
            //play sfx

            //do stuff
            GetComponent<IEscapable>().Ready();
        }
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