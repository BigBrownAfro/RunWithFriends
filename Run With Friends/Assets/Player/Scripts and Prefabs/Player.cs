using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //Player Info
    public static int playerCount = 0;
    [SerializeField] private int playerId;

    //Health
    bool isAlive = true;
    int health = 1;
    int invincibilityCooldown = 0;
    int maxInvincibilityCooldown = 120;

    //Physics
    [SerializeField] float maxMoveSpeed = 5f;
    [SerializeField] float maxSprintSpeed = 8f;
    int maxJumpCooldown = 10; 

    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float jumpHeight = 12f;
    int jumpCooldown = 0;
    [SerializeField] float throwSpeed = 1f;
    [SerializeField] float throwHeight = 3f;
    [SerializeField] float frictionStrength = 5f;

    bool isLanded = true;

    //Grabbing
    bool isGrabbing = false;
    GameObject grabbedObject;
    int grabButtonCooldown = 0;

    //Sounds
    public AudioSource audioSource;
    public AudioClip jumpSound;
    public AudioClip landSound;

    //Directions
    enum Direction {left, right};
    Direction lastDirection = Direction.right;

    //Components
    public Rigidbody2D rigidbody;
    public BoxCollider2D feetCollider;
    public CapsuleCollider2D bodyCollider;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        //Increment the player count
        playerCount += 1;

        //Determine which player this is
        playerId = playerCount;

        //If there are more than 4 players, freeze the extras
        if (playerId > 4)
        {
            isAlive = false;
        }

        rigidbody = GetComponent<Rigidbody2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }
        Move();
        Jump();
        CheckForLanding();
        Grab();
        MoveTrophy();
        IncrementCooldowns();
        CheckDeath();
        UpdateAnimator();
    }

    private void Move()
    {
        float horizontalControlThrow = Input.GetAxis("Horizontal_"+playerId); // -1 to 1, the magnitude of the controller in the horizontal direction
        //set the last direction faced based on input
        if(horizontalControlThrow < -0.05)
        {
            lastDirection = Direction.left;
        }
        else if(horizontalControlThrow > 0.05)
        {
            lastDirection = Direction.right;
        }

        bool isSprinting = false;
        if (Input.GetButton("L2_" + playerId) || Mathf.Abs(Input.GetAxis("L2_" + playerId)) > .2)
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

        bool moved = false;//boolean to keep track of whether the player moved or not during this update

        //Move the player based on the strength of the control throw and move speed
        //Restrict the player's ability to accelerate passed their max speeds however if an outside force were to force them above it, allow this but slow them down till max.
        if(rigidbody.velocity.x <= maxSpeed && rigidbody.velocity.x >= -maxSpeed) //if the player is moving below there max speed
        {
            
            if(rigidbody.velocity.x + horizontalControlThrow * moveSpeed * .03f < maxSpeed && rigidbody.velocity.x + horizontalControlThrow * moveSpeed * .03f > -maxSpeed) //if the player remains below max speed after adding on the new speed
            {
                //Add the speed to the player's velocity
                rigidbody.velocity += new Vector2(horizontalControlThrow * moveSpeed * .03f, 0);
            }
            else //if adding the new speed to their velocity would result in velocity over the allowed max speed
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

            if(Mathf.Abs(horizontalControlThrow) > .01)
            {
                moved = true;
            }
        }
        else if ((rigidbody.velocity.x < -maxSpeed && horizontalControlThrow > 0.01) || (rigidbody.velocity.x > maxSpeed && horizontalControlThrow < -0.01)) //If the player is moving over max and they wish to move in the opposite direction
        {
            //Add the speed to the player's velocity
            rigidbody.velocity += new Vector2(horizontalControlThrow * moveSpeed * .03f, 0);

            if (Mathf.Abs(horizontalControlThrow) > .01)
            {
                moved = true;
            }
        }

        //apply ground friction if the player hasn't moved this frame and feet touching ground or other player
        if (!moved && (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || feetCollider.IsTouchingLayers(LayerMask.GetMask("Player"))))
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x * (.90f / frictionStrength), rigidbody.velocity.y);
        }

        //apply max negative vertical velocity if player is rubbing up against a wall
        if (CanWallJump().Equals("Left") && horizontalControlThrow < -.05 &&rigidbody.velocity.y < -2f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -2f);
        }
        else if (CanWallJump().Equals("Right") && horizontalControlThrow > .05 && rigidbody.velocity.y < -2f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -2f);
        }

        //Restrict max negative vertical velocity
        if (rigidbody.velocity.y < -15f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, -15f);
        }
    }

    private void Jump()
    {
        //if the jump button is pressed, the cooldown has ended, and the player is on the ground...
        if (Input.GetButton("A_" + playerId) && jumpCooldown <= 0 && (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || feetCollider.IsTouchingLayers(LayerMask.GetMask("Player"))))
        {
            //propel the player upwards while keeping horizontal velocity
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpHeight);
            //Set the jump cooldown to max so the player can't jump immediately after (double jump while on groung)
            jumpCooldown = maxJumpCooldown;
            audioSource.PlayOneShot(jumpSound);
        }
        else if (Input.GetButton("A_" + playerId) && jumpCooldown <= 0 && CanWallJump().Equals("Left")) //Trying to jump while touching left wall
        {
            //propel the player upwards and away from the wall
            rigidbody.velocity = new Vector2(5f, jumpHeight);
            jumpCooldown = maxJumpCooldown;
            audioSource.PlayOneShot(jumpSound);
        }
        else if(Input.GetButton("A_" + playerId) && jumpCooldown <= 0 && CanWallJump().Equals("Right")) //Trying to jump while touching right wall
        {
            //propel the player upwards and away from the wall
            rigidbody.velocity = new Vector2(-5f, jumpHeight);
            jumpCooldown = maxJumpCooldown;
            audioSource.PlayOneShot(jumpSound);
        }

        //decrement cooldown
        jumpCooldown -= 1;

        //Make sure the cooldown doesn't overflow
        if (jumpCooldown <= 0)
        {
            jumpCooldown = 0;
        }
    }

    private void Grab()
    {
        //Player pushed the 'x' button
        if(Input.GetButton("X_" + playerId) && !isGrabbing && grabButtonCooldown <= 0)
        {
            Debug.Log("Pushed throw");

            //Detect the objects close to the left and right of player
            RaycastHit2D[] nearestObject = new RaycastHit2D[1];
            if(lastDirection == Direction.left)
            {
                //Look for an object to the left
                bodyCollider.Cast(Vector2.left, nearestObject, .3f);

                //if their is no grabbable object to left, try right
                if (!nearestObject[0] || nearestObject[0].rigidbody.bodyType.Equals(RigidbodyType2D.Static) || nearestObject[0].rigidbody.bodyType.Equals(RigidbodyType2D.Kinematic)) 
                {
                    nearestObject = new RaycastHit2D[1];
                    bodyCollider.Cast(Vector2.right, nearestObject, .3f);
                }
            }
            else //(lastDirecion == Direction.right)
            {
                //look for an object to the right
                bodyCollider.Cast(Vector2.right, nearestObject, .3f);

                //if their is no object to right, try left
                if (!nearestObject[0]) 
                {
                    nearestObject = new RaycastHit2D[1];
                    bodyCollider.Cast(Vector2.left, nearestObject, .3f);
                }
            }

            //If there was a grabbable object close to the player in the direction they're facing
            if (nearestObject[0] && !(nearestObject[0].rigidbody.bodyType.Equals(RigidbodyType2D.Static)) && !(nearestObject[0].rigidbody.bodyType.Equals(RigidbodyType2D.Kinematic))) 
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

                //set grab cooldown before being able to throw
                grabButtonCooldown = 20;
            }
        }
        else if (Input.GetButton("X_" + playerId) && isGrabbing && grabButtonCooldown <= 0) //Throw
        {
            isGrabbing = false;
            grabButtonCooldown = 20;

            //release the object by:
            //resetting the mass to 1
            grabbedObject.GetComponent<Rigidbody2D>().mass = 1;
            //destroying the joint
            Destroy(grabbedObject.GetComponent<FixedJoint2D>());
            //add velocity in the direction the player is facing so the object goes that direction
            if(lastDirection == Direction.left)
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity = new Vector2(-6f * throwSpeed, rigidbody.velocity.y + throwHeight);
            }
            else
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity = new Vector2(6f * throwSpeed, rigidbody.velocity.y + throwHeight);
            }
            //If the player is facing the same way they are throwing: the player's momentum will cause the object's initial velocity to be higher
            if (lastDirection == Direction.left && rigidbody.velocity.x < -0.1)
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity += new Vector2(rigidbody.velocity.x * 1f, 0f);
            }
            else if(lastDirection == Direction.right && rigidbody.velocity.x > 0.1)
            {
                grabbedObject.GetComponent<Rigidbody2D>().velocity += new Vector2(rigidbody.velocity.x * 1f, 0f);
            }
        }
    }

    private void CheckForLanding()
    {
        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            if (!isLanded)
            {
                //audioSource.PlayOneShot(landSound);
                isLanded = true;
            }
        }
        else
        {
            isLanded = false;
        }
    }

    public void Hurt(int amount)
    {
        if (invincibilityCooldown <= 0)
        {
            health -= amount;
            invincibilityCooldown = maxInvincibilityCooldown;
        }
    }

    private void IncrementCooldowns()
    {
        invincibilityCooldown -= 1;
        if(invincibilityCooldown < 0)
        {
            invincibilityCooldown = 0;
        }

        grabButtonCooldown -= 1;
        if(grabButtonCooldown < 0)
        {
            grabButtonCooldown = 0;
        }
    }

    private void CheckDeath()
    {
        if (health <= 0 || rigidbody.transform.position.y < -50)
        {
            Kill();
            //StartCoroutine(Kill());
        }
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("xVelocity", rigidbody.velocity.x);
        animator.SetBool("facingLeft", lastDirection == Direction.left);
    }

    public void Kill()
    {
        //Release anything they're holding
        if (isGrabbing)
        {
            isGrabbing = false;

            //release the object by:
            //resetting the mass to 1
            grabbedObject.GetComponent<Rigidbody2D>().mass = 1;
            //destroying the joint
            Destroy(grabbedObject.GetComponent<FixedJoint2D>());
        }

        //Insta revive right now
        health = 1;
        //respawn the player
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().respawnPlayer(playerId);

        //kill the player (stop update loop), stop their animations
        /*
        isAlive = false;
        StartCoroutine(PlayDeathAnimationAndDeactivate());
        Debug.Log("Player " + playerId + " is real dead.");
        */
    }

    private IEnumerator PlayDeathAnimationAndDeactivate()
    {
        rigidbody.simulated = false;
        SpriteRenderer r = GetComponent<SpriteRenderer>();
        float x = 100;
        for(float i = x; i >= 0f; i--)
        {
            transform.Rotate(0, 0, 10f);
            float scale = 1.5f * (i / x);
            transform.localScale = new Vector3(scale, scale, scale);
            yield return new WaitForSeconds(.02f);
        }
        GetComponent<SpriteRenderer>().enabled = false;
    }

    /**
     * Casts raycasts to the left and right of the player to detect whether or not the player can wall jump or not
     * Returns a string Left, Right, or False (Describing which direction the wall is or whether or not it can jump)
     */
    private String CanWallJump()
    {
        //Create arrays of size one to store our raycasts into
        RaycastHit2D[] castLeft = new RaycastHit2D[1];
        RaycastHit2D[] castRight = new RaycastHit2D[1];

        //Cast left
        bodyCollider.Cast(Vector2.left, castLeft, .1f);
        //Cast right
        bodyCollider.Cast(Vector2.right, castRight, .1f);

        //If their is an object to the left and it is the ground (wall)
        if (castLeft[0] && (castLeft[0].collider.attachedRigidbody.bodyType.Equals(RigidbodyType2D.Static) || castLeft[0].collider.attachedRigidbody.bodyType.Equals(RigidbodyType2D.Kinematic)))
        {
            return "Left";
        }

        //If their is an object to the right and it is the ground (wall)
        if (castRight[0] && (castRight[0].collider.attachedRigidbody.bodyType.Equals(RigidbodyType2D.Static) || castRight[0].collider.attachedRigidbody.bodyType.Equals(RigidbodyType2D.Kinematic)))
        {
            return "Right";
        }

        return "False";
    }

    private void MoveTrophy()
    {
        if(Input.GetButton("B_" + playerId))
        {
            GameObject trophy = GameObject.FindGameObjectWithTag("Trophy");
            Rigidbody2D trophyBody = trophy.GetComponent<Rigidbody2D>();

            Vector2 newVelocity = rigidbody.position - trophyBody.position;
            newVelocity = newVelocity.normalized * 2;

            trophyBody.velocity = newVelocity;
        }
    }
}
