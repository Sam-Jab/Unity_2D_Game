using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance ; 
    private int coins ; 
    [SerializeField]
    private TMP_Text coinDisplay ; 

    public void Awake()
    {
        if(!Instance)
        {
            Instance = this ; 
        }
    } 

    private void OnGUI()
    {
        coinDisplay.text = coins.ToString() ; 
    }

    public void ChangeCoins(int amount ){
        coins += amount ; 
    }
}
