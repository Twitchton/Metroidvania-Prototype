using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    //Object references
    [SerializeField] private GameObject parent;
    [SerializeField] private Transform attack1HitboxPos;
    [SerializeField] private LayerMask damagable;
    [SerializeField] private Rigidbody2D rb;
    public Animator animator;

    //Variables
    [SerializeField] private float inputTimer; //timing to fudge attack time
    [SerializeField] private float attack1Radius;
    [SerializeField] private int attack1Damage;
    [SerializeField] public int health, maxHealth, mana, maxMana;
    [SerializeField]  private bool isAttacking, isFirstAttack, gotInput, combo, attack1, attack2;
    [SerializeField]  private float lastInputTime;

    private void Start()
    {
        lastInputTime = Mathf.NegativeInfinity;
    }

    private void Update()
    {
        CheckAttacks();
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
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack1HitboxPos.position, attack1Radius, damagable);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", attack1Damage);
        }
    }

    //resets booleans for Attack
    private void FinishAttack1()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", isAttacking);
        animator.SetBool("Attack1", false);
    }

    //Visualizeses attack radius
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitboxPos.position, attack1Radius);
    }

    //function that calls the disable flip function 
    public void disableFlip()
    {
        parent.GetComponent<PlayerMovement>().disableFlip();
    }

    //function that calls the disable flip function 
    public void enableFlip()
    {
        parent.GetComponent<PlayerMovement>().enableFlip();
    }

}
