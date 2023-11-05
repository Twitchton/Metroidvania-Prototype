using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Weapon : MonoBehaviour
{
    //object reference
    [SerializeField] GameObject character;
    [SerializeField] private LayerMask Damageable;
    [SerializeField] private Transform attackHitboxPos;
    [SerializeField] private Transform upAttackHitboxPos;
    [SerializeField] private Transform downAttackHitboxPos;
    [SerializeField] private Animator attackAnim;

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
        
    }

    //function that starts an attack
    public void Attack(bool up, bool down)
    {
        if (up)
        {
            this.up = up;
            gameObject.transform.position = upAttackHitboxPos.position;
            gameObject.transform.eulerAngles = new Vector3(0, 0, 90);
        }
        else if (down)
        {
            this.down = down;
            gameObject.transform.position = downAttackHitboxPos.position;
            gameObject.transform.eulerAngles = new Vector3(0, 0, -90);
        }
        else
        {
            gameObject.transform.position = attackHitboxPos.position;
            gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
        }

        attackAnim.SetBool("attack", true);
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

        attackDetails[0] = attack1Damage;
        attackDetails[1] = character.transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.gameObject.SendMessage("Damage", attackDetails);
        }

        up = false;
        down = false;
    }

    private void EndAttack()
    {
        attackAnim.SetBool("attack", false);
    }

    //Visualizeses attack radius
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackHitboxPos.position, attack1Radius);
        Gizmos.DrawWireSphere(upAttackHitboxPos.position, attack1Radius);
        Gizmos.DrawWireSphere(downAttackHitboxPos.position, attack1Radius);
    }
}
