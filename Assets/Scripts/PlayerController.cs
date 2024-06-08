// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
// using UnityEngine.InputSystem;
// using UnityEngine.PlayerLoop;

// public class PlayerControler : MonoBehaviour
// {
//     public int maxHealth = 10;
//     public  CoinManager cm  ; 
//     public int currentHealth;
//     public float moveSpeed = 1f; //we do it public so we can modify it just from unity without code 
//     public float collisionOffset = 0.05f;
//     public ContactFilter2D mouvementFilter;
//     Vector2 mouvementInput;
//     SpriteRenderer spriterenderer;
//     Rigidbody2D rb;
//     Animator animator;
//     List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
//     public SwordAttack swordAttack ;
//     public bool canMove = true;
//     // Start is called before the first frame update
//     // private HealthManager healthManager;

//     public AbstractDungeonGenerator obj;

//     private Dictionary<Vector3, GameObject> nearbyCoinsCache = new Dictionary<Vector3, GameObject>();
//     void Start()
//     {
//         // healthManager = HealthManager.Instance;
//         rb = GetComponent<Rigidbody2D>();
//         animator = GetComponent<Animator>();
//         spriterenderer = GetComponent<SpriteRenderer>();
//         currentHealth = maxHealth;
        
//     }

//     private List<GameObject> GetCoinsInRadius()
//     {
//         List<GameObject> nearbyCoins = new List<GameObject>();
//         foreach (var coin in cm.coins)
//         {
//             if (coin != null)
//             {
//                 if (nearbyCoinsCache.ContainsKey(coin.transform.position))
//                 {
//                     nearbyCoins.Add(coin);
//                 }
//                 else
//                 {
//                     float distance = Vector3.Distance(transform.position, coin.transform.position);
//                     if (distance <= coinDetectionRadius)
//                     {
//                         nearbyCoins.Add(coin);
//                         nearbyCoinsCache[coin.transform.position] = coin;
//                     }
//                 }
//             }
//         }
//         return nearbyCoins;
//     }

//     private void CollectNearbyCoins()
//     {
//         List<GameObject> nearbyCoins = GetCoinsInRadius();
//         if (nearbyCoins.Count > 0)
//         {
//             Debug.Log("Found " + nearbyCoins.Count + " coins within detection radius.");
//             StartCoroutine(MoveToCollectCoins(nearbyCoins));
//         }
//         else
//         {
//             Debug.Log("No coins found within detection radius.");
//         }
//     }

//     private IEnumerator MoveToCollectCoins(List<GameObject> coins)
//     {
//         foreach (var coin in coins)
//         {
//             if (coin != null)
//             {
//                 while (coin != null && Vector3.Distance(transform.position, coin.transform.position) > 0.1f)
//                 {
//                     transform.position = Vector3.MoveTowards(transform.position, coin.transform.position, speed * Time.deltaTime);
//                     yield return null;
//                 }

//                 if (coin != null)
//                 {
//                     cm.CollectCoin(coin);
//                     nearbyCoinsCache.Remove(coin.transform.position);
//                     Debug.Log("Collected coin at " + coin.transform.position);
//                 }
//             }
//         }
//     }


//     public void TakeDamage(int damage)
//     {
//         currentHealth -= damage;
//         // healthManager.ChangeCoins(damage);
//         if (currentHealth <= 0)
//         {
//             animator.SetTrigger("Death");
//             Die() ; 
//         }
//     }
//     private void Die()
//     {
//         Debug.Log("Player Died");
//         currentHealth = maxHealth;
//         obj.GenerateDungeon();
//         // coinManager.coins = 0 ; 
       
//     }
//     public void FixedUpdate()
//     {
//         if (canMove)
//         {
//             if (mouvementInput != Vector2.zero)
//             {
//                 bool success = TryMove(mouvementInput);
//                 if (!success)
//                 {
//                     success = TryMove(new Vector2(mouvementInput.x, 0));

//                     if (!success)
//                     {
//                         success = TryMove(new Vector2(0, mouvementInput.y));
//                     }
//                 }
//                 animator.SetBool("isMoving", success);
//             }
//             else
//             {
//                 animator.SetBool("isMoving", false);
//             }
//             // Set the direction of spite to the mouvement  direction 
//             if (mouvementInput.x < 0)
//             {
//                 spriterenderer.flipX = true;
//             }
//             else if (mouvementInput.x > 0)
//             {
//                 spriterenderer.flipX = false;
//             }

//         }
//     }

//     private void Update()
//     {

//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             animator.SetTrigger("swordAttack");
//         }
//     }
//     private bool TryMove(Vector2 direction)
//     {
//         if (direction != Vector2.zero)
//         {
//             int count = rb.Cast(direction,
//                 mouvementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);
//             if (count == 0)
//             {
//                 rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
//                 return true;
//             }
//             return false;
//         }
//         else
//         {

//             // can't move if there is not a direction to move in  
//             return false;
//         }
//     }
//     void OnMove(InputValue mouvementValue)
//     {
//         mouvementInput = mouvementValue.Get<Vector2>();
//     }
//     // void OnFire(){
//     //     animator.SetTrigger("swordAttack") ; 
//     // }

//     public SwordAttack GetSwordAttack()
//     {
//         return swordAttack;
//     }

//     public void SwordAttack(SwordAttack swordAttack)
//     {
//         LockMovement();
//         if (spriterenderer.flipX == true)
//         {
//             GetSwordAttack().AttackLeft();
//         }
//         else
//         {
//             GetSwordAttack().AttackRight();
//         }
//     }

//     public void EndSwordAttack()
//     {
//         UnlockMovement();
//         GetSwordAttack().StopAttack();
//     }

//     public void LockMovement()
//     {
//         canMove = false;
//     }

//     public void UnlockMovement()
//     {
//         canMove = true;
//     }


// }

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public int maxHealth = 10;
    public CoinManager cm;
    public int currentHealth;
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D mouvementFilter;
    public float coinDetectionRadius = 10f; // Define detection radius
    Vector2 mouvementInput;
    SpriteRenderer spriterenderer;
    Rigidbody2D rb;
    Animator animator;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public SwordAttack swordAttack;
    public bool canMove = true;
    public float Speed=8f;

    public AbstractDungeonGenerator obj;

    private Dictionary<Vector3, GameObject> nearbyCoinsCache = new Dictionary<Vector3, GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriterenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    private List<GameObject> GetCoinsInRadius()
    {
        List<GameObject> nearbyCoins = new List<GameObject>();
        GameObject[] coinsObjects  = GameObject.FindGameObjectsWithTag("Coin"); 
        cm.coinsprefab = coinsObjects.ToList() ; 
        foreach (var coin in cm.coinsprefab)
        {
            if (coin != null)
            {
                float distance = Vector3.Distance(transform.position, coin.transform.position);
                if (distance <= coinDetectionRadius)
                {
                    nearbyCoins.Add(coin);
                }
            }
        }
        return nearbyCoins;
    }

    private void CollectNearbyCoins()
    {
        List<GameObject> nearbyCoins = GetCoinsInRadius();
        if (nearbyCoins.Count > 0)
        {
            Debug.Log("Found " + nearbyCoins.Count + " coins within detection radius.");
                StartCoroutine(MoveToCollectCoins(nearbyCoins));
        }
        else
        {
            Debug.Log("No coins found within detection radius.");
        }
    }

    private IEnumerator MoveToCollectCoins(List<GameObject> coins)
    {
        foreach (var coin in coins)
        {
            if (coin != null)
            {
                while (coin != null && Vector3.Distance(transform.position, coin.transform.position) > 0.1f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, coin.transform.position, Speed * Time.deltaTime);
                    yield return null;
                }

                if (coin != null)
                {
                    cm.CollectCoin(coin);
                    Debug.Log("Collected coin at " + coin.transform.position);
                }
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            animator.SetTrigger("Death");
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died");
        obj.GenerateDungeon() ; 
        cm.coins = 0 ; 
        currentHealth = maxHealth;
    }

    void FixedUpdate()
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
         if (Input.GetKeyDown(KeyCode.V))
        {
            // Call CollectNearbyCoins when the "A" key is pressed
            animator.SetTrigger("isMoving");
            CollectNearbyCoins();
        }
    }

    private bool TryMove(Vector2 direction)
    {
        if (direction != Vector2.zero)
        {
            int count = rb.Cast(direction, mouvementFilter, castCollisions, moveSpeed * Time.fixedDeltaTime + collisionOffset);
            if (count == 0)
            {
                rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    void OnMove(InputValue mouvementValue)
    {
        mouvementInput = mouvementValue.Get<Vector2>();
    }

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
