using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class PlayerItem : MonoBehaviour {
    [SerializeField] private TMP_Text playerName;

    public void SetPlayerInfo(Player player) {
        playerName.text = player.NickName;
    }
}
