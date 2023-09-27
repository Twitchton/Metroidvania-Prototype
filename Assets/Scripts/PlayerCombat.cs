using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    //Object references
    [SerializeField] private Transform attack1HitboxPos;
    [SerializeField] private LayerMask damagable;
    [SerializeField] private Rigidbody2D rb;
    public Animator animator;

    //Variables
    [SerializeField] private float inputTimer; //timing to fudge attack time
    [SerializeField] private float attack1Radius;
    [SerializeField] private int attack1Damage;
    public int health, maxHealth, mana, maxMana;
    private bool isAttacking, isFirstAttack, gotInput, combo, attack1, attack2;
    private float lastInputTime;

    private void Start()
    {
        lastInputTime = Mathf.NegativeInfinity;
    }

    private void Update()
    {

    }

    public void DisableFlip()
    {

    }

    public void EnableFlip()
    {

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
                isFirstAttack = !isFirstAttack;
                animator.SetBool("attack1", true);
                animator.SetBool("firstAttack", isFirstAttack);
                animator.SetBool("isAttacking", isAttacking);
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
        animator.SetBool("isAttacking", isAttacking);
        animator.SetBool("attack1", false);
    }

    //Visualizeses attack radius
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack1HitboxPos.position, attack1Radius);
    }

    private bool canCombo()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1");
    }

}
