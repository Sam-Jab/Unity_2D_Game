// using System.Collections;
// using System.Collections.Generic;
// using TMPro;
// using UnityEngine;

// public class CoinManager : MonoBehaviour
// {
//     public static CoinManager Instance;
//     public int coins;
//     public List<GameObject> coinsprefab ; 
//     [SerializeField]
//     private TMP_Text coinDisplay;

//     public void Awake()
//     {
//         if (!Instance)
//         {
//             Instance = this;
//         }
//     }

//     public List<GameObject> FindCoinsInRadius(Vector3 playerPosition, float radius)
//     {
//         List<GameObject> nearbyCoins = new List<GameObject>();
//         foreach (GameObject coin in coinsprefab)
//         {
//             if (coin != null)
//             {
//                 float distance = Vector3.Distance(playerPosition, coin.transform.position);
//                 if (distance <= radius)
//                 {
//                     nearbyCoins.Add(coin);
//                     Debug.Log("Coin at " + coin.transform.position + " is within detection radius.");
//                 }
//             }
//         }
//         Debug.Log("Found " + nearbyCoins.Count + " coins within detection radius.");
//         return nearbyCoins;
//     }

//     private void GenerateCoinInRoom(BoundsInt roomBounds)
// {
//     int numberOfCoins = Random.Range(1, 20);
//     for (int i = 0; i < numberOfCoins; i++)
//     {
//         Vector3 randomPosition = new Vector3(Random.Range(roomBounds.min.x + 5, roomBounds.max.x - 5), Random.Range(roomBounds.min.y + 5, roomBounds.max.y - 5), 0);
//         GameObject newCoin = CoinFactory.CreateCoin(randomPosition, coinsParent);
//         coinInstances.Add(newCoin);

//         // Ajouter la pièce à la liste du CoinManager
//         CoinManager cm = FindObjectOfType<CoinManager>();
//         if (cm != null)
//         {
//             cm.coinsprefab.Add(newCoin);
//         }
//     }
// }

//     private void OnGUI()
//     {
//         coinDisplay.text = coins.ToString();
//     }

//     public void ChangeCoins(int amount)
//     {
//         coins += amount;
//     }
// }
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public int coins;
    public List<GameObject> coinsprefab;
    [SerializeField]
    private TMP_Text coinDisplay;

    public void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        UpdateCoinDisplay();
    }

    public void CollectCoin(GameObject coin)
    {
        coinsprefab.Remove(coin);
        Destroy(coin);
        ChangeCoins(1);
    }

    public void ChangeCoins(int amount)
    {
        coins += amount;
        UpdateCoinDisplay();
    }

    private void UpdateCoinDisplay()
    {
        coinDisplay.text = coins.ToString();
    }
}
