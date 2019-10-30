using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

public class Gun : GunBehavior
{
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

        Vector3 bulletPos = this.transform.position + Vector3.right * 2;

        if (Input.GetMouseButtonDown(0))
        {
            this.networkObject.SendRpc(RPC_FIRE, Receivers.Server, bulletPos, this.transform.rotation);
        }

        this.networkObject.rotation = this.transform.rotation;
    }

    public override void fire(RpcArgs args)
    {
        NetworkManager.Instance.InstantiateBullet(position: args.GetNext<Vector3>(), rotation: args.GetNext<Quaternion>());
    }
}
