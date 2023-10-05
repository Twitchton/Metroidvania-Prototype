using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    //Object references
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject movement;
    [SerializeField] private Transform attack1HitboxPos;
    [SerializeField] private LayerMask Damageable;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerInput playerInput;
    public Animator animator;

    //Open Variables
    [SerializeField] private float inputTimer; //timing to fudge attack time
    [SerializeField] private float attack1Radius;
    [SerializeField] private float attack1Damage;
    [SerializeField] private float lastInputTime;
    [SerializeField] private float maxHealth;
    [SerializeField] private float maxMana;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private Vector2 knockbackSpeed;

    //private variables
    private float[] attackDetails = new float[2];
    [SerializeField] private float health, mana;
    private float invincibilityTimer;
    private bool isAttacking, isFirstAttack, gotInput, attack1, invincible;
    private int damageDirection;

    private void Start()
    {
        lastInputTime = Mathf.NegativeInfinity;

        health = maxHealth;
        mana = maxMana;

        invincible = false;
        invincibilityTimer = 0f;
    }

    private void Update()
    {

        CheckAttacks();

        if(invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }

        if (invincibilityTimer <= 0f)
        {
            invincible = false;
        }
    }

    //function for catching inputs from player.
    public void AttackAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            gotInput = true;
            lastInputTime = Time.time;
            //attempt combat
        }

        if (context.canceled)
        {
            gotInput = false;
        }

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
                //isFirstAttack = !isFirstAttack;
                isFirstAttack = true;
                animator.SetBool("Attack1", true);
                animator.SetBool("FirstAttack", isFirstAttack);
                animator.SetBool("IsAttacking", isAttacking);
            }
        }

        if (Time.time >= lastInputTime + inputTimer)
        {
            //wait for new input
        }
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

    //resets booleans for Attack
    private void FinishAttack1()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("Attack1", false);
    }

    //function to take damage
    public void Damage(float[] damageDetails)
    {
        if (!invincible)
        {
            //getting array of values for the attack
            health -= damageDetails[0];

            if (damageDetails[1] > movement.transform.position.x)
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
        disableControls();
    }

    private void endKnockback()
    {
        animator.SetBool("Knockback", false);
        enableControls();
    }

    //disables the player controls
    private void disableControls()
    {
        playerInput.enabled = false;
    }

    //enables player controls
    private void enableControls()
    {
        playerInput.enabled = true;
    }

    //Visualizeses attack radius
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitboxPos.position, attack1Radius);
    }

    //function that calls the disable flip function 
    public void disableFlip()
    {
        movement.GetComponent<PlayerMovement>().disableFlip();
    }

    //function that calls the disable flip function 
    public void enableFlip()
    {
        movement.GetComponent<PlayerMovement>().enableFlip();
    }

    public float getHealth()
    {
        return health;
    }

}
