using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyPlayer : MonoBehaviour
{
    float maxMoveSpeed = 5f;
    float maxSprintSpeed = 8f;
    int maxJumpCooldown = 10;

    float moveSpeed = 8f;
    float sprintMultiplier = 1.3f;
    float jumpHeight = 10f;
    int jumpCooldown = 0;
    float frictionStrength = 1f;

    bool isGrabbing = false;
    GameObject grabbedObject;

    enum Direction { left, right };
    Direction lastDirection = Direction.right;

    Rigidbody2D rigidbody;
    BoxCollider2D feetCollider;
    CapsuleCollider2D bodyCollider;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        //Jump(); //dummy can't jump
        //Grab(); //dummy don't grab
    }

    private void Move()
    {
        float horizontalControlThrow = 0; //dummy has no horizontal controls

        //set the last direction faced based on input
        if (horizontalControlThrow < -0.05)
        {
            lastDirection = Direction.left;
        }
        else if (horizontalControlThrow > 0.05)
        {
            lastDirection = Direction.right;
        }

        bool isSprinting = false;
        if (Input.GetButton("Sprint") || Mathf.Abs(Input.GetAxis("Sprint")) > .2)
        {
            isSprinting = true;
        }
        float maxSpeed;


        //Decide max horizontal speed and acceleration based on if sprinting or not
        if (isSprinting)
        {
            maxSpeed = maxSprintSpeed;
        }
        else
        {
            maxSpeed = maxMoveSpeed;
        }

        //Move the player based on the strength of the control throw and move speed
        //Restrict the player's ability to accelerate passed their max speeds however if an outside force were to force them above it, allow this but slow them down till max.
        if (rigidbody.velocity.x <= maxSpeed && rigidbody.velocity.x >= -maxSpeed) //if the player is moving below there max speed
        {
            if (rigidbody.velocity.x + horizontalControlThrow * moveSpeed * .03f < maxSpeed && rigidbody.velocity.x + horizontalControlThrow * moveSpeed * .03f > -maxSpeed) //if the player remains below max speed after adding on the new speed
            {
                //Add the speed to the player's velocity
                rigidbody.velocity += new Vector2(horizontalControlThrow * moveSpeed * .03f, 0);
            }
            else //if adding the new speed to their velocity would result in velocity over the allow max speed
            {
                //set the player to their max velocity
                if (rigidbody.velocity.x < 0)
                {
                    rigidbody.velocity = new Vector2(-maxSpeed, rigidbody.velocity.y);
                }
                else
                {
                    rigidbody.velocity = new Vector2(maxSpeed, rigidbody.velocity.y);
                }
            }

        }

        //apply ground friction if not running and feet touching ground or other player
        if (Mathf.Abs(horizontalControlThrow) < .01f && (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || feetCollider.IsTouchingLayers(LayerMask.GetMask("Player"))))
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * (.90f / frictionStrength), rigidbody.velocity.y);
        }

        //Restrict max negative vertical velocity
        if (rigidbody.velocity.y < -20f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -20f);
        }

        //Update the animator
        animator.SetFloat("xVelocity", rigidbody.velocity.x);
    }

    private void Jump()
    {
        //if the jump button is pressed, the cooldown has ended, and the player is on the ground...
        //jump
        if (Input.GetButton("Jump") && jumpCooldown <= 0 && (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || feetCollider.IsTouchingLayers(LayerMask.GetMask("Player"))))
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpHeight);
            jumpCooldown = maxJumpCooldown;
        }

        jumpCooldown -= 1;

        if (jumpCooldown <= 0)
        {
            jumpCooldown = 0;
        }
    }

    private void Grab()
    {
        if (Input.GetButtonDown("Grab") && !isGrabbing)
        {
            //Detect the objects close to the left and right of player
            RaycastHit2D[] nearestObject = new RaycastHit2D[1];
            if (lastDirection == Direction.left)
            {
                bodyCollider.Cast(Vector2.left, nearestObject, .3f);
            }
            else //(lastDirecion == Direction.right)
            {
                bodyCollider.Cast(Vector2.right, nearestObject, .3f);
            }

            //If there was an object close to the player in the direction they're facing
            if (nearestObject[0])
            {
                //grab the nearest object
                grabbedObject = nearestObject[0].collider.gameObject;

                //position the grabbed object above the player
                grabbedObject.GetComponent<Rigidbody2D>().position = new Vector2(rigidbody.position.x, rigidbody.position.y + 1f);
                //reduce its mass to 0 so it can be carried easily
                grabbedObject.GetComponent<Rigidbody2D>().mass = 0;
                //attach a joint component to the object
                FixedJoint2D joint = grabbedObject.AddComponent<FixedJoint2D>();
                joint.anchor.Set(grabbedObject.GetComponent<Rigidbody2D>().position.x, grabbedObject.GetComponent<Rigidbody2D>().position.y);
                //connect the player to the joint so the object follows the player
                joint.connectedBody = rigidbody;
                isGrabbing = true;
            }
        }
        else if (Input.GetButtonDown("Grab") && isGrabbing)
        {
            isGrabbing = false;

            //release the object by:
            //resetting the mass to 1
            grabbedObject.GetComponent<Rigidbody2D>().mass = 1;
            //destroying the joint
            Destroy(grabbedObject.GetComponent<FixedJoint2D>());
            //adding velocity in the direction the player is facing so the object goes that direction
            if (lastDirection == Direction.left)
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-6f, 1f);
            }
            else
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity = new Vector2(6f, 1f);
            }
            //If the player is facing the same way they are throwing: the player's momentum will cause the object's initial velocity to be higher
            if (lastDirection == Direction.left && rigidbody.velocity.x < -0.1)
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity += new Vector2(rigidbody.velocity.x * 1f, 1f);
            }
            else if (lastDirection == Direction.right && rigidbody.velocity.x > 0.1)
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity += new Vector2(rigidbody.velocity.x * 1f, 1f);
            }
        }
    }
}
