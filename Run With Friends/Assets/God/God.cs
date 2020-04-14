using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    private GameObject selectedObject;
    private bool objectSelected;

    public GameObject spikeBall;
    int creationCooldown = 0;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        objectSelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        ListenForLeftClick();
        ListenForRightClick();
        //MoveSelectedObject();
    }

    /**
     * Listens for mouse clicks, selects clicked items, releases items
     */
    private void ListenForLeftClick()
    {
        //if god clicks and no object is selected
        if (Input.GetButtonDown("Left Click"))
        {
            //Create ray from camera to mouse position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //display and draw the line it creates (only visible in 3d)
            Debug.Log("Ray origin: " + ray.origin + ", Ray direction: " + ray.direction);
            Debug.DrawRay(ray.origin, ray.direction * 11, Color.red, 10f, false);

            //Object hit by the ray
            RaycastHit2D hit;

            //Check to see if ray intersects with colliders in world
            if (hit = Physics2D.Raycast(ray.origin, ray.direction))
            {
                //Identify the object god has clicked on
                GameObject gameObject = hit.collider.gameObject;
                Debug.Log("You hit something with your click: " + gameObject.name);

                //If god clicked on a falling spike, toggle it
                if (gameObject.tag.Equals("FallingSpike"))
                {
                    gameObject.GetComponent<FallingSpike>().Toggle();
                    return;
                }

                //If god clicked on a spike ball, send it toward the player
                if (gameObject.tag.Equals("SpikeBall"))
                {
                    Rigidbody2D ball = gameObject.GetComponent<Rigidbody2D>();
                    Player player1 = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().players[0];
                    Vector2 newVelocity = player1.rigidbody.position - ball.position;
                    newVelocity = newVelocity.normalized * 10;
                    ball.velocity = newVelocity;
                    return;
                }

                //Select the object
                selectedObject = gameObject;
                objectSelected = true;
            }
            else //They clicked on nothing
            {
                //Deselect any selected object
                objectSelected = false;
                selectedObject = null;
            }
        }
    }

    private void ListenForRightClick()
    {
        //If god right clicks and the creation cooldown has ended
        if (Input.GetButtonDown("Right Click") && creationCooldown <= 0)
        {
            //Create ray from camera to mouse position in game
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //Potential object hit by the ray
            RaycastHit2D hit;

            //Check to see if ray doesn't intersect with colliders in world
            if (!(hit = Physics2D.Raycast(ray.origin, ray.direction)))
            {
                GameObject newSpikeBall = Instantiate(spikeBall);
                newSpikeBall.transform.position = new Vector3(ray.origin.x, ray.origin.y);
                newSpikeBall.GetComponent<SpikeBall>().timedSelfDestruct();

                creationCooldown = 1000;
            }
        }

        creationCooldown -= 1;
    }

    /**
     * Accelerates selected object toward the mouse position while still obeying game physics
     */
    private void MoveSelectedObject()
    {
        //if an object is selected
        if (objectSelected)
        {
            //create a ray from the camera to the cursor position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //The selected object's rigidbody2d
            Rigidbody2D rigidBody = selectedObject.GetComponent<Rigidbody2D>();

            //Set the displacement between the rigidbody and cursor (multiplied by 2) as the velocity of the rigidbody
            rigidBody.velocity = new Vector2(ray.origin.x - rigidBody.position.x, ray.origin.y - rigidBody.position.y) * 2;

            //if the velocity of the object is above a threshold of 12
            if (rigidBody.velocity.magnitude > 12)
            {
                //Normalize the velocity and set the magnitude to 12
                rigidBody.velocity = rigidBody.velocity.normalized * 12;
            }
        }
    }
}
