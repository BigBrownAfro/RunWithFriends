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
    [SerializeField] GameObject trophyGameObject;
    [SerializeField] bool enableDeathWall = true;

    KillTile killTilemap;
    GameObject[] fallingSpikes;
    Rigidbody2D foregroundBody;
    GameObject spawnZone;
    GameObject endZone;
    DeathWall deathWall;
    ProgressBar progressBar;

    public Player[] players;
    public God god;
    public GameObject trophy;

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
        endZone = GameObject.FindGameObjectWithTag("EndZone");
        deathWall = Instantiate(deathWallGameObject).GetComponent<DeathWall>();
        deathWall.transform.position = new Vector3(spawnZone.transform.position.x - 10, spawnZone.transform.position.y, 0);
        trophy = Instantiate(trophyGameObject);
        trophy.transform.position = new Vector3(spawnZone.transform.position.x + 3, spawnZone.transform.position.y + 5, 0);
        fallingSpikes = GameObject.FindGameObjectsWithTag("FallingSpike");
        progressBar = GameObject.FindGameObjectWithTag("ProgressBar").GetComponent<ProgressBar>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckWin();
        UpdateProgressBar();
    }

    private void CheckWin()
    {
        //for each player
        for(int i = 0; i < players.Length; i++)
        {
            //Check to see if the player has crossed the finish line
            if (players[i].rigidbody.position.x > endZone.transform.position.x)
            {
                respawnPlayer(i + 1);
            }
        }

        if(trophy.transform.position.x > endZone.transform.position.x)
        {
            trophy.transform.localScale += new Vector3(.01f, .01f, .01f);
        }
    }

    private void UpdateProgressBar()
    {
        float spawnX = spawnZone.transform.position.x; //The x position of the start of the map
        float totalMapLength = endZone.transform.position.x - spawnX; //length of the map

        //Find which player has traveled the furthest
        float furthestDistance = -10000;
        for (int i = 0; i < players.Length; i++)
        {
            float playerDistance = players[i].rigidbody.position.x - spawnX; //player distance from the spawn zone

            //Check to see if the player's distance is further than the current furthest
            if (playerDistance > furthestDistance)
            {
                furthestDistance = playerDistance;
            }
        }

        //Update the progress bar given the percentage of the map complete
        progressBar.UpdateProgress(furthestDistance / totalMapLength);
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
        //Enable the deathwall if the option is enabled
        if (enableDeathWall)
        {
            deathWall.Activate();
        }

        //Enable the harmful tilemap
        killTilemap.Activate();

        //Enable the falling spikes
        for(int i = 0; i < fallingSpikes.Length; i++)
        {
            fallingSpikes[i].GetComponent<KillTile>().Activate();
        }
    }

    public void respawnPlayer(int playerId)
    {
        //Set the position of the player to the spawn zone
        players[playerId - 1].rigidbody.position = new Vector2(spawnZone.transform.position.x, spawnZone.transform.position.y + 2);
    }
}
