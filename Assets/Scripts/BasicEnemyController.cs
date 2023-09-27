using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class BasicEnemyController : MonoBehaviour
{
    private enum State
    {
        idle,
        running,
        attacking,
        knockback,
        dead
    }

    //variables
    private State currentState;
    private bool floorDetected, wallDetected, playerDetected;
    private float currentHealth, knockbackStartTime, behaviourTimer;
    private int facingDirection, damageDirection;

    private Vector2 movement;

    //object references
    [SerializeField] private GameObject alive;
    [SerializeField] private Rigidbody2D aliveRB;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 knockBackSpeed;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float floorCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float behaviourTimerValue;

    //initial state of the enemy
    private void Start()
    {
        currentState = State.idle;
        behaviourTimer = behaviourTimerValue * 2f;
        playerDetected = false;
        facingDirection = 1;
    }

    //function called every frame
    private void Update()
    {
        //switching states
        switch (currentState)
        {
            case State.idle:
                UpdateIdleState();
                break;

            case State.running:
                UpdateRunningState();
                break;

            case State.attacking:
                UpdateAttackingState();
                break;

            case State.knockback:
                UpdateKnockbackState();
                break;

            case State.dead:
                UpdateDeadState(); 
                break;

        }
    }

    //Idle state
    private void EnterIdleState()
    {
        animator.SetBool("Idle", true);
    }

    private void UpdateIdleState()
    {
        if(behaviourTimer <= 0f)
        {
            SwitchState(State.running);
        }
        behaviourTimer -= Time.deltaTime;
    }

    private void ExitIdleState()
    {
        behaviourTimer = behaviourTimerValue;
        animator.SetBool("Idle", false);
    }

    //Running state
    private void EnterRunningState()
    {
        animator.SetBool("Running", true);
    }
    
    private void UpdateRunningState()
    {
        floorDetected = Physics2D.Raycast(floorCheck.position, Vector2.down, floorCheckDistance, floorLayer);
        wallDetected = Physics2D.Raycast(wallCheck.right, Vector2.right, wallCheckDistance, wallLayer);

        if (!floorDetected || wallDetected)
        {
            if (playerDetected)
            {
                movement.Set(0f, 0f);
                aliveRB.velocity = movement;
            }

            if (!playerDetected)
            {
                Flip();
            }

        }
        else
        {
            movement.Set(movementSpeed*facingDirection, aliveRB.velocity.y);
            aliveRB.velocity = movement;
        }

        if (!playerDetected)
        {
            behaviourTimer -= Time.deltaTime;
        }

        if (behaviourTimer <= 0)
        {
            SwitchState(State.idle);
        }
    }

    private void ExitRunningState()
    {
        behaviourTimer = behaviourTimerValue*2;
        movement.Set(0f, 0f);
        aliveRB.velocity = movement;
        animator.SetBool("Running", false);
    }

    //Attacking state
    private void EnterAttackingState()
    {

    }

    private void UpdateAttackingState()
    {

    }

    private void ExitAttackingState()
    {

    }

    //Knockback state
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockBackSpeed.x * damageDirection, knockBackSpeed.y);
        aliveRB.velocity = movement;
        animator.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.running);
        }
    }

    private void ExitKnockbackState()
    {
        animator.SetBool("Knockback", false);
    }

    //Dead state
    private void EnterDeadState() 
    {
        //play death animation
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }

    //Other functions

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.idle:
                ExitIdleState();
                break;

            case State.running:
                ExitRunningState();
                break;

            case State.attacking:
                ExitAttackingState();
                break;

            case State.knockback:
                ExitKnockbackState();
                break;

            case State.dead:
                ExitDeadState();
                break;
             
        }

        switch (state)
        {
            case State.idle:
                EnterIdleState();
                break;

            case State.running:
                EnterRunningState();
                break;

            case State.attacking:
                EnterAttackingState();
                break;

            case State.knockback:
                EnterKnockbackState();
                break;

            case State.dead:
                EnterDeadState();
                break;

        }

        currentState = state;
    }

    //function to flip enemy sprite
    private void Flip()
    {
        facingDirection *= -1;
        Vector3 localScale = alive.transform.localScale;
        localScale.x *= -1f;
        alive.transform.localScale = localScale;
    }

    //function to take damage
    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        //hit particle

        if (currentHealth > 0.0f)
        {
            SwitchState(State.knockback);
        }
        else
        {
            SwitchState(State.dead);
        }
    }

    //Function to visualize raycasts for checks
    private void OnDrawGizmos()
    {
        //floor check
        Gizmos.DrawLine(floorCheck.position, new Vector2(floorCheck.position.x, floorCheck.position.y - floorCheckDistance));

        //wall check
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
    }
}
