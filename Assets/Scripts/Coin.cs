using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int value  ; 
    private bool hasTriggered ;
    private CoinManager coinManager ; 
    private void Start(){
        coinManager = CoinManager.Instance ; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true ; 
            coinManager.ChangeCoins(value) ; 
            Destroy(gameObject) ; 
        }  
    } 
}
