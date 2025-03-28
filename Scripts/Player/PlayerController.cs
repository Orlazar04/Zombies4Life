// Golden Version
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieSpace;

// This script represents the player's movement.
// Dependencies: Level State
// Main Contributors: Olivia Lazar
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float jumpHeight = 2;
    [SerializeField]
    private float airControl = 10;
    [SerializeField]
    private float gravity = 9.81f;

    private CharacterController controller;
    private Vector3 input, moveDirection;

    private int movementState;
    private Animator animator;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {   
        // While the level is active
        if(LevelManager.IsLevelActive())
        {
            // Get movement input
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            input = (transform.right * moveHorizontal + transform.forward * moveVertical).normalized;
            input *= moveSpeed;

            UpdateSprinting();
            UpdateJumping();
            UpdateMovementAnimation();
            
            // Gravity
            moveDirection.y -= gravity * Time.deltaTime;        

            // Move player
            controller.Move(moveDirection * Time.deltaTime);
        }
        else if (LevelManager.levelState == LevelState.Over)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    // Updates sprinting
    private void UpdateSprinting()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            input *= 2;
            ////animator.SetBool("isRunning", true);
        }
        else
        {
            ////animator.SetBool("isRunning", false);
        }
    }

    // Updates jumping
    private void UpdateJumping()
    {
        if(controller.isGrounded)
        {
            moveDirection = input;
            if(Input.GetButton("Jump"))
            {
                moveDirection.y = Mathf.Sqrt(2 * jumpHeight * gravity);
                ////animator.SetTrigger("jump");
            }
            else
            {
                moveDirection.y = 0;
            }
        }
        else
        {
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }
    }

    // Updates the animation of the player based on the direction of movement
    private void UpdateMovementAnimation()
    {
        Vector3 direction = moveDirection.normalized;
        direction.y = transform.forward.y;
        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.forward);

        // Idle
        if(direction.x == 0 && direction.z == 0)
        {
            //animator.SetInteger("movementState", 0);
        }
        // Forward
        else if(angle <= 45 && angle >= -45)
        {
            //animator.SetInteger("movementState", 1);
        }
        // Backward
        else if(angle >= 135 && angle <= -135)
        {
            //animator.SetInteger("movementState", 2);
        }
        // Left
        else if(angle > 45 && angle < 135)
        {
            //animator.SetInteger("movementState", 3);
        }
        // Right
        else if(angle < -45 && angle > -135)
        {
            //animator.SetInteger("movementState", 4);
        }
    }
}