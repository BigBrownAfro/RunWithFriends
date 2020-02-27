using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] [Range(1,4)] int playerCount;
    [SerializeField] GameObject playerGameObject;
    [SerializeField] GameObject godGameObject;
    [SerializeField] GameObject deathWallGameObject;
    [SerializeField] bool enableDeathWall = true;
    KillTile killTilemap;
    Rigidbody2D foregroundBody;
    GameObject spawnZone;
    DeathWall deathWall;

    public Player[] players;
    public God god;

    // Start is called before the first frame update
    void Start()
    {
        //Make cursor invisible so that when "god" is created we know it's working
        Cursor.visible = false;
        InitMap();
        VerifyComponents();
        CreatePlayers();
        CreateGod();
        Activate();
    }

    /**
     * Creates or Find the components for the map
     */
    private void InitMap()
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

    /**
     * Verifies that the map components were either created or located correctly
     */
    private void VerifyComponents()
    {
        if(!playerGameObject || !killTilemap || !foregroundBody || !deathWallGameObject)
        {
            Debug.Log("Missing one or more SerializedFields in GameController");
            Destroy(this);
        }
    }

    /**
     * Create the platforming players based on the playercount variable
     */
    private void CreatePlayers()
    {
        players = new Player[playerCount];

        for (int i = 0; i < playerCount; i++)
        {
            players[i] = Instantiate(playerGameObject).GetComponent<Player>();
            players[i].transform.position = new Vector2(spawnZone.transform.position.x, spawnZone.transform.position.y + 2);
        }
    }

    /**
     * Creates the "God" player that will be manipulating the playing field
     */
    private void CreateGod()
    {
        god = Instantiate(godGameObject).GetComponent<God>();
    }

    /**
     * Activates components that need activation
     */
    private void Activate()
    {
        if (enableDeathWall)
        {
            deathWall.Activate();
        }
        killTilemap.Activate();
    }
}
