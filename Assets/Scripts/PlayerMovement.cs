using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // References
    public CharacterController controller;
    private Animator animator;

    // Variables
    // SerializeField lets you declare a private var and having it accessible inside Unity inspector
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;

    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isFlying = false;
    [SerializeField] private float groundCheckDistance; // check character skinWidth
    [SerializeField] private float checkDistance; // check character skinWidth
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    //public float speed = 1f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    //bool fell = false;


    private Vector3 direction;
    private Vector2 inputVector;
    private Vector3 velocity;

    private bool isJumping = false;
    private bool isRunning = false;
    private bool isTaunting = false;
    public float fallingThreshold = 1f;
    public float maxFallingThreshold = 20f;
    private float initialDistance = 0f;
    private RaycastHit hit;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        var dist = 0f;
        GetHitDistance(out dist);
        initialDistance = dist;
    }
    bool GetHitDistance(out float distance)
    {
        distance = 0f;
        Ray downRay = new Ray(transform.position, -Vector3.up); // this is the downward ray
        if (Physics.Raycast(downRay, out hit))
        {
            distance = hit.distance;
            return true;
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        //Move();         

    }

    void FixedUpdate()
    {
        var dist = 0f;
        if (GetHitDistance(out dist))
        {
            if (initialDistance < dist)
            {
                //Get relative distance
                var relDistance = dist - initialDistance;
                //Are we actually falling?
                if (relDistance > fallingThreshold)
                {
                    //How far are we falling
                    if (relDistance > maxFallingThreshold)
                    {
                        UnityEngine.Debug.Log("Fell off a cliff");
                        isFalling = true;
                    }
                    else UnityEngine.Debug.Log("basic falling!");
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("Infinite Fall");
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Falling To Landing"))
            Move();
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        //isFalling = !(Physics.CheckSphere(transform.position, checkDistance, groundMask));

        if (isGrounded && velocity.y < 0)    // stop applying gravity if grounded
        {
            velocity.y = -2f; // small neg value should work better than zero
        }

        if (isFalling && velocity.y < 0)
        {
            if (isGrounded)
            {
                animator.SetTrigger("Fall");
                isFalling = false;
            }
        }


        direction = new Vector3(-inputVector.y, 0, inputVector.x).normalized;

        if (isGrounded)
        {
            if (direction.magnitude >= 0.1f && !isRunning)
            {
                /* player facing movement direction */
                TargetRotation();
                Walk();

            }
            else if (direction.magnitude >= 0.1f && isRunning)
            {
                /* player facing movement direction */
                TargetRotation();
                Run();
            }
            else
            {
                Idle();

            }
            if (isJumping)    // jump only if grounded (but we will have double jump, so...)
            {
                /* player facing movement direction */
                TargetRotation();
                Jump();
                isJumping = false;
            }

        }

        direction *= moveSpeed;

        controller.Move(direction * Time.deltaTime);

        if (isTaunting)
        {
            Taunt();
            isTaunting = false;
        }

        if (isFlying)
        {
            Fly();
        }
        else velocity.y += gravity * Time.deltaTime; //calculate gravity

        controller.Move(velocity * Time.deltaTime); //apply gravity
    }

    private void Idle()
    {
        animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    }
    private void Walk()
    {
        animator.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
        moveSpeed = walkSpeed;
        
    }
    private void Run()
    {
        animator.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
        moveSpeed = runSpeed;
    }

    private void Jump()
    {
        animator.SetTrigger("Jump");
        velocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravity);
    }
    private void Fly()
    {
        //animator.SetTrigger("Jump");
        velocity.y = 4f;
    }

    private void Taunt()
    {
        animator.SetTrigger("Taunt");
    }

    private void TargetRotation()
    {
        /* player facing movement direction */
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        inputVector = value.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started && isGrounded)
        {
            isJumping = true;
            FindObjectOfType<AudioManager>().Play("PlayerJump");
        }

    }
    public void OnRun(InputAction.CallbackContext value)
    {
        if (value.started) isRunning = true;
        if (value.canceled) isRunning = false;
    }
    public void OnFly(InputAction.CallbackContext value)
    {
        if (value.performed && direction.magnitude < 0.1f) isFlying = true;
        if (value.canceled) isFlying = false;
    }

    public void OnTaunt(InputAction.CallbackContext value)
    {
        UnityEngine.Debug.Log("Taunted");
        isTaunting = true;
    }


}
