using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    Rigidbody2D rigidbody;
    bool isFalling = false;
    float fallSpeed = .05f;

    Vector2 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startPosition = new Vector2(rigidbody.position.x, rigidbody.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            Fall();
        }
        else
        {
            CheckForPlayerTouch();
        }
    }

    public void Toggle()
    {
        isFalling = true;
    }

    private void CheckForPlayerTouch()
    {
        GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Player[] players = gameController.players;

        for (int i = 0; i < players.Length; i++)
        {
            if (rigidbody.IsTouching(players[i].feetCollider))
            {
                Toggle();
            }
        }
    }

    private void Fall()
    {
        rigidbody.velocity += new Vector2(0, -fallSpeed);
        if (rigidbody.position.y < -50)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        isFalling = false;
        rigidbody.velocity = new Vector2(0, 0);
        rigidbody.position = new Vector2(startPosition.x, startPosition.y);
    }
}
