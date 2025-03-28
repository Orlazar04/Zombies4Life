// Golden Version
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieSpace;
using UnityEngine.AI;

// This script is for enemy momevent and tracking
// Dependencies: Level State, Level Difficulty, Settings Manager, Enemy Behavior, Target Manager
// Main Contributors: Grace Calianese, Olivia Lazar 
public class EnemyAI : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float chaseRange;
    [SerializeField]
    private float attackRange;
    [SerializeField]
    private GameObject[] healthPickups;

    [SerializeField]
    private AudioClip zombieDiesSFX;
    [SerializeField]
    private GameObject zombieDiesFX;

    public ZombieState currentState;            // The current behavior state
    private ZombieTarget currentTarget;         // The current movement target
    private bool canSwitchState;
    private float timer;

    private Transform eyesTF;
    private ZombieTarget sightedTarget;
    private Coroutine sightedRoutine;

    private Transform playerTF;
    private Transform pickupTF;
    private Vector3 randomLocation;             // Random nearby location to wander towards
    private NavMeshAgent agent;
    
    private Animator anim;
    
    // Start is called before the first frame update
    private void Start()
    {
        // Initiliaze enemy values
        currentTarget = ZombieTarget.None;
        currentState = ZombieState.Wander;
        canSwitchState = true;

        // Initialize enemy vision
        eyesTF = transform.GetChild(0);
        sightedTarget = ZombieTarget.None;
        
        InitializeEnemyType();

        InitializeTargets();
        InitializeNavigation();

        RandomDestination();

        // Initialize effects
        anim = GetComponent<Animator>();
    }

    // Initialize enemy stats, affected by difficulty and enemy type
    private void InitializeEnemyType()
    {
        switch(EnemyBehavior.enemyType)
        {
            case 1:
                SetEnemyValues(4, 15);
                break;
            case 2:
                SetEnemyValues(3, 20);
                break;
            case 3:
                SetEnemyValues(2, 10);
                break;
        }       
    }

    // Set enemy values
    private void SetEnemyValues(float speed, float chase)
    {
        moveSpeed = speed + (LevelManager.levelDifficulty - 1);
        chaseRange = chase;
        attackRange = 1.5f;
    }

    // Initializes player and pickup targets
    private void InitializeTargets()
    {
        // Initialize player components
        var player = GameObject.FindGameObjectWithTag("Player");
        playerTF = player.transform;

        if(TargetManager.IsTargetActive())
        {
            // Initialize pickup components
            var pickup = GameObject.FindGameObjectWithTag("Target Pickup");
            pickupTF = pickup.transform;
        }
    }

    private void InitializeNavigation()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.angularSpeed = 360;
        agent.stoppingDistance = attackRange + 0.75f;
    }

    // Update is called once per frame
    private void Update()
    {
        // While the level is active
        if (LevelManager.IsLevelActive())
        {
            // While the enemy is alive
            if (IsAlive())
            {
                UpdateZombieState();

                if (canSwitchState)
                {
                    StartCoroutine(SwitchZombieState());
                }
            }
        }
    }

    // Whether the enemy is alive
    public bool IsAlive()
    {
        return (currentState != ZombieState.Dead);
    }

    // Updates the zombie's state and target
    private void UpdateZombieState()
    {
        bool foundTarget = false;

        float distancePlayer = Vector3.Distance(transform.position, playerTF.position);
        foundTarget = SetZombieState(ZombieTarget.Player, distancePlayer, playerTF);
        
        // Checks for pickup target if it exists and player is not the target
        if(TargetManager.IsTargetActive() && !foundTarget)
        {
            float distancePickup = Vector3.Distance(transform.position, pickupTF.position);
            foundTarget = SetZombieState(ZombieTarget.Pickup, distancePickup, pickupTF);
        }
        
        // No target is found
        if (!foundTarget)
        {
            currentState = ZombieState.Wander;
            currentTarget = ZombieTarget.None;
        }
    }

    // Sets the zombie state for the given target if possible
    private bool SetZombieState(ZombieTarget target, float distance, Transform targetTF)
    {
        if(IsTargetInSight(target, targetTF) || sightedTarget == target)
        {
            // Set target if within range and has been seen recently or is in sight now
            if(distance <= attackRange)
            {
                currentTarget = target;
                currentState = ZombieState.Attack;
                //SetSightRoutine(target);
                return true;
            }
            else if(distance <= chaseRange)
            {
                currentTarget = target;
                currentState = ZombieState.Chase;
                return true;
            }
        }
        return false;
    }

    // Whether the given target is in sight
    private bool IsTargetInSight(ZombieTarget target, Transform targetTF)
    {
        RaycastHit hit;
        Vector3 directionToTarget = targetTF.position - eyesTF.position;

        if(Vector3.Angle(directionToTarget, eyesTF.forward) <= 70)
        {
            if(Physics.Raycast(eyesTF.position, directionToTarget, out hit, chaseRange))
            {
                bool target1 = (target == ZombieTarget.Player && hit.collider.CompareTag("Player"));
                bool target2 = (target == ZombieTarget.Pickup && hit.collider.CompareTag("Target Pickup"));

                if(target1 || target2)
                {
                    if(sightedTarget != ZombieTarget.None) StopCoroutine(sightedRoutine);
                    sightedTarget = target;
                    sightedRoutine = StartCoroutine(LoseSightOfTarget());
                    return true;
                }
            }
        }
        return false;
    }

    // Lose track of target after a few seconds
    private IEnumerator LoseSightOfTarget()
    {
        yield return new WaitForSeconds(3);
        sightedTarget = ZombieTarget.None;
    }


    // Switches the zombie state without interfering
    private IEnumerator SwitchZombieState()
    {
        canSwitchState = false;

        switch (currentState)
        {
            case ZombieState.Wander:
                Wander();
                break;
            case ZombieState.Chase:
                Chase();
                break;
            case ZombieState.Attack:
                Attack();
                yield return new WaitForSeconds(2);
                break;
        }
        
        canSwitchState = true;
    }

    // Moves the enemy randomly
    private void Wander()
    {
        anim.SetInteger("animState", 1);
        agent.isStopped = false;
        agent.speed = 0.5f;

        float distanceLocation = Vector3.Distance(transform.position, randomLocation);
        timer += Time.deltaTime;

        // If target location has been reached or is too far or taking too long
        if (distanceLocation < agent.stoppingDistance + 1 || !IsPointReachable(randomLocation) || timer > 5)
        {
            RandomDestination();
        }

        agent.SetDestination(randomLocation);
    }

    // Establish a nearby random position for the enemy to move towards
    private void RandomDestination()
    {
        timer = 0;

        Vector3 pos = transform.position;
        float xPos = Random.Range(pos.x - 5f, pos.x + 5f);
        float yPos = 1;
        float zPos = Random.Range(pos.z - 5f, pos.z + 5f);

        randomLocation = new Vector3(xPos, yPos, zPos);
    }

    // Ensures that the given destination is reachable
    private bool IsPointReachable(Vector3 targetPosition)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, agent.stoppingDistance, NavMesh.AllAreas))
        {
            NavMeshPath path = new NavMeshPath();
            agent.CalculatePath(hit.position, path);

            if (path.status == NavMeshPathStatus.PathComplete)
            {
                return true;
            }
        }
        return false;
    }

    // Makes the enemy chase the current target
    private void Chase()
    {
        anim.SetInteger("animState", 2);
        agent.isStopped = false;
        agent.speed = moveSpeed;
        //agent.acceleration = moveSpeed;

        agent.SetDestination(SelectTarget());
    }

    // Makes the enemy attack the player
    private void Attack()
    {
        anim.SetInteger("animState", 3);
        agent.isStopped = true;
        FaceTarget(SelectTarget()); 
    }

    // Returns the position of the current target
    private Vector3 SelectTarget()
    {
        switch (currentTarget)
        {
            case ZombieTarget.Player:
                return playerTF.position;
            case ZombieTarget.Pickup:
                return pickupTF.position;
            default:
                return randomLocation;
        }
    }

    // Faces the enemy towards the given target
    private void FaceTarget(Vector3 target)
    {
        Vector3 directionToTarget = (target - transform.position).normalized;
        directionToTarget.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10 * Time.deltaTime);
    }

    // Initiates procedure for zombie dying
    public void ZombieDies()
    {
        currentState = ZombieState.Dead;
        agent.SetDestination(transform.position);

        anim.SetInteger("animState", 4);

        // Disable colliders
        Collider[] colliders = gameObject.GetComponents<Collider>();
        colliders.ToList().ForEach(collider => collider.enabled = false);

        if(SettingsManager.isSFXOn) AudioSource.PlayClipAtPoint(zombieDiesSFX, transform.position);

        EnemySpawner.enemyCount--;
        FindObjectOfType<LevelManager>().UpdateScore(2 * EnemyBehavior.enemyType * LevelManager.levelDifficulty);

        Invoke("DestroyZombie", 3);
    }

    // Destroys the zombie
    private void DestroyZombie()
    {
        gameObject.SetActive(false);

        GameObject particleEffect = Instantiate(zombieDiesFX, transform.position, transform.rotation);
        particleEffect.transform.SetParent(transform.parent);

        DropHealthPickup();

        Destroy(gameObject);
    }

    // Possibly instantiates a health pickup
    private void DropHealthPickup()
    {
        int spawnChance = Random.Range(0, 100 + (LevelManager.levelDifficulty * 50));

        if(spawnChance < 50)
        {
            int spawnType = Random.Range(0, healthPickups.Length);

            Vector3 pickupPos = new Vector3(transform.position.x, 1.5f, transform.position.z);

            GameObject healthPickup = Instantiate(healthPickups[spawnType], pickupPos, transform.rotation);
            healthPickup.transform.parent = GameObject.FindGameObjectWithTag("Pickups").transform;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}