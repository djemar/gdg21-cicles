using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private float groundCheckDistance; // check character skinWidth
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;
    //public float speed = 1f;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;


    private Vector3 direction;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)    // stop applying gravity if grounded
        {
            velocity.y = -2f; // small neg value should work better than zero
        }

        direction = new Vector3(-vertical, 0f, horizontal).normalized;

        if (isGrounded)
        {
            if (direction.magnitude >= 0.1f && !Input.GetKey(KeyCode.LeftShift))
            {
                /* player facing movement direction */
                TargetRotation();
                Walk();

            }
            else if (direction.magnitude >= 0.1f && Input.GetKey(KeyCode.LeftShift))
            {
                /* player facing movement direction */
                TargetRotation();
                Run();
            }
            else
            {
                Idle();
            }
            if (Input.GetKeyDown(KeyCode.Space))    // jump only if grounded (but we will have double jump, so...)
            {
                /* player facing movement direction */
                TargetRotation();
                Jump();
            }
            direction *= moveSpeed;

        }

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
        velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
    }

    private void TargetRotation()
    {
        /* player facing movement direction */
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }
}
