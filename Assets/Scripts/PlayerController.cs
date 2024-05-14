using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem ; 

public class PlayerControler : MonoBehaviour
{
    public float moveSpeed = 1f ; //we do it public so we can modify it just from unity without code 
    public float collisionOffset = 0.05f ; 
    public ContactFilter2D mouvementFilter ; 
    Vector2 mouvementInput ; 
    SpriteRenderer spriterenderer ; 
    Rigidbody2D rb ;  
    Animator animator ; 
    List<RaycastHit2D> castCollisions  = new List<RaycastHit2D>() ; 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>() ; 
        animator = GetComponent<Animator>() ; 
        spriterenderer = GetComponent<SpriteRenderer>() ; 
    }

    public void FixedUpdate(){
        if(mouvementInput != Vector2.zero){
            bool success = TryMove(mouvementInput) ;
            if(!success){
                success = TryMove(new Vector2(mouvementInput.x , 0)) ; 
             
            if(!success){
                success = TryMove(new Vector2(0 , mouvementInput.y)) ; 
               }
            }
            animator.SetBool("isMoving" , success) ; 
        }
        else{
            animator.SetBool("isMoving" , false) ; 
        }
        // Set the direction of spite to the mouvement  direction 
        if(mouvementInput.x < 0 ){
            spriterenderer.flipX = true ; 
        }
        else if(mouvementInput.x > 0){
            spriterenderer.flipX = false ; 
        }
        
    }
    private bool TryMove(Vector2 direction ){
        if(direction != Vector2.zero){
        int count = rb.Cast(direction , 
            mouvementFilter , castCollisions , moveSpeed * Time.fixedDeltaTime + collisionOffset ) ;  
           if(count == 0 ){
            rb.MovePosition(rb.position + direction  * moveSpeed * Time.fixedDeltaTime ) ;
           return true ; 
           } 
           return false ; 
        }
        else {

            // can't move if there is not a direction to move in  
            return false ; 
        }
    }
    void OnMove(InputValue mouvementValue) {
        mouvementInput = mouvementValue.Get<Vector2>() ; 
    }
}

