using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AI : MonoBehaviour
{
    public TapToStart tapToStart;
    public float enemyRetardSpeed;
    public GameObject canvas;
    public GameObject gameOverUI;
    public bool enemyBreak;
    public float enemyJetpackFuelDecrementValue;
    public float enemyJetpackFuel;
    public bool isDead;
    public float ColliderCountdownPowerUp;
    public Rigidbody enemy_Rigidbody;
    public float enemy_Speed = 3;
    public float enemy_Jump_Power = 25;
    public float enemy_Jump_Power1 = 35;
    public float enemy_Rb_Velocity;
    public bool isJumped;
    public float enemyExtraForce_Down = 100f;
    public float enemyExtraForce_Up = 100f;
    public bool isJumpLimitReached;
    public GameObject flyingParticle;
    public Vector3 offset;
    public Vector3 offset1;
    public LayerMask breakable_LM;
    public bool isJum;
    public bool isJump1;
    public float PowerUp_Countdown_Time;
    public bool isBreakableEnemyTouch;
    public bool boom;
    public bool isEnemyWon;
    public bool isEnemyLost;
    public Player_Movement player_Movement;
    public Camera enemycamera;
    public Animator enemyAnimator;
    public Collider enemyCollider;
    public Vector3 obstacleOffset;
    public Vector3 hammerObstacleRayOffset;
    public Vector3 sideScrollingObstacleOffset;
    public float enemy_SpeedBoost;
    public Vector3 enemyBoostRaycastOffset;

    void Start()
    {
        tapToStart = GameObject.FindGameObjectWithTag("C1").GetComponent<TapToStart>();
        enemyBreak = false;
        isDead = false;
        enemyAnimator = GetComponent<Animator>();
        isEnemyWon = false;
        isBreakableEnemyTouch = false;
        enemy_Rigidbody = GetComponent<Rigidbody>();
        isJumpLimitReached = false;
        player_Movement = GameObject.Find("PlayerNew").GetComponent<Player_Movement>();
    }



    void Update()
    {
        if(tapToStart.playerMovePermission)
        {   
            print(enemyJetpackFuelDecrementValue);
        enemyJetpackFuel = Mathf.Clamp(enemyJetpackFuel, -0.009f, 1);

        enemy_Rb_Velocity = enemy_Rigidbody.velocity.y;


        if (!isEnemyWon && !enemyBreak)
        {
            transform.position += Vector3.forward * enemy_Speed * Time.deltaTime;
        }


        RaycastHit bottomHit;
        Debug.DrawRay(transform.position , -transform.up * .30f, Color.yellow);
        
        if(Physics.Raycast(transform.position , -transform.up , out bottomHit , .05f))
        {
            if(bottomHit.transform.gameObject.CompareTag("Normal_Tile"))
            {
                print("BoomHit");
            }
        }

        RaycastHit hit1;
        Debug.DrawRay(transform.position + offset1, transform.up * 1.5f, Color.blue);

        if (Physics.Raycast(transform.position + offset1, transform.up, out hit1, 1.5f))     //Up
        {

            if (!isJumpLimitReached && hit1.transform.gameObject.CompareTag("Breakable") && !isJump1)
            {
                isJump1 = true;

                if (isJump1)
                {
                    GetComponent<Animator>().SetBool("IsJump", false);
                    enemyAnimator.SetBool("IsJump", true);
                    enemy_Rigidbody.velocity = Vector3.up * enemy_Jump_Power1 * Time.deltaTime;
                    isJumped = true;
                    isJump1 = false;

                }
                isJump1 = false;
            }
            
        }



        if (transform.position.y >= 7)
        {
            isJumpLimitReached = true;
        }
        else
        {
            isJumpLimitReached = false;
        }

        RaycastHit hit3;
        Debug.DrawRay(transform.position, -transform.up * 0.05f, Color.blue);
        if (Physics.Raycast(transform.position, -transform.up, out hit3, 0.05f))
        {
            flyingParticle.SetActive(false);
            GetComponent<Animator>().SetBool("IsJump", false);

        }
        else
        {
            GetComponent<Animator>().SetBool("IsJump", true);
            flyingParticle.SetActive(true);
        }

        RaycastHit hit4;                                                                                    // obstacle evade
        Debug.DrawRay(transform.position + obstacleOffset, transform.forward * .5f, Color.blue);
        if (Physics.Raycast(transform.position + obstacleOffset, transform.forward, out hit4, .5f))
        {
            if (enemyJetpackFuel >= 0)
            {
                if (hit4.transform.gameObject.tag == "Obstacle")
                {
                    enemy_Rigidbody.drag = 15;
                    isJum = true;
                    enemy_Rigidbody.velocity = Vector3.up * enemy_Jump_Power * Random.Range(30f, 35f) * Time.deltaTime;
                    enemyJetpackFuel -= enemyJetpackFuelDecrementValue * Time.deltaTime;

                }

            }

            else
            {
                isJum = false;
                enemy_Rigidbody.drag = 10;
            }
        }


        RaycastHit hit5;
        Debug.DrawRay(transform.position, transform.forward * 2f, Color.red);
        if (Physics.Raycast(transform.position, transform.forward * 2, out hit5, 1f))
        {
            if (hit5.transform.gameObject.tag == "HammerObstacle")
            {
                isEnemyWon = true;
                enemyAnimator.SetBool("Idle", true);
            }

        }
        else
        {
            isEnemyWon = false;
            enemyAnimator.SetBool("Idle", false);
        }

        RaycastHit hit6;
        Debug.DrawRay(transform.position + sideScrollingObstacleOffset, transform.forward * 2f, Color.red);
        if (Physics.Raycast(transform.position + sideScrollingObstacleOffset, transform.forward * 2, out hit6, 1f))
        {
            if (hit6.transform.gameObject.tag == "SideScrollingObstacle")
            {
                isEnemyWon = true;
                enemyAnimator.SetBool("Idle", true);
            }
        }
        else
        {
            isEnemyWon = false;
            enemyAnimator.SetBool("Idle", false);
        }

        RaycastHit hit7;
        Debug.DrawRay(transform.position , -transform.up * .6f , Color.black );
        if(Physics.Raycast(transform.position , -transform.up , out hit7 , .6f))
        {
            if(hit7.transform.gameObject.tag == "Water")
            {
                print("Water");
                enemy_Rigidbody.drag = 15;
                isJum = true;
                enemy_Rigidbody.velocity = Vector3.up * enemy_Jump_Power * Random.Range(15f, 15f) * Time.deltaTime;
                enemyJetpackFuel -= enemyJetpackFuelDecrementValue * Time.deltaTime;

            }
        }



        
        RaycastHit hitTileUp ;
         Debug.DrawRay(transform.position + enemyBoostRaycastOffset , transform.up * 1f, Color.blue);
        if(Physics.Raycast(transform.position + enemyBoostRaycastOffset , transform.up * 1f, out hitTileUp , 1 ))
        {
            if(hitTileUp.transform.CompareTag("Normal_Tile"))
            {
                enemy_Speed +=  enemy_SpeedBoost;
            }
          
        }
         else
            {
                enemy_Speed = enemyRetardSpeed;
                StartCoroutine("EnemySlowDown");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Breakable") && isJumped && enemy_Rb_Velocity < 0)
        {
            isBreakableEnemyTouch = true;
            boom = true;
            if (boom)
            {
                StartCoroutine("Blaay");
            }
            collision.gameObject.GetComponent<Collider>().enabled = false;
            enemy_Rigidbody.velocity = Vector3.down * enemyExtraForce_Down * Time.deltaTime;
            print("Breakable");
        }
        else if (collision.gameObject.CompareTag("Breakable") && isJumped && enemy_Rb_Velocity > 0)
        {
            isBreakableEnemyTouch = true;
            enemy_Rigidbody.velocity = Vector3.up * enemyExtraForce_Up * Time.deltaTime;
            collision.gameObject.GetComponent<Collider>().enabled = false;
            boom = true;
            if (boom)
            {
                StartCoroutine("Blaay");
            }
            print("Breakable");
        }

        if (collision.gameObject.CompareTag("Normal_Tile"))
        {
            isJumped = false;
            GetComponent<Animator>().SetBool("IsJump", false);
        }



        if (collision.gameObject.CompareTag("PowerUp"))
        {
            enemy_Speed = 6;
            StartCoroutine("PowerUp_CountDown");
            StartCoroutine("PowerUpCollider", collision.gameObject);
        }


        if (collision.gameObject.CompareTag("Finish"))
        {

           canvas.SetActive(false);
            gameOverUI.SetActive(true);
            enemyBreak = true;
            player_Movement.isPlayerWon = true;
            collision.gameObject.SetActive(false);
            GetComponent<Animator>().SetTrigger("Won");
            player_Movement.GetComponent<Animator>().SetTrigger("IsLost");

        }

        if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("HammerObstacle") || collision.gameObject.CompareTag("SideScrollingObstacle") )
        {
            isEnemyWon = true;
            enemyBreak = true;
            isDead = true;
            enemyAnimator.SetTrigger("IsLost");
           enemy_Rigidbody.velocity = (new Vector3(enemy_Rigidbody.velocity.x , enemy_Rigidbody.velocity.y , -10));
           
            StartCoroutine("EnemyDeathCoroutine");
        }

        if (collision.gameObject.CompareTag("Fuel"))
        {
            enemyJetpackFuel = 1;
            collision.gameObject.GetComponent<Animator>().SetTrigger("Fuel");
            StartCoroutine("PowerUpCollider", collision.gameObject);
        }

    }

    IEnumerator EnemySlowDown()
    {
        yield return new WaitForSeconds(16f);
        
        if(enemyRetardSpeed > 1.6)
        {
            enemyRetardSpeed -= .04f * Time.deltaTime;
        }
    }

    IEnumerator EnemyDeathCoroutine()
    {
       
        yield return new WaitForSeconds(2f);
        isEnemyWon = false;
        enemyBreak = false;
        gameObject.SetActive(false);
        
    }

    IEnumerator PowerUp_CountDown()
    {
        yield return new WaitForSeconds(PowerUp_Countdown_Time);
        enemy_Speed = 0.9f;
    }

     IEnumerator PowerUpCollider( GameObject PowerGo)
    {
        
        yield return new WaitForSeconds(ColliderCountdownPowerUp);
        PowerGo.SetActive(false);
    }
    IEnumerator Blaay()
    {
         enemy_Rigidbody.GetComponent<Collider>().isTrigger = true;
         yield return new WaitForSeconds(.09f);
         enemy_Rigidbody.GetComponent<Collider>().isTrigger = false;
         boom = false;
    }
}
