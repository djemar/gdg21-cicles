using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // References
    public CharacterController controller;

    private Animator animator;
    public GameObject MainMenu;

    // Variables
    // SerializeField lets you declare a private var and having it accessible inside Unity inspector
    [SerializeField] private float groundCheckDistance; // check character skinWidth
    [SerializeField] private float checkDistance; // check character skinWidth
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float airTime;
    [SerializeField] private float fallingThreshold = 1f;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isGliding = false;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private int doubleJump;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float glidingSpeed;
    [SerializeField] private Vector3 playerVelocity;
    [SerializeField] private float gravity;
    [SerializeField] private float glidingGravity;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float doubleJumpHeight;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float turnSmoothVelocity;
    private Vector3 direction;
    private Vector2 inputVector;
    private bool isTaunting = false;
    private bool isRunning = false;
    private float currentSpeed = 0f;
    private bool canMove = true;
    public StaminaUI stamina;
    public HUDController HUD;

    private bool isPaused = false;


    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        airTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Move();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("PowerUpMelee"))
        {
            FindObjectOfType<AudioManager>().Play("PowerUp");
            HUD.pickUpMeleeWeapon();
        }
    }

    void FixedUpdate()
    {
        if (canMove)
            Move();
    }

    private void Move()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundCheckDistance, groundMask);
        //isGrounded = controller.isGrounded;
        //isFalling = !(Physics.CheckSphere(transform.position, checkDistance, groundMask));

        CheckLand();
        CheckAirTime();

        if (isGrounded && playerVelocity.y < 0)    // stop applying gravity if grounded
        {
            playerVelocity.y = -1f; // small neg value should work better than zero
            playerVelocity.z = 0f;
            isGliding = false;
            stamina.Start();
            doubleJump = 2;
            gravity = Physics2D.gravity.y;
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
        {
            isGliding = false;
            animator.SetBool("isGliding", false);
            playerVelocity.y += gravity * Time.deltaTime; //calculate gravity
        }
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
        if (doubleJump > 0 && canMove)
        {
            if (playerVelocity.y < 0f) playerVelocity.y = 0f;

            if (doubleJump == 2)
            {
                animator.SetTrigger("Jump");
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -2.0f * gravity);
            }
            else
            {
                animator.SetTrigger("DoubleJump");
                playerVelocity.y = Mathf.Sqrt(doubleJumpHeight * -2.0f * gravity);
            }
            doubleJump--;
        }
    }
    private void Glide()
    {
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
        if (value.started)
        {
            TargetRotation();
            Jump();
        }
    }

    public void OnRun(InputAction.CallbackContext value)
    {
        if (value.started) isRunning = true;
        if (value.canceled) isRunning = false;
    }
    public void onGlide(InputAction.CallbackContext value)
    {

        UnityEngine.Debug.Log("onGlide ->" + stamina.canGlide);
        if (value.started && !isGrounded && stamina.canGlide) isGliding = true;
        if (value.canceled || !stamina.canGlide)
        {
            UnityEngine.Debug.Log("exit gliding");
            isGliding = false;
            animator.SetBool("isGliding", false);
        }

    }

    public void OnTaunt(InputAction.CallbackContext value)
    {
        UnityEngine.Debug.Log("Taunted");
        isTaunting = true;
    }

    private void CheckAirTime()
    {
        if (isGrounded || isGliding)
        {
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
        }
    }
    private void CheckLand()
    {
        if (airTime > 0 || isGliding)
        {
            TargetRotation();
            if (isGliding && stamina.canGlide)
            {
                animator.SetBool("isGliding", true);
                // animator.SetBool("isFalling", false);
            }
            else
            {
                //animator.SetBool("isGliding", false);
                animator.SetBool("isFalling", true);
            }
            UnityEngine.Debug.Log("Falling!");

            if (isGrounded && airTime <= fallingThreshold)
            {
                UnityEngine.Debug.Log("Soft Landing!");
                animator.SetBool("isFalling", false);
                animator.SetBool("isGliding", false);
            }

            if (isGrounded && airTime > fallingThreshold)
            {
                UnityEngine.Debug.Log("Hard landing!");
                animator.SetTrigger("Landed");
                animator.SetBool("isFalling", false);
                animator.SetBool("isGliding", false);

            }
        }
    }

    public void StopMovement(int enable)
    {
        Debug.Log("PrintEvent: " + enable.ToString() + " called at: " + Time.time);
        canMove = enable == 0;

    }
    public void OnMenu()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0;
            MainMenu.SetActive(true);

        }
        else
        {
            isPaused = false;
            Time.timeScale = 1;
            MainMenu.SetActive(false);
        }
    }

}
