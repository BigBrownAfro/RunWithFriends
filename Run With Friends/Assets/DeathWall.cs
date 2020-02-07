using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    GameController gameController;
    Rigidbody2D rigidbody;
    Player[] players;


    bool isActive = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }
        Move();
        CheckPlayerCollision();
    }

    public void Activate()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        this.players = gameController.players;
        isActive = true;
    }

    public void Move()
    {
        rigidbody.position = new Vector2(rigidbody.position.x + .02f, rigidbody.position.y);
    }

    private void CheckPlayerCollision()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (rigidbody.IsTouching(players[i].feetCollider) || rigidbody.IsTouching(players[i].bodyCollider))
            {
                players[i].Hurt(1);
            }
        }
    }
}
