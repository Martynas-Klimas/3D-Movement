using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{   
    [Header("General stats")]
    [SerializeField] protected float health = 100f;
    [SerializeField] protected float damage = 20f;
    [SerializeField] protected float attackRange = 3f;
    [SerializeField] protected float attackCooldown = 1f;

    [Header("Distances")]
    [SerializeField] protected float chaseDistance = 15f;
    [SerializeField] protected float stopChaseDistance = 30f;
    [SerializeField] protected float attackDistance = 5f;
    [SerializeField] protected float randomMoveDistance = 5f;

    [Header("Public components")]
    [SerializeField] protected Transform player;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsPlayer;

    protected NavMeshAgent agent; 
    protected State state;

    protected bool movePointSet = false;
    protected bool hasAttacked = false;
    protected float remainingAttackCooldown;

    protected Vector3 startingPosition;
    protected float playerDistance;
    protected float reachedPositionDistance = 1f; //for checking if we have reached the destination
    protected Vector3 movePointPosition;

    protected float randomX;
    protected float randomY;

    //the enemies will chase and attack based on distance but implement raycast vision in the future

    protected enum State
    {   
        GoingBackToStart,
        Patrolling,
        Chasing,
        Attacking
    }

    protected void Awake()
    {
        state = State.Patrolling;
        agent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
        remainingAttackCooldown = 0f;
    }

    protected void Update()
    {
        ResetAttackCoolDown();
        switch (state)
        {
            default:
            case State.Patrolling:
                EnemyPatrol();
                break;
            case State.GoingBackToStart:
                GoBackToStart();
                break;
            case State.Chasing:
                EnemyChase();
                break;
            case State.Attacking:
                EnemyAttack();
                break;
        }  
    }

    protected virtual void EnemyPatrol()
    {
        //get a random way point, if doesnt have one
        Debug.Log("Patrolling");
        if (!movePointSet){
            RandomMovePoint();
        }

        if (movePointSet)
        {   
            agent.SetDestination(movePointPosition);

            if (Vector3.Distance(transform.position, movePointPosition) < reachedPositionDistance)
            {
                movePointSet = false;
            }
        }
        

        if(Physics.CheckSphere(transform.position, chaseDistance, whatIsPlayer))
        {
            state = State.Chasing;
        }
    }

    protected virtual void EnemyChase()
    {
        // chase the player to engage
        Debug.Log("Chasing");
        transform.LookAt(player.position);
        agent.SetDestination(player.position);
        if (Physics.CheckSphere(transform.position, attackDistance, whatIsPlayer)) 
        {
            state = State.Attacking;
        }
        if (!Physics.CheckSphere(transform.position, stopChaseDistance, whatIsPlayer)) 
        {
            state = State.GoingBackToStart;
        }
    }

    protected virtual void EnemyAttack() //melee enemy hits player, ranged shoots projectiles
    {
        if (!hasAttacked && attackCooldown <= 0f)
        {
            if (Physics.CheckSphere(transform.position, attackDistance, whatIsPlayer))
            {
                Debug.Log("Attacking");
                //different attack actions for different enemies
                remainingAttackCooldown = attackCooldown;
                hasAttacked = true;
                state = State.Chasing;
            }
        }
    } 

    protected void GoBackToStart()
    {
        //go back to starting position
        agent.SetDestination(startingPosition);
        
        if (Vector3.Distance(transform.position, startingPosition) < reachedPositionDistance)
        {
            state = State.Patrolling; // if returned to start, patrol
        }
        //possibly redudant
        if (Vector3.Distance(transform.position, player.position) < chaseDistance)
        {
            state = State.Chasing; //if player enters chase distance again, chase.
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health < 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        //play death animation in the future
        Destroy(gameObject);
    }

    protected void RandomMovePoint()
    {
        float randomX = Random.Range(-randomMoveDistance, randomMoveDistance);
        float randomZ = Random.Range(-randomMoveDistance, randomMoveDistance);

        Vector3 movePointPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(movePointPosition, -transform.up, 2f, whatIsGround))
        {
            movePointSet = true;
        }
    }

    protected void ResetAttackCoolDown()
    {
        attackCooldown = Mathf.Min(0, attackCooldown - Time.deltaTime);
    }
}
