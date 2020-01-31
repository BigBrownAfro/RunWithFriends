using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTile : MonoBehaviour
{
    Rigidbody2D rigidBody;
    Player[] players;

    bool ready = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!ready)
        {
            Debug.Log(ready);
            return;
        }
        CheckPlayerHit();
    }

    public void setPlayers(Player[] players)
    {
        this.players = players;
        ready = true;
    }

    private void CheckPlayerHit()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (rigidBody.IsTouching(players[i].feetCollider) || rigidBody.IsTouching(players[i].bodyCollider))
            {
                players[i].Hurt(1);
            }
        }
    }
}
