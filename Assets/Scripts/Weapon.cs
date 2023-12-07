using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering;

public class Weapon : MonoBehaviour
{
    //object reference
    [SerializeField] private GameObject character;
    [SerializeField] private PlayerCombat source;
    [SerializeField] private LayerMask Damageable;
    [SerializeField] private Transform attackHitboxPos;
    [SerializeField] private Transform upAttackHitboxPos;
    [SerializeField] private Transform downAttackHitboxPos;
    [SerializeField] private Animator attackAnim;

    //attack sounds
    [SerializeField] private AudioSource playerSound;
    [SerializeField] private AudioClip attackHit;
    [SerializeField] private AudioClip attackMiss;

    //variables
    private float[] attackDetails = new float[2];
    [SerializeField] private float attack1Radius;
    [SerializeField] private float attack1Damage;

    private bool up, down;
    

    // Start is called before the first frame update
    void Start()
    {
        up = false;
        down = false;
    }

    // Update is called once per frame
    void Update()
    {
        //getting bools for up/down
        up = source.getUp();
        down = source.getDown();
    }

    //function that starts an attack
    public void Attack()
    {
        if (up)
        {
            gameObject.transform.position = upAttackHitboxPos.position;
            
        }
        else if (down)
        {
            gameObject.transform.position = downAttackHitboxPos.position;
            
        }
        else
        {
            gameObject.transform.position = attackHitboxPos.position;
            
        }

        animateAttack();
    }

    //check hitbox for landing attack
    private void CheckAttackHitbox()
    {
        Transform attackHitbox;

        if (up)
        {
            attackHitbox = upAttackHitboxPos;
        }
        else if(down)
        {
            attackHitbox = downAttackHitboxPos;
        }
        else
        {
            attackHitbox = attackHitboxPos;
        }

        //creates a circle collider to check for attack
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackHitbox.position, attack1Radius, Damageable);

        if (detectedObjects.Length > 0)
        {
            playerSound.clip = attackHit;
        }
        else
        {
            playerSound.clip = attackMiss;
        }

        playerSound.Play();
        
        attackDetails[0] = attack1Damage;
        attackDetails[1] = character.transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.gameObject.SendMessage("Damage", attackDetails);
        }

    }

    //sets parameters for attack animations
    private void animateAttack()
    {
        attackAnim.SetTrigger("attack");
        attackAnim.SetBool("up", up);
        attackAnim.SetBool("down", down);
    }

    //Visualizeses attack radius
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitboxPos.position, attack1Radius);
        Gizmos.DrawWireSphere(upAttackHitboxPos.position, attack1Radius);
        Gizmos.DrawWireSphere(downAttackHitboxPos.position, attack1Radius);
    }

    //method that updates the player's is attacking variable
    private void startAttack()
    {
        source.StartAttacking();
    }

    //method that updates the player's is attacking variable
    private void endAttack()
    {
        source.EndAttacking();
    }
}
