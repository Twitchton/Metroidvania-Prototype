using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController: MonoBehaviour
{
    //object references
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    //movement variables
    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;

    //wall sliding variables
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 1f;

    void Update()
    {
        Flip();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //grounded check
        if (context.performed && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        //action ended early
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    //checks if player is touching the floor
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(floorCheck.position, 0.2f, groundLayer);
    }

    //checks if player is touching a wall
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void Flip()
    {
        //checks if player is changing direction
        if ((isFacingRight && horizontal <0f) || (!isFacingRight && horizontal >0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<Vector2>().x;
    }
}
