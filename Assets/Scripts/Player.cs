using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float fallMultiplier = 0.5f;
    public float lowJumpMultiplier = 1f;

    private Rigidbody2D rbd2;
    public float groundCheckRadius = 0.0001f;
    public LayerMask groundLayer;

    private bool jumpPressed;

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

        return Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);
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
        Gizmos.DrawSphere(transform.position, groundCheckRadius);
    }
}

