using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class Bullet : BulletBehavior
{

    protected override void NetworkStart()
    {
        base.NetworkStart();
        if (!this.networkObject.IsOwner)
        {
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<Collider2D>());
        }
        GetComponent<Rigidbody2D>().AddForce(Vector2.right * 100, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.networkObject == null)
        {
            return;
        }

        // if not owner just update position from the server
        if (!this.networkObject.IsOwner)
        {
            this.transform.position = this.networkObject.position;
            return;
        }

        if (this.transform.position.y < -10)
        {
            this.networkObject.Destroy();
        }
    }

    private void OnCollisionEnter(Collision triggeringCollision)
    {
        this.networkObject.Destroy();
    }
}
