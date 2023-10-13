using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement: MonoBehaviour
{
    //object references
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject combat;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    public Animator animator;

    //movement variables
    private float horizontal;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private bool airJump = false;
    private bool canFlip = true;

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

    //animation variables

    void Update()
    {
        WallSlide();

        //determining when to flip sprite
        if (!isWallJumping)
        {
            Flip();
        }
        

        if (IsFloored() || isWallSliding)
        {
            airJump = true;
        }

        animateMovement();
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
        if (!gameManager.getPaused()) {
            //floored check
            if (context.performed && IsFloored())
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
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsFloored() && horizontal !=0f && rb.velocity.y < 0f)
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
        if (!gameManager.getPaused()) { 
            //check to see if attack animation needs to be cancelled
            if (combat.GetComponent<PlayerCombat>().getAttack())
            {
                combat.GetComponent<PlayerCombat>().FinishAttack1();
            }

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

            if (context.performed && wallJumpingCounter > 0f && !IsFloored())
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
    }

    //function to reset wall jumping
    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    //checks if player is touching the floor
    public bool IsFloored()
    {
        return Physics2D.OverlapCircle(floorCheck.position, 0.2f, floorLayer);
    }

    //checks if player is touching a wall
    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    //function to flip the player sprite
    private void Flip()
    {
        //checks if player is changing direction
        if ((isFacingRight && horizontal <0f) || (!isFacingRight && horizontal >0f) || isWallJumping)
        {
            if (canFlip) {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!gameManager.getPaused())
        {
            horizontal = context.ReadValue<Vector2>().x;
        }
    }

    private void animateMovement()
    {
        animator.SetFloat("HorizontalSpeed", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("VerticalVelocity", rb.velocity.y);
        animator.SetBool("IsWallSliding", isWallSliding);
        animator.SetBool("IsFloored", IsFloored());
    }

    //method to lock the ability to flip (used for attacks)
    public void disableFlip()
    {
        canFlip = false;
    }

    //method to re-enable flip
    public void enableFlip()
    {
        canFlip = true;
    }

    //function that passes damage to player combat
    private void Damage(float[] damageDetails)
    {
        combat.GetComponent<PlayerCombat>().Damage(damageDetails);
    }


}
