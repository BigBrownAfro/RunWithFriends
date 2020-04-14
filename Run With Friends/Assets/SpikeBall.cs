using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeBall : MonoBehaviour
{
    Rigidbody2D rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPlayerTouch();
    }

    private void CheckForPlayerTouch()
    {
        GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
        Player[] players = gameController.players;

        for (int i = 0; i < players.Length; i++)
        {
            if (rigidbody.IsTouching(players[i].feetCollider) || rigidbody.IsTouching(players[i].bodyCollider))
            {
                players[i].Hurt(1);
            }
        }
    }

    public void timedSelfDestruct()
    {
        Destroy(this.gameObject, 5);
    }
}
