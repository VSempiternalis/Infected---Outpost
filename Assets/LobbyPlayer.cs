using UnityEngine;
using Mirror;
using TMPro;
// using Steamworks;

public class LobbyPlayer : NetworkBehaviour {
    #region SERVER



    #endregion SERVER

    //========================================

    #region CLIENT



    #endregion CLIENT

    //========================================

    #region GENERAL

    public override void OnStartLocalPlayer() {
        transform.SetParent(GameObject.Find("IMG - Online").transform);
        transform.GetChild(0).GetComponent<TMP_Text>().text = connectionToClient.connectionId.ToString();

        base.OnStartLocalPlayer();
    }

    #endregion GENERAL
}
