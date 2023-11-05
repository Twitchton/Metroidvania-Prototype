using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    //Object references
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private GameObject player;
    [SerializeField] private Transform attack1HitboxPos;
    [SerializeField] private LayerMask Damageable;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerMovementInput;
    public Animator animator;

    //Open Variables
    [SerializeField] private float inputTimer; //timing to fudge attack time
    [SerializeField] private float attack1Radius;
    [SerializeField] private float attack1Damage;
    [SerializeField] private float lastInputTime;
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float directionTime;
    [SerializeField] private Vector2 knockbackSpeed;
    [SerializeField] private Vector2 dashPower;

    //private variables
    private float[] attackDetails = new float[2];
    [SerializeField] private float health, mana, gravityScale;
    private float invincibilityTimer, dashTimer, directionTimer;
    [SerializeField] private bool isAttacking, attackCheck, isFirstAttack, gotInput, attack1, attack2, dashAttack, invincible, dashing, dashCheck, waitForAnim, downInput, upInput;
    private int damageDirection;

    //function called on load
    private void Start()
    {
        //setting initial variables
        lastInputTime = Mathf.NegativeInfinity;

        health = maxHealth;
        mana = maxMana;

        invincible = false;
        invincibilityTimer = 0f;
        dashTimer = 0f;
        directionTimer = 0f;

        waitForAnim = false;

        downInput = false;
        upInput = false;
    }

    //function called each frame
    private void Update()
    {

        CheckAttacks();

        if(invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        //invicibility timer.
        if (invincibilityTimer <= 0f && !dashing)
        {
            invincible = false;
        }

        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }

        if (directionTimer <= 0)
        {
            downInput = false;
            upInput = false;
        }

        if (directionTimer > 0f)
        {
            directionTimer -= Time.deltaTime;
        }

        //checks to see is there's a mismatch in animation and boolean for dashing
        if (dashCheck && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dash NoDust"))
        {
            EndDash();
        }
    }

    //function for catching inputs from player.
    public void AttackAction(InputAction.CallbackContext context)
    {
        if (context.performed && !gameManager.getPaused())
        {
            gotInput = true;
            lastInputTime = Time.time;
            //attempt combat
        }

        if (context.canceled && !gameManager.getPaused())
        {
            gotInput = false;
        }

    }

    //gets input for a verticle "up" attack
    public void UpInput(InputAction.CallbackContext context)
    {
        if (context.performed && !gameManager.getPaused())
        {
            upInput = true;
            directionTimer = directionTime;

            if (downInput)
            {
                downInput = false;
            }
        }
    }

    //gets input for a verticle "down" attack
    public void DownInput(InputAction.CallbackContext context)
    {
        if (context.performed && !gameManager.getPaused())
        {
            if (context.performed && !gameManager.getPaused())
            {
                downInput = true;
                directionTimer = directionTime;

                if (upInput)
                {
                    upInput = false;
                }
            }
        }
    }

    //function to give player a dodge/dash
    public void Dash(InputAction.CallbackContext context)
    {
        if (dashTimer <= 0f && !gameManager.getPaused())
        {
            gravityScale = rb.gravityScale;
            rb.gravityScale = 0f;
            dashing = true;
            animator.SetBool("Dashing", dashing);
            movement.Dash(dashPower);
            dashTimer = dashCooldown;
        }
    }

    //function that ends Dash
    private void EndDash()
    {
        EnableEnemeyCollisions();

        rb.gravityScale = gravityScale;
        dashing = false;
        dashCheck = false;
        animator.SetBool("Dashing", dashing);
        movement.endDash();
    }

    private void DashCheck()
    {
        dashCheck = true;
    }

    //handles logic for the attacks
    private void CheckAttacks()
    {
        if (gotInput)
        {
            //perform attack1
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            //wait for new input
            gotInput = false;
        }
    }

    private void AttackCheck()
    {
        attackCheck = true;
    }

    //check hitbox for landing attack
    private void CheckAttackHitbox()
    {
        //creates a circle collider to check for attack
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitboxPos.position, attack1Radius, Damageable);

        attackDetails[0] = attack1Damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
           collider.transform.gameObject.SendMessage("Damage", attackDetails);
        }
    }

    //function to take damage
    public void Damage(float[] damageDetails)
    {
        if (!invincible)
        {
            //getting array of values for the attack
            health -= damageDetails[0];

            if (damageDetails[1] > player.transform.position.x)
            {
                damageDirection = -1;
            }
            else
            {
                damageDirection = 1;
            }

            //hit particle can be added here

            //checks what state needs to be transitioned to
            if (health > 0.0f)
            {
                knockback();
            }
            else
            {
                death();
            }
        }
    }

    //function that handles player losing all their health
    private void death()
    {
        gameManager.endGame("You Lose!");
        //Destroy(movement); //destroys the parent game object that handles movement
    }

    //function that handles getting knockback from a hit
    private void knockback()
    {

        invincible = true;
        invincibilityTimer = invincibilityDuration;

        animator.SetBool("Knockback", true);

        rb.velocity = new Vector2(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        disableMovement();
    }

    //Function that ends the knockback state for the player;
    private void endKnockback()
    {
        animator.SetBool("Knockback", false);
        enableMovement();
    }

    //disables the player controls
    private void disableMovement()
    {
        playerMovementInput.enabled = false;
    }

    //enables player controls
    private void enableMovement()
    {
        playerMovementInput.enabled = true;
    }

    //disables player collisions with enemies
    private void DisableEnemeyCollisions()
    {
        Physics2D.IgnoreLayerCollision(9, 8, true);
        invincible = true;
    }

    //enables player collisions with enemies
    private void EnableEnemeyCollisions()
    {
        Physics2D.IgnoreLayerCollision(9, 8, false);
        invincible = false;
    }

    //Visualizeses attack radius
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitboxPos.position, attack1Radius);
    }

    //method that returns the player health
    public float getHealth()
    {
        return health;
    }

    //returns if the player is attacking
    public bool getAttack()
    {
        return isAttacking;
    }

    private void CancalWait()
    {
        waitForAnim = false;
    }

}
