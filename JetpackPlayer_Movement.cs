using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player_Movement : MonoBehaviour
{
    public GameObject nearMissObject;
    public GameObject playerWaterParticle;
    public GameObject confettiParticle;
    public GameObject canvas;
    public GameObject levelCompleteUI;
    public GameObject flyingParticle;


    public float nearMissCountdown1;
    public float nearMissCountdown2;
    public float jetpackFuelDecrementValue;
    public float player_SpeedBoost;
    public float jetpackFuel;
    public float playerRetardSpeed; //must be equal to player speed
    public float ColliderCountdownPowerUp ;
    public float player_Speed = 3;
    public float player_Jump_Power = 25;
    public float player_Rb_Velocity;
    public float playerExtraForce_Down = 100f;
    public float playerExtraForce_Up = 100f;
    public float PowerUp_Countdown_Time;


    public bool isbu;
    public bool isfu;
    public bool isJumped;
    public bool isJumpLimitReached;
    public bool playerBreak;
    public bool isPlayerWon;
    public bool isBreakableTrigger;
    public bool blaab;
    public bool isPlayerLost;
    public bool playerBoost;
    public bool isDead;

    public Animator playerAnimator;
    public Animator nearMissAnimation;

    public Vector3 trailOffset;
    public Vector3 playerBoostRaycastOffset;

    public RaycastHit hit3;
    public Camera mainCamera;
    public Rigidbody player_Rigidbody;
    public Enemy_AI enemy_AI;
    public TrailRenderer playerTrail;
    public Image fuelFillCircleImage;
    public Vector3 particleOffset;
    public TapToStart tapToStart;

    void Start()
    {
        tapToStart = GameObject.FindGameObjectWithTag("C1").GetComponent<TapToStart>();
        isbu = false;
        isfu = false;
        playerBreak = false;
        isDead = false;
        enemy_AI = GameObject.Find("Enemy").GetComponent<Enemy_AI>();
        isPlayerWon = false;
        player_Rigidbody = GetComponent<Rigidbody>();
        isJumpLimitReached = false;
    }




    void Update()
    {
        if(tapToStart.playerMovePermission)
        {

            if(playerBreak)
            {
            nearMissObject.SetActive(false);
            }

            if(isbu)
            {
                playerAnimator.SetBool("StopFlying",true);
            }

            jetpackFuel = Mathf.Clamp(jetpackFuel,-0.009f, 1);
         
            fuelFillCircleImage.fillAmount = jetpackFuel;

            player_Rb_Velocity = player_Rigidbody.velocity.y;
        
        if(!isPlayerWon && !playerBreak)
        {
             transform.position +=  Vector3.forward * player_Speed * Time.deltaTime;
            
        }

        if(playerBoost && jetpackFuel >= 0)
        {
            
          if(isfu)
            {
                GetComponent<Animator>().SetBool("IsJump",true);
            }
            
                if (!isPlayerLost && !isPlayerWon)
                {

                    jetpackFuel -= jetpackFuelDecrementValue * Time.deltaTime;
                    player_Rigidbody.velocity += Vector3.up * player_Jump_Power * Time.deltaTime;
                    flyingParticle.SetActive(true);
                    isJumped = true;

                }

        }
       
        if(transform.position.y >= 7)
        {
            isJumpLimitReached = true;
        }
        else
        {
            isJumpLimitReached = false;
        }


        if(isPlayerLost)
        {
            gameObject.GetComponent<Animator>().SetTrigger("LooseGame");
            enemy_AI.gameObject.GetComponent<Animator>().SetBool("isWon",true);
        }

       
        Debug.DrawRay(transform.position, -transform.up * 0.05f, Color.blue);
        if (Physics.Raycast(transform.position, -transform.up, out hit3, 0.05f))
        {
            flyingParticle.SetActive(false);
            GetComponent<Animator>().SetBool("IsJump", false);
            player_Rigidbody.drag = 5 ;
        }


        else
        {
            GetComponent<Animator>().SetBool("IsJump", true);
            flyingParticle.SetActive(true);
        }


        RaycastHit hitTileUp ;
        Debug.DrawRay(transform.position + playerBoostRaycastOffset , transform.up * .3f, Color.blue);
        if(Physics.Raycast(transform.position + playerBoostRaycastOffset , transform.up * 1f, out hitTileUp , .3f ))
        {
            if(hitTileUp.transform.CompareTag("Normal_Tile") || hitTileUp.transform.CompareTag("HammerObstacle") ||  hitTileUp.transform.CompareTag("Obstacle")   || hitTileUp.transform.CompareTag("SideScrollingObstacle") )
            {
                player_Speed +=  player_SpeedBoost * Time.deltaTime;
                StartCoroutine("NearMiss");
            }
          
        }
        
         else
            {
                 player_Speed = playerRetardSpeed;
            }
    }
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Breakable") && isJumped && player_Rb_Velocity < 0) //DOWN
        {
            isBreakableTrigger = true;
            blaab = true;

            if(blaab)
            {
               StartCoroutine("Blaay");
            }

            collision.gameObject.GetComponent<Collider>().enabled = false;
            player_Rigidbody.velocity = Vector3.down * playerExtraForce_Down * Time.deltaTime;

        }
        else if(collision.gameObject.CompareTag("Breakable") && isJumped && player_Rb_Velocity > 0)  //UP
        {
             isBreakableTrigger = true;
                collision.gameObject.GetComponentInChildren<Collider>().enabled = false;

             player_Rigidbody.velocity = Vector3.up * playerExtraForce_Up * Time.deltaTime;
                blaab = true;
             if(blaab)
            {
               StartCoroutine("Blaay");
            }
        }

        if(collision.gameObject.CompareTag("Normal_Tile"))
        {
            isJumped = false;
            GetComponent<Animator>().SetBool("IsJump",false);
            
        }

        if(collision.gameObject.CompareTag("Fuel"))
        {
            jetpackFuel = 1f;
            collision.gameObject.GetComponent<Animator>().SetTrigger("Fuel");
            StartCoroutine("PowerUpCollider", collision.gameObject);


        }

        if(collision.gameObject.CompareTag("PowerUp"))
        {
            
            player_Speed = 6;
            StartCoroutine("PowerUp_CountDown");
            StartCoroutine("PowerUpCollider", collision.gameObject);

        }



        if(collision.gameObject.CompareTag("Finish"))
        {
            confettiParticle.SetActive(true);
            canvas.SetActive(false);
            levelCompleteUI.SetActive(true);
            isPlayerWon = true;
            enemy_AI.enemyBreak = true;
            collision.gameObject.SetActive(false);
            GetComponent<Animator>().SetTrigger("Won");
            enemy_AI.GetComponent<Animator>().SetTrigger("IsLost");
            
        }

         if (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("HammerObstacle") || collision.gameObject.CompareTag("SideScrollingObstacle") )
         {
             if(!playerBreak)
            {
                isPlayerWon = true;
                playerBreak = true;
                isDead = true;
                GetComponent<Animator>().SetTrigger("IsLost");
                StartCoroutine("PlayerDeathCoroutine");
            }
        

         }

        if(collision.gameObject.CompareTag("Water"))
        {
            isPlayerWon = true;
            playerBreak = true;
            isDead = true;
            GetComponent<Animator>().SetTrigger("IsLost");
            Instantiate(playerWaterParticle, transform.position + particleOffset, Quaternion.identity);
            StartCoroutine("PlayerWaterDeathCoroutine");
        }
        }
        
    IEnumerator PlayerDeathCoroutine()
    {
       
        yield return new WaitForSeconds(2f);
        isPlayerWon = false;
        playerBreak = false;
        gameObject.SetActive(false);
        
    }

       IEnumerator PlayerWaterDeathCoroutine()
    {
        
        yield return new WaitForSeconds(0.4f);
        isPlayerWon = false;
        playerBreak = false;

        gameObject.SetActive(false);
        
    }

     IEnumerator PowerUp_CountDown()  
    {
        playerTrail.enabled = true;
        playerTrail.SetPosition(0,trailOffset);
        yield return new WaitForSeconds(PowerUp_Countdown_Time);
        playerTrail.enabled = false;
        player_Speed = 1;
    }

    IEnumerator PowerUpCollider( GameObject PowerGo)
    {
         yield return new WaitForSeconds(ColliderCountdownPowerUp);
        PowerGo.SetActive(false);
    }

    IEnumerator Blaay()
    {
        
         player_Rigidbody.GetComponent<Collider>().isTrigger = true;
         yield return new WaitForSeconds(.09f);
         player_Rigidbody.GetComponent<Collider>().isTrigger = false;
         blaab = false;
    }

    IEnumerator NearMiss()
    {
     
        yield return new WaitForSeconds(nearMissCountdown1);

        nearMissAnimation.SetBool("NearMiss",true);

        yield return new WaitForSeconds(nearMissCountdown2);

        nearMissAnimation.SetBool("NearMiss", false);
    }

    public void StopMoving()
    {
        isPlayerWon =true;
    }
}
