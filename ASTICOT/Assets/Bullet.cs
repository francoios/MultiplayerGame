using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class Bullet : BulletBehavior
{

    protected override void NetworkStart()
    {
        base.NetworkStart();
        this.gameObject.SetActive(true);
        if (!this.networkObject.IsOwner)
        {
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<Collider2D>());
        }
        else
        {
            this.transform.position += transform.right / 1.5f;
            GetComponent<Rigidbody2D>().AddForce(transform.right * 10, ForceMode2D.Impulse);
        }
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

        this.networkObject.position = this.transform.position;

        if (this.transform.position.y < -10)
        {
            this.networkObject.Destroy();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!this.networkObject.IsOwner)
        {
            return;
        }

        this.networkObject.Destroy();
    }
}
