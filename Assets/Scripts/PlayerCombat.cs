using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    //Object references
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Weapon weapon;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerMovementInput;
    public Animator animator;

    //Open Variables
    [SerializeField] private float inputTimer; //timing to fudge attack time
    [SerializeField] private float lastInputTime;
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float directionTime;
    [SerializeField] private Vector2 knockbackSpeed;
    [SerializeField] private Vector2 dashPower;

    //private variables
    [SerializeField] private float health, mana, gravityScale, attackCooldown;
    [SerializeField] private float invincibilityTimer, dashTimer, directionTimer, attackTimer;
    [SerializeField] private bool isAttacking, attackCheck, isFirstAttack, gotInput, attack1, attack2, dashAttack, invincible, dashing, dashCheck, downInput, upInput, timerOn, attackOn;
    [SerializeField] private int damageDirection, attackCount, comboMax;

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

        downInput = false;
        upInput = false;

        timerOn = false;

        //max attack variables
        attackCount = 0;
        attackTimer = 0f;
        attackOn = true;
    }

    //function called each frame
    private void Update()
    {

        CheckAttacks();

        //timer counting down for invincibility
        if(invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        //invincibility toggle
        if (invincibilityTimer <= 0f && !dashing)
        {
            invincible = false;
        }

        //timer for dash cooldown
        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
        }

        //directional cooldown
        if (directionTimer <= 0 && timerOn)
        {
            downInput = false;
            upInput = false;
        }

        if (directionTimer > 0f)
        {
            directionTimer -= Time.deltaTime;
        }

        //maximum attacks handling
        if (attackCount >= comboMax && attackOn)
        {
            attackOn = false;
            attackTimer = attackCooldown;
        }

        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        if (attackTimer <=0f)
        {
            attackCount = 0;
            attackOn = true;
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
        if (context.performed && !gameManager.getPaused() && !movement.GetWallSliding())
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
            timerOn = false;
            downInput = false;
            upInput = true;
        }

        if (context.canceled && !gameManager.getPaused())
        {
            timerOn = true;
            directionTimer = directionTime;
        }
    }

    //gets input for a verticle "down" attack
    public void DownInput(InputAction.CallbackContext context)
    {
        if (context.performed && !gameManager.getPaused())
        {
            timerOn = false;
            upInput = false;
            downInput = true;
        }

        if (context.canceled && !gameManager.getPaused())
        {
            timerOn = true;
            directionTimer = directionTime;
        }
    }

    //function to give player a dodge/dash
    public void Dash(InputAction.CallbackContext context)
    {
        if (dashTimer <= 0f && !gameManager.getPaused() && context.performed)
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
            if (!isAttacking && attackOn)
            {
                gotInput = false;
                weapon.Attack();
                attackCount++;
                attackTimer = attackCooldown;
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            //wait for new input
            gotInput = false;
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

    //method to access up input
    public bool getUp()
    {
        return upInput;
    }

    //method to access down input
    public bool getDown()
    {
        return downInput;
    }

    public bool getDash()
    {
        return dashing;
    }

}
