using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;



public class Player : MonoBehaviour
{
    public ParticleSystem dust_System;
    public ParticleSystem dust_System_2;
    public ParticleSystem playerParticle;

    private Vector3 lastVelocity;
    public Vector3 offsetLine;
    public Vector3 startPos;

    public GameObject GameOver_Canvas;
    public GameObject WinScreen_Canvas;
    public GameObject confetti;

    public float Rb_stopLimit;
    public float speed;
    public float trailHeight;
    public float distance;
    public float lineRenderer_Length = 5f;
    public float power = 10f;
    public float dis;

    public bool canRun;
    public bool isDead;
    public bool isWon;
    public bool startGame = false;
    public bool letgo;
    public bool touch;

    public Image JoyStickBackgroud;
    public Image JoyStick;

    public Animator playerAnimator;
    
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 offset;
    private Vector2 direction;

    public Rigidbody Rb;

    public TrailRenderer trail;
    public LineRenderer lineRenderer;
    
    public Transform trailHolder;
    public EnemyAi enemyAi_Script;
    public Collider Main_Box_Collider;
    public PostProcessProfile PP;
    public LayerMask Enemy_Layer;
    public Time_Manager timeManager_Script;

    
    private void Start()
    {
        startGame = true;
        isDead = false;
       
        canRun = true;
       Main_Box_Collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        lastVelocity = Rb.velocity;
        Debug.Log(Rb.velocity.magnitude);
       if(startGame)
       {
                if(!isDead || !isWon)

        {

        lineRenderer.transform.position = this.transform.position;
      
        if (Rb.velocity != Vector3.zero && canRun)
        {
            playerAnimator.SetBool("Sprint",true);
            trail.gameObject.SetActive(true);
                    playerAnimator.speed = Rb.velocity.magnitude/5f;
        }
        else
        {
                    playerAnimator.speed = 1f;
                    playerAnimator.SetBool("Sprint", false);
                    trail.gameObject.SetActive(false);
        }


                if (Input.GetMouseButtonDown(0))
                {
                    timeManager_Script.canTimeRun = false;
                    timeManager_Script.DoSlowMotion();
                    lineRenderer.gameObject.SetActive(true);
                    startPoint = Input.mousePosition;
                }
                 
                if (Input.GetMouseButton(0))
                {
                    
                    endPoint = Input.mousePosition;
                    float evil_dis = Vector2.Distance(startPoint, endPoint);
                    dis = Mathf.Clamp(evil_dis, 0, 20f);
                    dust_System.gameObject.SetActive(true);
                    dust_System_2.gameObject.SetActive(true);
                    timeManager_Script.canTimeRun = false;
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, transform.position + offsetLine);
                    
                    lineRenderer.SetPosition(1, transform.position+ offsetLine + transform.forward * dis/2f); //sets line opposite as dragging
                    

                   
                    offset = endPoint - startPoint;
                                                                             //sets the size of the joystick move area
                                                                            //adjust the backgroud size accordingly in editor
                    direction = Vector2.ClampMagnitude(offset, 25.0f);
                    touch = true;
                    PP.GetSetting<Bloom>().intensity.value = 2f;

                }
               


                if (Input.GetMouseButtonUp(0))
                {
                    if (canRun)
                    {
                        Rb.velocity = Vector3.zero;
                        Rb.AddRelativeForce(Vector3.forward * (dis * 2f), ForceMode.Impulse);
                        Rb.drag = 1;
                        letgo = false;
                    }
                    PP.GetSetting<Bloom>().intensity.value = 0f;
                    dust_System.gameObject.SetActive(false);
                    dust_System_2.gameObject.SetActive(false);
                                                                         // dust_System.Stop();
                    startPos = transform.position;

                    timeManager_Script.RevertFromSlowMotion();
           
                                                                         //timeManager_Script.canTimeRun = true;
          
                    lineRenderer.gameObject.SetActive(false);
                    trail.Clear();
                    
                   
                                           

                    


                    touch = false;
                                                                         //direction = Vector2.zero;
                    offset = Vector2.zero;
                    

                   
                }

               distance = Vector3.Distance(startPos,transform.position);


            }
            if(isDead)
            {
                playerAnimator.speed = 1f;
            }
            else if(isWon){
                playerAnimator.speed = 1f;
            }


                if(Rb.velocity.magnitude < 2f)
            {
                Rb.velocity = Vector3.zero;
            }

        

       }

       if(!touch)
        this.transform.rotation = Quaternion.LookRotation(Rb.velocity, Vector3.up);


    }
      
    private void FixedUpdate()
    {
        
        if (!isDead || !isWon)

        {
          
            float heading = Mathf.Atan2(direction.x, direction.y);
           
                transform.localRotation = Quaternion.Euler(0, heading * Mathf.Rad2Deg, 0);

       
        }
}       

    IEnumerator MoveTimeLimit()
    {
        yield return new WaitForSecondsRealtime(2f);
        Rb.drag = 1;
        yield return new WaitForSecondsRealtime(.01f);
        Rb.drag = 0;

    }

    void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Enemy" )
        {

            print("PPP");
           
           Instantiate(playerParticle,transform.position + new Vector3(0,3,0),Quaternion.identity);
           canRun = false;
            enemyAi_Script = collision.gameObject.GetComponentInParent<EnemyAi>();

            foreach (var col in enemyAi_Script.All_Colliders)
            {
                col.enabled = true;
                
               
            }
          
            enemyAi_Script.Main_Collider.enabled = false;
          
            enemyAi_Script.enabled = false;
            enemyAi_Script.GetComponent<Animator>().enabled = false;
            enemyAi_Script.GetComponent<Rigidbody>().useGravity = true;
            StartCoroutine("Run");
            Destroy(enemyAi_Script.gameObject,4f);
         
            
        }
        

        if(collision.gameObject.tag == "Wall")
        {
            var speed = lastVelocity.magnitude;
            var direction = Vector3.Reflect(lastVelocity.normalized,collision.contacts[0].normal);
            transform.eulerAngles = direction;
            Rb.velocity = direction * Mathf.Max(speed,0) ;
        }

        if(collision.gameObject.tag == "Breakable")
        {
            collision.gameObject.layer = LayerMask.NameToLayer("Pieces");
            StartCoroutine("Breakable_Hit_Delay");
        }
      
    }

    IEnumerator Breakable_Hit_Delay()
    {
        yield return new WaitForSeconds(0.4f);
        Rb.isKinematic = false;
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(0.2f);
        canRun = true;
        Rb.isKinematic = false;
       
    }

    public void Death()
    {
        isDead = true;
        playerAnimator.speed = 1f;
         playerAnimator.SetBool("Sprint",false);
        Rb.isKinematic = true;
         Main_Box_Collider.enabled = false;
         playerAnimator.SetTrigger("Death_Trigger");
        Time.timeScale = 1;
        GetComponent<Player>().enabled = false;
        lineRenderer.enabled = false;
        trail.enabled = false;
        GameOver_Canvas.gameObject.SetActive(true);
        
    }

    public void Won()
    {
        confetti.SetActive(true);
        if(isWon)
        {
            playerAnimator.speed = 1f;
            playerAnimator.SetBool("Sprint",false);
        }
      startGame = false;
        WinScreen_Canvas.SetActive(true);
        
        Rb.isKinematic = true;
         Main_Box_Collider.enabled = false;
        playerAnimator.SetTrigger("Won");
        lineRenderer.enabled = false;
        trail.enabled = false;
    }

}
