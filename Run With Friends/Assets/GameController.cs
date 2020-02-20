using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] [Range(1,4)] int playerCount;
    [SerializeField] GameObject playerGameObject;
    [SerializeField] GameObject deathWallGameObject;
    [SerializeField] bool enableDeathWall = true;
    KillTile killTilemap;
    Rigidbody2D foregroundBody;
    GameObject spawnZone;
    DeathWall deathWall;

    public Player[] players;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        VerifyComponents();
        CreatePlayers();
        Activate();
    }

    private void Init()
    {
        killTilemap = GameObject.FindGameObjectWithTag("KillTilemap").GetComponent<KillTile>();
        foregroundBody = GameObject.FindGameObjectWithTag("ForegroundTilemap").GetComponent<Rigidbody2D>();
        spawnZone = GameObject.FindGameObjectWithTag("SpawnZone");
        deathWall = Instantiate(deathWallGameObject).GetComponent<DeathWall>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void VerifyComponents()
    {
        if(!playerGameObject || !killTilemap || !foregroundBody || !deathWallGameObject)
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
            players[i].transform.position = new Vector2(spawnZone.transform.position.x, spawnZone.transform.position.y + 2);
        }
    }

    private void Activate()
    {
        if (enableDeathWall)
        {
            deathWall.Activate();
        }
        killTilemap.Activate();
    }
}
