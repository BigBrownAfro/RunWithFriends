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
        /*if (Input.GetButton("Sprint") || Mathf.Abs(Input.GetAxis("Sprint")) > .2)
        {
            isSprinting = true;
        }*/
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
}
