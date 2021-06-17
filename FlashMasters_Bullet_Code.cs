using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject player ;
    public float bulletSpeed = 2;
    public Player player_Script;
    public ParticleSystem bullet_Particle;
    bool particleBool;
    
    void Start()
    {
        particleBool = false;
        player = GameObject.Find("PlayerObj");
        player_Script = GameObject.Find("PlayerObj").GetComponent<Player>();

    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * bulletSpeed);
        Destroy(gameObject, 5);
        
    }

    private void OnTriggerEnter(Collider other) {
       if(other.gameObject.CompareTag("Player"))
       {
           particleBool = true;
           
           if(particleBool)
           {
               Instantiate(bullet_Particle,transform.position ,Quaternion.identity);
           }
         
          player_Script.Death();
          

       }
   }
}
