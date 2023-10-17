using UnityEngine;

public class Generator : MonoBehaviour, IClickable, ITooltipable, IFuelable {
    private void Start() {
        
    }

    private void Update() {
        
    }

    public void OnClick() { //ItemHandler item
        // print("Adding fuel");
        // print(item.GetComponent<FuelCan>() != null);
        // print("has fuel: " + item.GetComponent<FuelCan>().hasFuel);
        // if(!item.GetComponent<FuelCan>() || !item.GetComponent<FuelCan>().hasFuel) return;

        // item.GetComponent<FuelCan>().hasFuel = false;
        // WinManager.current.AddFuel();
    }

    public string GetHeader() {
        return "Generator";
    }

    public string GetContent() {
        return "Fuels the outpost's heating.\nWhen the timer goes to zero,\nall humans will die.\n\nUse fuel to increase timer.";
    }
}
