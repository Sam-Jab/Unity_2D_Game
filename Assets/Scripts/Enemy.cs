using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    // public PlayerControler player ; 
    public int damage = 1;
    EnemyFollowPlayer enemyFollowPlayer ; 
     Animator animator;

    public float Health {
        set {
            health = value;

            if(health <= 0) {
                Defeated();
            }
        }
        get {
            return health;
        }
    }

    public float health = 1;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    public void Defeated(){
        animator.SetTrigger("Defeated");
        //Destroy(gameObject) ; 
    }

    public void RemoveEnemy() {
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


}
