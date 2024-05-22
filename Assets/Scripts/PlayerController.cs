using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

public class PlayerControler : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth;
    public float moveSpeed = 1f; //we do it public so we can modify it just from unity without code 
    public float collisionOffset = 0.05f;
    public ContactFilter2D mouvementFilter;
    Vector2 mouvementInput;
    SpriteRenderer spriterenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public SwordAttack swordAttack ;
    public bool canMove = true;
    // Start is called before the first frame update
    // private HealthManager healthManager;

    public AbstractDungeonGenerator obj;
    void Start()
    {
        // healthManager = HealthManager.Instance;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        // healthManager.ChangeCoins(damage);
        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            
            // StartWaiting() ; 
            // obj.DestroyItems();
            Die() ; 
        }
    }
    private void Die()
    {
        // Handle player death (e.g., play animation, reload level)
        
       
        // obj.RunProceduralGeneration() ; 
        Debug.Log("Player Died");
        currentHealth = maxHealth;
        obj.GenerateDungeon();
        // foreach(GameObject ob in RoomFirstDungeonGenerator.instantiatedItems){
        //     Destroy(ob);
        //     RoomFirstDungeonGenerator.instantiatedItems.Remove(ob);

        // }

    }
    // public void ReloadGame(){
    //     obj
    // }

    public void FixedUpdate()
    {
        if (canMove)
        {
            if (mouvementInput != Vector2.zero)
            {
                bool success = TryMove(mouvementInput);
                if (!success)
                {
                    success = TryMove(new Vector2(mouvementInput.x, 0));

                    if (!success)
                    {
                        success = TryMove(new Vector2(0, mouvementInput.y));
                    }
                }
                animator.SetBool("isMoving", success);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
            // Set the direction of spite to the mouvement  direction 
            if (mouvementInput.x < 0)
            {
                spriterenderer.flipX = true;
            }
            else if (mouvementInput.x > 0)
            {
                spriterenderer.flipX = false;
            }

        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("swordAttack");
        }
    }
    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(direction,
                mouvementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);
            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            return false;
        }
        else
        {

            // can't move if there is not a direction to move in  
            return false;
        }
    }
    void OnMove(InputValue mouvementValue)
    {
        mouvementInput = mouvementValue.Get<Vector2>();
    }
    // void OnFire(){
    //     animator.SetTrigger("swordAttack") ; 
    // }

    public SwordAttack GetSwordAttack()
    {
        return swordAttack;
    }

    public void SwordAttack(SwordAttack swordAttack)
    {
        LockMovement();
        if (spriterenderer.flipX == true)
        {
            GetSwordAttack().AttackLeft();
        }
        else
        {
            GetSwordAttack().AttackRight();
        }
    }

    public void EndSwordAttack()
    {
        UnlockMovement();
        GetSwordAttack().StopAttack();
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }


}

