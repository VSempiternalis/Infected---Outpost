using UnityEngine;
using System.Collections.Generic;

public class CharacterTrigger : MonoBehaviour, ITooltipable {
    [SerializeField] private string header;
    [SerializeField] private string desc;

    public List<GameObject> charactersInTrigger = new List<GameObject>();

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Character")) {
            charactersInTrigger.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Character")) {
            charactersInTrigger.Remove(other.gameObject);
        }
    }

    public List<GameObject> GetObjectsInTrigger() {
        return charactersInTrigger;
    }

    public string GetHeader() {
        return header;
    }

    public string GetContent() {
        string extra = "";

        if(charactersInTrigger.Count > 0) {
            foreach(GameObject character in charactersInTrigger) {
                extra += "\n- " + character.name;
            }
        }

        return desc + "\n\nCONTAINS:" + extra;
    }
}
