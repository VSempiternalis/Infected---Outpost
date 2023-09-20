using UnityEngine;

public class MainMenu : MonoBehaviour {
    [SerializeField] private CustomNetworkManager netMan = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    //Called when hosting a match
    public void HostLobby() {
        netMan.StartHost();

        landingPagePanel.SetActive(false);
    }
}
