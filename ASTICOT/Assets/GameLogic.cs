using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Unity;
using TMPro;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public TextMeshProUGUI Auth;

    // Start is called before the first frame update
    private void Start()
    {
        if (NetworkManager.Instance == null)
        {
            return;
        }
        if (NetworkManager.Instance.IsServer == false)
        {
            NetworkManager.Instance.InstantiatePlayer(position: new Vector3(0, 0, 0));
        }

        this.Auth.text = (NetworkManager.Instance.IsServer) ? "Server" : "Client";
    }

}
