using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using TMPro;
using UnityEngine;

public class GameLogic : GameLogicBehavior
{
    public TextMeshProUGUI Auth;

    // Start is called before the first frame update
    private void Start()
    {
        if (NetworkManager.Instance == null)
        {
            return;
        }
        NetworkManager.Instance.InstantiatePlayer(position: new Vector3(0, 0, 0));

        this.Auth.text = (NetworkManager.Instance.IsServer) ? "Server" : "Client";
    }

    public override void spawnBullet(RpcArgs args)
    {
        NetworkManager.Instance.InstantiateBullet(0, args.GetNext<Vector3>(), args.GetNext<Quaternion>(), true);
    }
}
