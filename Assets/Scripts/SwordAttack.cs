using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider2D swordCollider ; 
    public float damage = 2 ; 
    public Vector2 RightAttackOffset ; 
    private void Start(){
        // swordCollider = GetComponent<Collider2D>() ; 
        RightAttackOffset = transform.localPosition ;  
    }
    public void AttackRight() {
        // Debug.Log("Attack Right ") ; 
        swordCollider.enabled = true ; 
        transform.localPosition = RightAttackOffset ; 
    }
    public void AttackLeft(){
        // Debug.Log("Attack Left ") ; 
        swordCollider.enabled = true ; 
        transform.localPosition = new Vector3(RightAttackOffset.x * -1 , RightAttackOffset.y) ; 
    }
    public void StopAttack(){
        swordCollider.enabled = false ; 
    }

    private  void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Enemy" ){
            //Deal Damage to the Enemy 
            Enemy enemy = other.GetComponent<Enemy>() ; 
            
            if(enemy != null){
                enemy.Health -= damage ; 
            }
        }
        if(other.tag == "Boss"){
            
            BossController boss = other.GetComponent<BossController>() ; 

            if(boss != null ){
                boss.TakeDamage(damage) ; 
            }
        }
    }
}
