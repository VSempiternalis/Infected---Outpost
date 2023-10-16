using UnityEngine;
using System.Collections.Generic;

public class CharacterTrigger : MonoBehaviour {
    public List<GameObject> charactersInTrigger = new List<GameObject>();

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character")) {
            charactersInTrigger.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Character")) {
            charactersInTrigger.Remove(other.gameObject);
        }
    }

    public List<GameObject> GetObjectsInTrigger() {
        return charactersInTrigger;
    }
}
