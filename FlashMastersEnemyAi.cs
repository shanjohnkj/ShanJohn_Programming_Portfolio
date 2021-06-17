using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public bool destroyBool = false;
    public Enemy_Destroy enemy_Destroy_Script;
    public Collider Main_Collider;
    public Collider[] All_Colliders;

    Vector3 direction;
    public GameObject bullet_Holder;
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    public float speed;

                                                            //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

                                                            //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

                                                            //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    public EnemyAi enemy_AI;
    public Animator enemy_Animator;

    private void Awake()
    {
        enemy_Animator = GetComponent<Animator>();
        enemy_AI = GetComponent<EnemyAi>();
        All_Colliders = GetComponentsInChildren<Collider>(true);
        enemy_Destroy_Script = GetComponent<Enemy_Destroy>();
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
       
    }

    void Start()
    {
        foreach(var col in All_Colliders)
        {
            col.enabled = false;
        }
        Main_Collider.enabled = true;
    }

    private void Update()
    {
                                                                    //Check for sight and attack range

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }
    }

   
    private void AttackPlayer()                                     // enemy doesn't move
    {
        
        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            direction = (player.transform.localPosition-transform.localPosition ).normalized  ;
            enemy_Animator.SetBool("Attack",true);
           Transform projectileBall = Instantiate(projectile.transform,bullet_Holder.transform.position,Quaternion.identity);
            projectileBall.rotation = this.transform.rotation;
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);        //End of attack code
        }
        else 
        {
                enemy_Animator.SetBool("Attack",false);
        } 

    }
    private void ResetAttack()                                      //Reset Attack
    {
        alreadyAttacked = false;
        
    }

   
 

    private void OnDrawGizmosSelected()                             //Gizmos
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }

    public void DoRagdoll()                                         //Ragdoll
    {
        
        foreach (Collider col in All_Colliders)
        {
            col.enabled = true;
        }

        Main_Collider.enabled = false;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Animator>().enabled = false;

    }

}
