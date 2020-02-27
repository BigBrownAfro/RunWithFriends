using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class God : MonoBehaviour
{
    private GameObject selectedObject;
    private bool objectSelected;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        objectSelected = false;
    }

    // Update is called once per frame
    void Update()
    {
        ListenForInput();
        MoveSelectedObject();
    }

    /**
     * Listens for mouse clicks, selects clicked items, releases items
     */
    private void ListenForInput()
    {
        //if god clicks and no object is selected
        if (Input.GetButtonDown("Left Click") && !objectSelected)
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
                GameObject gameObject = hit.collider.gameObject;
                Debug.Log("You hit something with your click: " + gameObject.name);

                selectedObject = gameObject;
                objectSelected = true;
            }
        }
        else if (Input.GetButtonDown("Left Click") && objectSelected) //if god clicks and an object is already selected
        {
            objectSelected = false;
        }
    }

    private void MoveSelectedObject()
    {
        if (objectSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Rigidbody2D rigidBody = selectedObject.GetComponent<Rigidbody2D>();
            rigidBody.velocity = new Vector2(ray.origin.x - rigidBody.position.x, ray.origin.y - rigidBody.position.y) * 2;
            //selectedObject.GetComponent<Rigidbody2D>().position = new Vector3(ray.origin.x, ray.origin.y, 0);
        }
    }
}
