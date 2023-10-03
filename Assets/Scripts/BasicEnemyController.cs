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
    private float currentHealth, knockbackStartTime, behaviourTimer, detectionTimer;
    private int facingDirection, damageDirection, playerDirection;

    private Vector2 movement;

    //object references
    [SerializeField] private GameObject alive;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D aliveRB;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform playerCheckPos;
    [SerializeField] private Vector2 knockBackSpeed;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float floorCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float behaviourTimerValue;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float detectionTimerValue;

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

        detectPlayer(); // check for player detection
    }

    //Idle state
    private void EnterIdleState()
    {
        animator.SetBool("Idle", true);
    }

    private void UpdateIdleState()
    {
        //logic for idlestate when player is detected
        if (playerDetected)
        {

        }

        if(behaviourTimer <= 0f)
        {
            SwitchState(State.running);
        }
        behaviourTimer -= Time.deltaTime;
    }

    private void ExitIdleState()
    {
        //setting timer for other behaviours from random value
        behaviourTimer = Random.Range(behaviourTimerValue / 2f, behaviourTimerValue);
        animator.SetBool("Idle", false);
    }

    //Running state
    private void EnterRunningState()
    {
        animator.SetBool("Running", true);
    }
    
    private void UpdateRunningState()
    {
        //checks for blocks (gaps or walls)
        floorDetected = Physics2D.Raycast(floorCheck.position, Vector2.down, floorCheckDistance, floorLayer);
        wallDetected = Physics2D.Raycast(wallCheck.right, Vector2.right, wallCheckDistance, wallLayer);

        //check if path is impeded
        if (!floorDetected || wallDetected)
        {   
            //if player is detected wait by obstacle
            if (playerDetected)
            {
                movement.Set(0f, 0f);
                aliveRB.velocity = movement;
                SwitchState(State.idle);
            }

            //if player isn't detected flip and continue to move
            if (!playerDetected)
            {
                Flip();
            }

        }
        //if no obstables are in the way move
        else
        {
            movement.Set(movementSpeed*facingDirection, aliveRB.velocity.y);
            aliveRB.velocity = movement;
        }

        //cooldown on run if player isn't detected
        if (!playerDetected)
        {
            behaviourTimer -= Time.deltaTime;
        }

        //calling switch from current state
        if (behaviourTimer <= 0)
        {
            SwitchState(State.idle);
        }
    }

    private void ExitRunningState()
    {
        //setting longer timer for idle state
        behaviourTimer = Random.Range(behaviourTimerValue/ 2f, behaviourTimerValue)*2f;
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
        //exit switch statement for the state machine
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

        //enter switch statement for the state machine
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
        //flips sprite by changing it's local transform
        facingDirection *= -1;
        Vector3 localScale = alive.transform.localScale;
        localScale.x *= -1f;
        alive.transform.localScale = localScale;
    }

    //function to take damage
    private void Damage(float[] attackDetails)
    {
        //getting array of values for the attack
        currentHealth -= attackDetails[0];

        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        //hit particle can be added here

        //checks what state needs to be transitioned to
        if (currentHealth > 0.0f)
        {
            SwitchState(State.knockback);
        }
        else
        {
            SwitchState(State.dead);
        }
    }

    //function to determine if player has been detected by AI
    private void detectPlayer()
    {
        //find  distance between player and enemy
        float playerDist = Vector2.Distance(alive.transform.position, player.transform.position);

        //Raycast to check if player is obstructed
        RaycastHit2D hit = Physics2D.Linecast(alive.transform.position, player.transform.position - alive.transform.position);

        //is player in radius and not obstructed
        if (playerDist<=detectionRadius || hit.collider.gameObject == "Player")
        {
            playerDetected = true;
            detectionTimer = detectionTimerValue;
        }
        //timer for enemy remembering player
        else if(detectionTimer > 0f){
            detectionTimer -= detectionTimer.deltaTime
        }
        //enemy loses player detection
        else
        {
            playerDetected = false;
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
