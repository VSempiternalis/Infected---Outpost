using UnityEngine;
using System;

public class KeybindsManager : MonoBehaviour {
    public static KeybindsManager current;

    //[EVENTS]
    public event Action onKeyChangeEvent;

    private void Awake() {
        current = this;
    }

    public void KeyChanged() {
        if(onKeyChangeEvent != null) onKeyChangeEvent();
    }
}