using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] [Range(1,4)] int playerCount;
    [SerializeField] GameObject playerGameObject;
    KillTile killTilemap;
    Rigidbody2D foregroundBody;

    Player[] players;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        VerifyComponents();
        CreatePlayers();
        FillDependencies();
    }

    private void Init()
    {
        killTilemap = GameObject.FindGameObjectWithTag("KillTilemap").GetComponent<KillTile>();
        foregroundBody = GameObject.FindGameObjectWithTag("ForegroundTilemap").GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void VerifyComponents()
    {
        if(!playerGameObject || !killTilemap || !foregroundBody)
        {
            Debug.Log("Missing one or more SerializedFields in GameController");
            Destroy(this);
        }
    }

    private void CreatePlayers()
    {
        players = new Player[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            players[i] = Instantiate(playerGameObject).GetComponent<Player>();
        }
    }

    private void FillDependencies()
    {
        killTilemap.setPlayers(players);
    }
}
