using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingSpike : MonoBehaviour
{
    Rigidbody2D rigidbody;
    bool isFalling = false;
    float fallSpeed = .1f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isFalling)
        {
            Fall();
        }
    }

    private void Fall()
    {
        rigidbody.velocity += new Vector2(0, -fallSpeed);
        if(rigidbody.position.y < -30)
        {
            Destroy(this.gameObject);
        }
    }

    public void Toggle()
    {
        isFalling = true;
    }
}
