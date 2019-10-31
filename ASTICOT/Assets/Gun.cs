using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class Gun : GunBehavior
{
    private GameLogic gameLogic;

    private void Awake()
    {
        this.gameLogic = FindObjectOfType<GameLogic>();
    }

    private void Update()
    {
        if (this.networkObject == null)
        {
            return;
        }

        // if not owner just update position from the server
        if (!this.networkObject.IsOwner)
        {
            this.transform.rotation = this.networkObject.rotation;
            return;
        }

        Vector3 mouseScreenPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));

        Vector3 lookAt = mouseScreenPosition;

        float AngleRad = Mathf.Atan2(lookAt.y - this.transform.position.y, lookAt.x - this.transform.position.x);

        float AngleDeg = (180 / Mathf.PI) * AngleRad;

        this.transform.rotation = Quaternion.Euler(0, 0, AngleDeg);

        if (Input.GetMouseButton(0))
        {
            this.gameLogic.networkObject.SendRpc(GameLogic.RPC_SPAWN_BULLET, Receivers.Server, this.transform.position, this.transform.rotation);
        }

        this.networkObject.rotation = this.transform.rotation;
    }
}
