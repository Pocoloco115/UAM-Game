using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalDrag = rb2d.linearDamping;
        currGravityScale = rb2d.gravityScale;
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        MovementInputHandler();
        JumpInputHandler();
        WallJumpHandler();
        DashInputHandler();
        ChekPlayerPosition();

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

        rb2d.linearDamping = isClimbing ? climbingDrag : originalDrag;
        rb2d.gravityScale = isDashing ? 0 : currGravityScale;
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        Move();
        Jump();
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

    private void JumpInputHandler()
    {
        if(isDashing) { return; }
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && !isClimbing)
        {
            jumpPressed = true;
        }
    }

    private void WallJumpHandler()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isClimbing && !IsGrounded())
        {
            wallJumpPressed = true;
        }
    }

    private void DashInputHandler()
    {
        if (isWalljumpLock || wallJumpPressed) return;

        if (isClimbing)
        {
            dashInput = IsLeftWallCheckColliding() ? 1 : -1;
        }
        else
        {
            dashInput = spriteRenderer.flipX ? -1 : 1;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            Dash();
            StartCoroutine(DashCooldown());
        }
    }

    private void MovementInputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void Dash()
    {
        if (!canDash) return;
        if (isWalljumpLock) return; 

        isDashing = true;

        rb2d.linearVelocity = Vector2.zero; 
        rb2d.gravityScale = 0;             

        rb2d.linearVelocity = new Vector2(dashInput * dashSpeed, 0); 

        StartCoroutine(EndDash());
    }

    private void WallJump()
    {
        if (isDashing) return;
        if (!wallJumpPressed) return;

        rb2d.linearVelocity = new Vector2(IsLeftWallCheckColliding() ? jumpForce * 4 : jumpForce * -4, jumpForce);

        wallJumpPressed = false;

        StartCoroutine(WalljumpLock());
    }

    private void Move()
    {
        if (isDashing) return;

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

        else if (rb2d.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
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

    public void TakeDamage()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
    private void ChekPlayerPosition()
    {
        Vector2 viewportPoint = mainCamera.WorldToViewportPoint(playerTransform.position);
        bool isBellowViewport = viewportPoint.y < 0;
        bool isHorizontallyWithinViewport = viewportPoint.x >= 0 && viewportPoint.x <= 1;
        if (isBellowViewport && isHorizontallyWithinViewport)
        {
            TakeDamage();
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
    }


    void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded() ? Color.green : Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);

        Gizmos.color = IsLeftWallCheckColliding() ? Color.green : Color.red;
        Gizmos.DrawCube(leftWallCheck.position, wallCheckSize);

        Gizmos.color = IsRightWallCheckColliding() ? Color.green : Color.red;
        Gizmos.DrawCube(rightWallCheck.position, wallCheckSize);
    }
}
