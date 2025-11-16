using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rbd2;

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
    private float originalDrag;

    private float horizontalInput;
    private bool jumpPressed;
    private bool wallJumpPressed;
    private bool isClimbing;
    void Start()
    {
        rbd2 = GetComponent<Rigidbody2D>();
        originalDrag = rbd2.linearDamping;
    }

    void Update()
    {
        MovementInputHandler();
        JumpInputHandler();
        WallJumpHandler();
        if(!isClimbing && IsTryingToClimbWall())
        {
            isClimbing = true;
        }

        if (IsGrounded() || (!IsLeftWallCheckColliding() && !IsRightWallCheckColliding()))
        {
            isClimbing = false;
        }

        rbd2.linearDamping = isClimbing ? climbingDrag : originalDrag;
    }

    void FixedUpdate()
    {
        Move();
        Jump();
        WallJump();
    }


    private void MovementInputHandler()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    private bool IsTryingToClimbLeftWall()
    {
        return IsLeftWallCheckColliding() && horizontalInput < 0;
    }

    private bool IsTryingToClimbRightWall()
    {
        return IsRightWallCheckColliding() && horizontalInput > 0;
    }

    private bool IsTryingToClimbWall()
    {
        return IsTryingToClimbLeftWall() || IsTryingToClimbRightWall();
    }
    private void JumpInputHandler()
    {
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

    private void WallJump()
    {
        if(wallJumpPressed)
        {
            rbd2.linearVelocity = new Vector2(IsLeftWallCheckColliding() ? jumpForce * 4 : jumpForce * -4 , jumpForce); 
            wallJumpPressed = false;
        }
    }

    private void Move()
    {
        rbd2.linearVelocity = new Vector2(horizontalInput * speed, rbd2.linearVelocity.y);
    }

    private void Jump()
    {
        if (jumpPressed)
        {
            rbd2.linearVelocity = new Vector2(rbd2.linearVelocity.x, jumpForce);
            jumpPressed = false; 
        }

        if (rbd2.linearVelocity.y < 0)
        {
            rbd2.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.deltaTime;
        }
        else if (rbd2.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rbd2.linearVelocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;
        }
    }


    private bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
    }

    private bool IsLeftWallCheckColliding()
    {
        return Physics2D.OverlapBox(leftWallCheck.position, wallCheckSize, 0, wallLayer) && !IsGrounded();
    }
    private bool IsRightWallCheckColliding()
    {
        return Physics2D.OverlapBox(rightWallCheck.position, wallCheckSize, 0, wallLayer) && !IsGrounded();
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


