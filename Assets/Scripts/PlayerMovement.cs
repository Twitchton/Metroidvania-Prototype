using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    //object references
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;

    //movement variables
    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;
    [SerializeField] private bool airJump = false;

    //wall sliding variables
    private bool isWallSliding;
    [SerializeField] private float wallSlidingSpeed = 1f;

    //wall jumping
    private bool isWallJumping;
    private float wallJumpingDirection;
    [SerializeField] private float wallJumpingTime;
    private float wallJumpingCounter;
    [SerializeField] private float wallJumpingDuration;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    void Update()
    {
        WallSlide();

        if (!isWallJumping)
        {
            Flip();
        }
        

        if (Isfloored() || IsWalled())
        {
            airJump = true;
        }
        
    }

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        //floored check
        if (context.performed && Isfloored())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        else if (context.performed && !isWallJumping && !isWallSliding && airJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            airJump = false;
        }

        //action ended early
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    private void WallSlide()
    {
        if (IsWalled() && !Isfloored() && horizontal !=0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue)); ;
        }
        else
        {
            isWallSliding = false;
        }

    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x; //jumpiing in oposite direction of player character
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));//return ability to wall jump
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime; //allows player to wall jump for a little time after wall sliding
        }

        if (context.performed && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0;

            if (transform.localScale.x != wallJumpingDirection)
            {
                Flip();
            }
        }

        Invoke(nameof(StopWallJumping), wallJumpingDuration);//remove ability to wall jump
    }

    //function to reset wall jumping
    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    //checks if player is touching the floor
    private bool Isfloored()
    {
        return Physics2D.OverlapCircle(floorCheck.position, 0.2f, floorLayer);
    }

    //checks if player is touching a wall
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void Flip()
    {
        //checks if player is changing direction
        if ((isFacingRight && horizontal <0f) || (!isFacingRight && horizontal >0f) || isWallJumping)
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
