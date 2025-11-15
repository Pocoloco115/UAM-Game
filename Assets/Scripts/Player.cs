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

    void Start()
    {
        rbd2 = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    public void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded())
        {
            rbd2.linearVelocity = new Vector2(rbd2.linearVelocity.x, jumpForce);
        }
        if(rbd2.linearVelocity.y < 0)
        {
            rbd2.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.deltaTime;
        }
        else if (rbd2.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rbd2.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier) * Time.deltaTime;
        }
    }

    public bool isGrounded()
    {

        return Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer);
    }

    public void Move()
    {

        if (Input.GetKey(KeyCode.D))
        {
            rbd2.linearVelocity = new Vector2(speed, rbd2.linearVelocity.y);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            rbd2.linearVelocity = new Vector2(-speed, rbd2.linearVelocity.y);
        }
        else
        {
            rbd2.linearVelocity = new Vector2(0, rbd2.linearVelocity.y);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded() ? Color.green : Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
    }
}

