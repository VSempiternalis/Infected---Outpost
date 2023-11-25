using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.VisualScripting;

public class MapSettings : MonoBehaviourPunCallbacks {
    public int generatorTimeOnStart;
    public int fuelTimeAdd;
    public int revolverBulletCountOnStart;
    public int infectCooldownTime;
    public int flareTimeLeftOnStart;

    public bool isInteractable;
    [SerializeField] private TMP_InputField generatorTimeInput;
    [SerializeField] private TMP_InputField fuelAddInput;
    [SerializeField] private TMP_InputField revolverBulletsInput;
    [SerializeField] private TMP_InputField infectCooldownInput;
    [SerializeField] private TMP_InputField flareTimeInput;

    [SerializeField] private GameObject startingText;

    private void Start() {
        if(PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient) UpdateRoomSettings();
    }

    public void Starting() {
        photonView.RPC("StartingRPC", RpcTarget.All);
    }

    [PunRPC] private void StartingRPC() {
        print("STARTING RPC");
        startingText.SetActive(true);
    }

    private void UpdateRoomSettings() {
        print("UPDATING ROOM SETTINGS FROM GAME MASTER");
        generatorTimeOnStart = GameMaster.countdownOnStart;
        fuelTimeAdd = GameMaster.fuelTimeAdd;
        revolverBulletCountOnStart = GameMaster.revolverBulletCountOnStart;
        infectCooldownTime = GameMaster.infectCooldownTime;
        flareTimeLeftOnStart = GameMaster.flareTimeLeftOnStart;

        UpdateUI();
        SetClientSettings();
    }

    public void SetGeneratorTime() {
        generatorTimeOnStart = int.Parse(generatorTimeInput.text);
        UpdateUI();
        SetClientSettings();
    }

    public void SetFuelAddTime() {
        fuelTimeAdd = int.Parse(fuelAddInput.text);
        UpdateUI();
        SetClientSettings();
    }

    public void SetRevolverBulletCount() {
        revolverBulletCountOnStart = int.Parse(revolverBulletsInput.text);
        UpdateUI();
        SetClientSettings();
    }

    public void SetInfectCooldown() {
        infectCooldownTime = int.Parse(infectCooldownInput.text);
        UpdateUI();
        SetClientSettings();
    }

    public void SetFlareTime() {
        flareTimeLeftOnStart = int.Parse(flareTimeInput.text);
        UpdateUI();
        SetClientSettings();
    }

    //=========================================================

    public void GetSettingsFromMaster() {
        print("Get Settings from master");
        photonView.RPC("GetSettingsFromMasterRPC", RpcTarget.MasterClient);
    }

    [PunRPC] private void GetSettingsFromMasterRPC() {
        print("get settings from master rpc");
        photonView.RPC("SetClientSettingsRPC", RpcTarget.Others, generatorTimeOnStart, fuelTimeAdd, revolverBulletCountOnStart, infectCooldownTime, flareTimeLeftOnStart);
    }

    private void SetClientSettings() {
        print("setting client settings");
        photonView.RPC("SetClientSettingsRPC", RpcTarget.Others, generatorTimeOnStart, fuelTimeAdd, revolverBulletCountOnStart, infectCooldownTime, flareTimeLeftOnStart);
    }

    [PunRPC] private void SetClientSettingsRPC(int gen, int fuel, int revolver, int infect, int flare) {
        print("set client settings rpc: " + gen + " " + fuel);
        generatorTimeOnStart = gen;
        fuelTimeAdd = fuel;
        revolverBulletCountOnStart = revolver;
        infectCooldownTime = infect;
        flareTimeLeftOnStart = flare;

        UpdateUI();
    }

    public void SetSettingsInteractable(bool newInteractable) {
        print("SetSettingsInteractable: " + newInteractable);
        isInteractable = newInteractable;
        generatorTimeInput.interactable = newInteractable;
        fuelAddInput.interactable = newInteractable;
        revolverBulletsInput.interactable = newInteractable;
        infectCooldownInput.interactable = newInteractable;
        flareTimeInput.interactable = newInteractable;
    }

    private void UpdateUI() {
        print("Updating UI");
        generatorTimeInput.text = generatorTimeOnStart.ToString();
        fuelAddInput.text = fuelTimeAdd.ToString();
        revolverBulletsInput.text = revolverBulletCountOnStart.ToString();
        infectCooldownInput.text = infectCooldownTime.ToString();
        flareTimeInput.text = flareTimeLeftOnStart.ToString();
        
        UpdateGameMaster();
    }

    private void UpdateGameMaster() {
        print("Updating game master");
        GameMaster.countdownOnStart = generatorTimeOnStart;
        GameMaster.fuelTimeAdd = fuelTimeAdd;
        GameMaster.revolverBulletCountOnStart = revolverBulletCountOnStart;
        GameMaster.infectCooldownTime = infectCooldownTime;
        GameMaster.flareTimeLeftOnStart = flareTimeLeftOnStart;
    }
}
