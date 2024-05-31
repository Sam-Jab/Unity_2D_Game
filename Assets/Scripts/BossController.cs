using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{

    public int damage = 2 ;
    public float speed = 3.0f;
    public float stoppingDistanceNear = 1.0f; 
    public float stoppingDistanceFar = 10.0f ; 
    private Transform playerTransform;
    private Rigidbody2D rb;
    protected GameObject player ;
    Animator animator; 
    SpriteRenderer spriterenderer;

    public float health_Boss = 10;

    public float Health {
        set {
            health_Boss = Mathf.Max(0, value);  // Ensure health doesn't go below 0

            if (health_Boss <= 0) {
                Defeated();
            }
        }
        get {
            return health_Boss;
        }
    }
    void Start()
    {
        animator = GetComponent<Animator>() ;
        rb = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player") ; 
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else{
            Debug.Log("Player not Found !! ") ; 
        }
        
    }

   void FixedUpdate()
    {
        
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer > stoppingDistanceNear && distanceToPlayer <= stoppingDistanceFar)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                // Move the enemy using Rigidbody2D
                animator.SetTrigger("isMoving") ; 
                rb.velocity = direction * speed;
                Vector2Int BossMouvement = Vector2Int.RoundToInt(rb.velocity) ;
                if (BossMouvement.x < 0)
            { 
                spriterenderer.flipX = true;
            }
            else if (BossMouvement.x > 0)
            {
                spriterenderer.flipX = false;
            }
            }
            else if( distanceToPlayer <= stoppingDistanceNear ){
                animator.SetTrigger("BossAttack1") ; 
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }


    
    public void Defeated(){
        animator.SetTrigger("Defeated");
        // currentHealth = maxHealth;
        // obj.GenerateDungeon(); 
    }

    public void RemoveBoss() {
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has a PlayerHealth component
        PlayerControler player = collision.gameObject.GetComponent<PlayerControler>();
        if (player != null)
        {
            // Apply damage to the player
            player.TakeDamage(damage);
        }
    }

    public void TakeDamage(float damage){
        if(Health <= 0 ){
            Defeated() ; 
        }
        else 
            Health -= damage;

    }
}
