using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class Player : PlayerBehavior
{
    public float speed = 5.0f;

    private Rigidbody2D rigidbodyRef;

    private void Awake()
    {
        this.rigidbodyRef = GetComponent<Rigidbody2D>();
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();
        if (!this.networkObject.IsOwner)
        {
            Destroy(GetComponent<Rigidbody2D>());
            Destroy(GetComponent<Collider2D>());
            GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
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

        // move player
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            this.transform.position += Vector3.left * Time.deltaTime * this.speed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += Vector3.right * Time.deltaTime * this.speed;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            this.rigidbodyRef.AddForce(new Vector2(0, 100));
        }

        if (this.transform.position.y < -10)
        {
            Reset();
        }

        // update position on the server
        this.networkObject.position = this.transform.position;
    }

    private void Reset()
    {
        this.transform.position = new Vector3(0, 0, 0);

        // Reset the velocity for this object to zero
        this.rigidbodyRef.velocity = Vector3.zero;
    }
}