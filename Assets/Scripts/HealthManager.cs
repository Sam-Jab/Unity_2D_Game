using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance ; 
    public PlayerControler player ; 
    [SerializeField]
    private TMP_Text HealthDisplay ; 

    public void Awake()
    {
        if(!Instance)
        {
            Instance = this ; 
        }
    } 

    private void OnGUI()
    {
        HealthDisplay.text = player.currentHealth.ToString() ; 
    }

    // public void ChangeCoins(int amount ){
    //     Health -= amount ; 
    // }
}
