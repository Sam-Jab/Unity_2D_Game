using UnityEngine;

public class EnemyFollowPlayer : MonoBehaviour
{
// public GameObject player; // Reference to the player GameObject
    public float speed = 2.0f; // Speed at which the enemy follows the player
    public float stoppingDistanceNear = 1.0f; // Distance at which the enemy stops moving towards the player
    public float stoppingDistanceFar = 10.0f ; 
    private Transform playerTransform;
    private Rigidbody2D rb;
    protected GameObject player ;
    Animator animator1 ; 
    
    void Start()
    {
        animator1 = GetComponent<Animator>() ;
        rb = GetComponent<Rigidbody2D>();
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
                animator1.SetTrigger("isMoving") ; 
                rb.velocity = direction * speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
}
