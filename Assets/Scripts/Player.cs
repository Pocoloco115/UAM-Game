using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb2d;
    [Header("Movement Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float fallMultiplier = 0.5f;
    [SerializeField] private float lowJumpMultiplier = 1f;

    [Header("Ground Check Settings")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;

    [Header("Top Check Settings")]
    [SerializeField] private Transform topCheck;
    [SerializeField] private Vector2 topCheckSize;
    [SerializeField] private LayerMask topLayer;

    [Header("Wall Check Settings")]
    [SerializeField] private Transform leftWallCheck;
    [SerializeField] private Transform rightWallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float climbingDrag = 5f;
    [SerializeField] private float walljumpLockTime = 0.12f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashCoolDown = 0.5f;
    [SerializeField] private bool canDash = true;

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    private bool isWalljumpLock = false;

    private SpriteRenderer spriteRenderer;

    private float dashInput;
    private float originalDrag;
    private float horizontalInput;
    private float currGravityScale;
    private bool jumpPressed;
    private bool wallJumpPressed;
    private bool isClimbing;
    private bool isDashing;
    private Animator animator;
    private Transform playerTransform;
    private int jumpCount = 2;
    private bool isDying = false;
    private bool wasInsideViewport = true;
    private bool lockFlip = false;
    private HashSet<int> platformsId;
    private int counter = 0;
    private KeyCode KeyLeft => InputSettingsManager.GetOrCreate().settings.moveLeft;
    private KeyCode KeyRight => InputSettingsManager.GetOrCreate().settings.moveRight;
    private KeyCode KeyJump => InputSettingsManager.GetOrCreate().settings.jump;
    private KeyCode KeyDash => InputSettingsManager.GetOrCreate().settings.dash;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalDrag = rb2d.linearDamping;
        currGravityScale = rb2d.gravityScale;
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        platformsId = new HashSet<int>();
    }

    void Update()
    {
        MovementInputHandler();
        JumpInputHandler();
        WallJumpHandler();
        DashInputHandler();
        CheckPlayerPosition();
        DoubleJumpInputHandler();

        if (!isClimbing && IsTryingToClimbWall())
        {
            isClimbing = true;
        }

        if (IsGrounded() || (!IsLeftWallCheckColliding() && !IsRightWallCheckColliding()))
        {
            isClimbing = false;
        }
        if (isClimbing) 
        {
            if (IsLeftWallCheckColliding())
            {
                spriteRenderer.flipX = false;
            }
            else if (IsRightWallCheckColliding())
            {
                spriteRenderer.flipX = true;
            }
        }
        if (isClimbing) { ReloadJumpCounter(); }

        rb2d.linearDamping = isClimbing ? climbingDrag : originalDrag;
        rb2d.gravityScale = isDashing ? 0 : currGravityScale;
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        DoubleJump();
        WallJump();
    }


    private bool IsTryingToClimbLeftWall()
    {
        return IsLeftWallCheckColliding() && (horizontalInput < 0 || dashInput < 0);
    }

    private bool IsTryingToClimbRightWall()
    {
        return IsRightWallCheckColliding() && (horizontalInput > 0 || dashInput > 0);
    }

    private bool IsTryingToClimbWall()
    {
        return IsTryingToClimbLeftWall() || IsTryingToClimbRightWall();
    }

    private void MovementInputHandler()
    {
        horizontalInput = 0f;
        if (Input.GetKey(KeyLeft)) { horizontalInput -= 1f; }
        if (Input.GetKey(KeyRight)) { horizontalInput += 1f; }

        if (!lockFlip)
        {
            if (horizontalInput < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (horizontalInput > 0)
            {
                spriteRenderer.flipX = false;

            }
        }
    }

    private void JumpInputHandler()
    {
        if (isDashing) { return; }
        if (Input.GetKeyDown(KeyJump) && IsGrounded() && !isClimbing)
        {
            jumpPressed = true;
            SubstractJumpCounter();
        }
    }

    private void WallJumpHandler()
    {
        if (Input.GetKeyDown(KeyJump) && isClimbing && !IsGrounded())
        {
            wallJumpPressed = true;
            SubstractJumpCounter();
        }
    }

    private void DashInputHandler()
    {
        if (isWalljumpLock || wallJumpPressed) { return; }

        if (isClimbing)
        {
            dashInput = IsLeftWallCheckColliding() ? 1 : -1;

        }
        else
        {
            dashInput = spriteRenderer.flipX ? -1 : 1;
        }

        if (Input.GetKeyDown(KeyDash) && canDash)
        {
            Dash();
            StartCoroutine(DashCooldown());
        }
    }

    private void DoubleJumpInputHandler()
    {
        if (isDashing) { return; }
        if (Input.GetKeyDown(KeyJump) && jumpCount > 0 && !IsGrounded() && !isClimbing)
        {
            jumpPressed = true;
            SubstractJumpCounter();
        }
    }

    private void DoubleJump()
    {
        Jump();
    }
    private void Dash()
    {
        if (!canDash) { return; }
        if (isWalljumpLock) { return; } 

        isDashing = true;

        rb2d.linearVelocity = Vector2.zero; 
        rb2d.gravityScale = 0;             

        rb2d.linearVelocity = new Vector2(dashInput * dashSpeed, 0); 

        StartCoroutine(EndDash());
    }

    private void WallJump()
    {
        if (isDashing) { return; }
        if (!wallJumpPressed) { return; }

        rb2d.linearVelocity = new Vector2(IsLeftWallCheckColliding() ? jumpForce * 4 : jumpForce * -4, jumpForce);

        wallJumpPressed = false;

        StartCoroutine(WalljumpLock());
    }

    private void Move()
    {
        if (isDashing) { return; }

        rb2d.linearVelocity = new Vector2(horizontalInput * speed, rb2d.linearVelocity.y);

    }

    private void Jump()
    {
        if (jumpPressed)
        {
            rb2d.linearVelocity = new Vector2(rb2d.linearVelocity.x, jumpForce);
            jumpPressed = false;
        }

        if (rb2d.linearVelocity.y < 0)
        {
            rb2d.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }

        else if (rb2d.linearVelocity.y > 0 && !Input.GetKey(KeyJump))
        {
            rb2d.linearVelocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;
        }
    }

    private IEnumerator DashCooldown()
    {
        canDash = false;
        yield return new WaitForSeconds(dashCoolDown);
        canDash = true;
    }

    private IEnumerator EndDash()
    {
        yield return new WaitForSeconds(0.2f);
        isDashing = false;
        rb2d.gravityScale = currGravityScale;
    }

    private IEnumerator WalljumpLock()
    {
        isWalljumpLock = true;
        yield return new WaitForSeconds(walljumpLockTime);
        isWalljumpLock = false;
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
    }

    private bool IsLeftWallCheckColliding()
    {
        return Physics2D.OverlapBox(leftWallCheck.position, wallCheckSize, 0, wallLayer)&& !IsGrounded();
    }

    private bool IsRightWallCheckColliding()
    {
        return Physics2D.OverlapBox(rightWallCheck.position, wallCheckSize, 0, wallLayer)&& !IsGrounded();
    }

    private bool IsTopBlocked()
    {
        return Physics2D.OverlapBox(topCheck.position, topCheckSize, 0, topLayer);
    }


    public IEnumerator TakeDamage()
    {
        isDying = true;
        lockFlip = true;
        rb2d.linearVelocity = Vector2.zero;
        rb2d.gravityScale = 0;
        rb2d.constraints = RigidbodyConstraints2D.FreezePosition;
        mainCamera.GetComponent<CameraController>().FreezeCamera();
        yield return new WaitForSeconds(0.30f);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    private void CheckPlayerPosition()
    {
        Vector2 viewportPoint = mainCamera.WorldToViewportPoint(playerTransform.position);

        bool isInsideX = viewportPoint.x >= 0f && viewportPoint.x <= 1f;
        bool isInsideY = viewportPoint.y >= 0f && viewportPoint.y <= 1f;
        bool isInsideViewport = isInsideX && isInsideY;

        if (wasInsideViewport && !isInsideViewport)
        {
            StartCoroutine(TakeDamage());
        }
        wasInsideViewport = isInsideViewport;
    }

    private void IncreaseCounter()
    {
        counter++;
    }

    public int GetCounter() { return counter; }

    private void ReloadJumpCounter()
    {
        if((IsGrounded() || isClimbing) && !IsTopBlocked())
        {
            jumpCount = 2;
        }
    }

    private void SubstractJumpCounter()
    {
        if(jumpCount <= 0) { return; }
        jumpCount--;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null)
        {
            ReloadJumpCounter();
            int currPlatformId = collision.gameObject.GetInstanceID();
            if(platformsId.Contains(currPlatformId))
            {
                return;
            }
            platformsId.Add(currPlatformId);
            IncreaseCounter();
        }
    }

    private void UpdateAnimations()
    {
        bool grounded = IsGrounded();
        float velY = rb2d.linearVelocity.y;

        animator.SetBool("isDashing", isDashing);

        animator.SetBool("isClimbing", isClimbing && !isDashing);

        animator.SetBool("isIdle",
            grounded &&
            horizontalInput == 0 &&
            !isDashing &&
            !isClimbing
        );

        animator.SetBool("isWalking",
            grounded &&
            Mathf.Abs(horizontalInput) > 0.1f &&
            !isDashing &&
            !isClimbing
        );

        animator.SetBool("isJumping",
            !grounded &&
            velY > 0.1f &&
            !isDashing &&
            !isClimbing
        );

        animator.SetBool("isFalling",
            !grounded &&
            velY < -0.1f &&
            !isDashing &&
            !isClimbing
        );

        animator.SetBool("doubleJump",
            !grounded &&
            jumpCount == 1 &&
            velY > 0.1f &&
            !isDashing &&
            !isClimbing
        );

        animator.SetBool("isDying",
            isDying == true
        );
    }


    void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);

        Gizmos.color = IsLeftWallCheckColliding() ? Color.green : Color.red;
        Gizmos.DrawCube(leftWallCheck.position, wallCheckSize);

        Gizmos.color = IsRightWallCheckColliding() ? Color.green : Color.red;
        Gizmos.DrawCube(rightWallCheck.position, wallCheckSize);

        Gizmos.color = IsTopBlocked() ? Color.green : Color.red;
        Gizmos.DrawCube(topCheck.position, topCheckSize);
    }
}
