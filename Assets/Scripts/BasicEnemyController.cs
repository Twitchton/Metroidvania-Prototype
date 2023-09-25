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
    private bool floorDetected, wallDetected;
    private float facingDirection, currentHealth;

    private Vector2 movement;

    //object references
    [SerializeField] private GameObject alive;
    [SerializeField] private Rigidbody2D aliveRB;
    [SerializeField] private Transform floorCheck;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask floorLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float floorCheckDistance;
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float maxHealth;


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

    }

    private void UpdateIdleState()
    {

    }

    private void ExitIdleState()
    {

    }

    //Running state
    private void EnterRunningState()
    {

    }
    
    private void UpdateRunningState()
    {
        floorDetected = Physics2D.Raycast(floorCheck.position, Vector2.down, floorCheckDistance, floorLayer);
        wallDetected = Physics2D.Raycast(wallCheck.right, Vector2.down, wallCheckDistance, wallLayer);

        if (!floorDetected || wallDetected)
        {
            //stop
            //flip?
        }
        else
        {
            movement.Set(movementSpeed*facingDirection, aliveRB.velocity.y);
            aliveRB.velocity = movement;
        }
    }

    private void ExitRunningState()
    {

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

    }

    private void UpdateKnockbackState()
    {

    }

    private void ExitKnockbackState()
    {

    }

    //Dead state
    private void EnterDeadState() 
    { 
    
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
        facingDirection *= -1f;
        Vector3 localScale = alive.transform.localScale;
        localScale.x *= -1f;
        alive.transform.localScale = localScale;

    }

}
