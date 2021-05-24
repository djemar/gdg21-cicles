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
    [SerializeField] private float groundCheckDistance; // check character skinWidth
    [SerializeField] private float checkDistance; // check character skinWidth
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float fallingThreshold = 1f;
    [SerializeField] private float maxFallingThreshold = 20f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isGliding = false;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float glidingSpeed;
    [SerializeField] private Vector3 playerVelocity;

    [SerializeField] private float gravity;
    [SerializeField] private float glidingGravity;
    [SerializeField] private float jumpHeight;
    //public float speed = 1f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float turnSmoothVelocity;
    //bool fell = false;

    //bool fell = false;
    private Vector3 direction;
    private bool isTaunting = false;
    private Vector2 inputVector;

    private bool isJumping = false;

    private bool isRunning = false;
    private float initialDistance = 0f;

    private RaycastHit hit;
    private float currentSpeed = 0f;

    public StaminaUI stamina;

    private bool powerUp = false;

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

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("PowerUp"))
        {
            Destroy(collision.gameObject);
            FindObjectOfType<AudioManager>().Play("PowerUp");
            powerUp = true;
            //TODO trigger some event: unlock new weapon, glide, double jump, ...
            StartCoroutine(PowerDown(5f));
        }
    }

    private IEnumerator PowerDown(float secs)
    {
        yield return new WaitForSecondsRealtime(secs);
        FindObjectOfType<AudioManager>().Play("PowerDown");
        // TODO Revert back to normal state if needed, because power up is gone
        powerUp = false;
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
                    else
                        UnityEngine.Debug.Log("basic falling!");
                }
            }
        }
        else
        {
            UnityEngine.Debug.Log("Infinite Fall");
        }
        if (
            !animator
                .GetCurrentAnimatorStateInfo(0)
                .IsName("Falling To Landing")
        ) Move();

        if (powerUp)
        {
            // TODO Do something here while power up is on
        }
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        //isGrounded = controller.isGrounded;
        //isFalling = !(Physics.CheckSphere(transform.position, checkDistance, groundMask));

        if (isGrounded && playerVelocity.y < 0)    // stop applying gravity if grounded
        {
            playerVelocity.y = 0f; // small neg value should work better than zero
            playerVelocity.z = 0f;
            isGliding = false;
            gravity = Physics2D.gravity.y;
        }

        if (isFalling && playerVelocity.y < 0)
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
            if (
                isJumping // jump only if grounded (but we will have double jump, so...)
            )
            {
                /* player facing movement direction */
                TargetRotation();
                Jump();
                isJumping = false;

            }
        }
        if (moveSpeed == runSpeed)
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, 0.1f);
        else
            currentSpeed = moveSpeed;
        direction *= currentSpeed;

        controller.Move(direction * Time.deltaTime);


        if (isGliding && stamina.canGlide)
        {
            TargetRotation();
            Glide();
        }
        else
            playerVelocity.y += gravity * Time.deltaTime; //calculate gravity
        controller.Move(playerVelocity * Time.deltaTime); //apply gravity

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
        playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
    }
    private void Glide()
    {
        animator.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
        StaminaUI.instance.UseStamina(1);
        moveSpeed = glidingSpeed;
        playerVelocity.y = glidingGravity;
    }

    private void Taunt()
    {
        animator.SetTrigger("Taunt");
    }

    private void TargetRotation()
    {
        /* player facing movement direction */
        float targetAngle =
            Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle =
            Mathf
                .SmoothDampAngle(transform.eulerAngles.y,
                targetAngle,
                ref turnSmoothVelocity,
                turnSmoothTime);
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
    public void onGlide(InputAction.CallbackContext value)
    {
        if (value.started && !isGrounded) isGliding = true;
        if (value.canceled) isGliding = false;
    }

    public void OnTaunt(InputAction.CallbackContext value)
    {
        UnityEngine.Debug.Log("Taunted");
        isTaunting = true;
    }
}
