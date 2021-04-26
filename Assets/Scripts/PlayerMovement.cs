using System;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float groundCheckDistance; // check character skinWidth
    [SerializeField] private float checkDistance; // check character skinWidth
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    //public float speed = 1f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;
    bool fell = false;


    private Vector3 direction;
    private Vector2 inputVector;
    private Vector3 velocity;

    private bool isJumping = false;
    private bool isRunning = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Move();
    }

    void FixedUpdate()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Falling To Landing"))
            Move();
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        isFalling = Physics.CheckSphere(transform.position, checkDistance, groundMask);

        if (isGrounded && velocity.y < 0)    // stop applying gravity if grounded
        {
            velocity.y = -2f; // small neg value should work better than zero
            fell = false;

        }

        if (!isFalling && !isGrounded && !fell)
        {
            animator.SetTrigger("Fall");
            fell = true;
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

        velocity.y += gravity * Time.deltaTime; //calculate gravity
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
        }

    }

    public void OnRun(InputAction.CallbackContext value)
    {
        if (value.started) isRunning = true;
        if (value.canceled) isRunning = false;
    }


}