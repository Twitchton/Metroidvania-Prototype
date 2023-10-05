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
    [SerializeField] private bool floorDetected, wallDetected, playerDetected, canFlip;
    private float currentHealth, knockbackStartTime, behaviourTimer, detectionTimer;
    private float[] attackDetails = new float[2];
    private int facingDirection, damageDirection, playerDirection;

    private Vector2 movement;

    //object references
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject alive;
    [SerializeField] private GameObject player;
    [SerializeField] private Rigidbody2D aliveRB;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Transform playerCheckPos;
    [SerializeField] private Transform detectionPos;
    [SerializeField] private Transform attackHitboxPos;
    [SerializeField] private Vector2 knockBackSpeed;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask Player;
    [SerializeField] private float floorCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxHealth;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private float behaviourTimerValue;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float detectionTimerValue;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackDamage;
    

    //initial state of the enemy
    private void Start()
    {
        currentHealth = maxHealth;
        currentState = State.idle;
        behaviourTimer = behaviourTimerValue * 2f;
        playerDetected = false;
        canFlip = true;
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

        if (playerDetected && (Vector2.Distance(attackHitboxPos.position, player.transform.position)) <= attackRadius)
        {
            SwitchState(State.attacking); //attack if player is in attack range   
        }
    }

    private void LateUpdate()
    {
        //ensure enemy is facing player
        if (playerDetected)
        {
            if (player.transform.position.x < alive.transform.position.x && facingDirection > 0)
            {
                Flip();
            }
            if(player.transform.position.x > alive.transform.position.x && facingDirection < 0)
            {
                Flip();
            }
        }
    }

    //Idle state
    private void EnterIdleState()
    {
        animator.SetBool("Idle", true);
    }

    private void UpdateIdleState()
    {
        //floor and wall checking
        floorDetected = Physics2D.Raycast(floorCheck.position, Vector2.down, floorCheckDistance, floorLayer);
        wallDetected = Physics2D.Raycast(wallCheck.right, Vector2.right, wallCheckDistance, wallLayer);

        //logic for idlestate when player is detected
        if (playerDetected && floorDetected && !wallDetected)
        {
            SwitchState(State.running);
        }

        //timer if player isn't detected
        if (!playerDetected)
        {
            behaviourTimer -= Time.deltaTime;
        }

        //Idle Timer logic
        if(behaviourTimer <= 0f)
        {
            SwitchState(State.running);
        }
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
        animator.SetBool("Attacking", true);
    }

    private void UpdateAttackingState()
    {

    }

    private void ExitAttackingState()
    {
        animator.SetBool("Attacking", false);
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
        gameManager.enemyKilled();
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
        if (canFlip)
        {
            facingDirection *= -1;
            Vector3 localScale = alive.transform.localScale;
            localScale.x *= -1f;
            alive.transform.localScale = localScale;
        }
        
    }

    //function to take damage
    public void Damage(float[] damageDetails)
    {

        //getting array of values for the attack
        currentHealth -= damageDetails[0];

        if (damageDetails[1] > alive.transform.position.x)
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
        float playerDist = Vector2.Distance(detectionPos.position, player.transform.position);

        //Raycast to check if player is obstructed
        RaycastHit2D hit = Physics2D.Raycast(detectionPos.position, player.transform.position - detectionPos.position);

        //is player in radius and not obstructed
        if (playerDist<=detectionRadius && hit.collider.gameObject.tag == "Player")
        {
            playerDetected = true;
            detectionTimer = detectionTimerValue;
        }
        //timer for enemy remembering players
        else if(detectionTimer > 0f){
            detectionTimer -= Time.deltaTime;
        }
        //enemy loses player detection
        else
        {
            playerDetected = false;
        }
        
    }

    //function to check hitboxes for landing attacks
    private void CheckAttackHitbox()
    {
        //creates a circle collider to check for player hitboxes
        Collider2D[] detectedPlayer = Physics2D.OverlapCircleAll(attackHitboxPos.position, attackRadius, Player);

        attackDetails[0] = attackDamage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedPlayer)
        {
            collider.transform.gameObject.SendMessage("Damage", attackDetails);
        }
    }

    //function to disable flip while attacking
    public void disableFlip()
    {
        canFlip = false;
    } 

    //function to re-enable flip after attack
    public void enableFlip()
    {
        canFlip = true;
    }

    //Function to visualize raycasts for checks
    private void OnDrawGizmos()
    {
        //floor check
        Gizmos.DrawLine(floorCheck.position, new Vector2(floorCheck.position.x, floorCheck.position.y - floorCheckDistance));

        //wall check
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        
        //detection radius
        Gizmos.DrawWireSphere(detectionPos.position, detectionRadius);

        //attack radius
        Gizmos.DrawWireSphere(attackHitboxPos.position, attackRadius);
    }
}
