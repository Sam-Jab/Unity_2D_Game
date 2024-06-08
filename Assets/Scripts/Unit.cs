// using UnityEngine;
// using System.Collections;

// public class Unit : MonoBehaviour
// {
//     public Transform target;
//     float speed = 10;
//     Vector3[] path;
//     int targetIndex;

//     void Update()
//     {
//         if(Input.GetKeyDown(KeyCode.X)){
//             // Debug.Log("Path is Found !!") ; 
//             PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
//         }
//     }

//     public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
//     {
//         if (pathSuccessful)
//         {
//             path = newPath;
//             targetIndex = 0;
//             StopCoroutine("FollowPath");
//             StartCoroutine("FollowPath");
//         }
//     }

//     IEnumerator FollowPath()
//     {
//         Vector3 currentWaypoint = path[0];
//         while (true)
//         {
//             if (transform.position == currentWaypoint)
//             {
//                 targetIndex++;
//                 if (targetIndex >= path.Length)
//                 {
//                     yield break;
//                 }
//                 currentWaypoint = path[targetIndex];
//             }

//             transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
//             yield return null;

//         }
//     }

//     public void OnDrawGizmos()
//     {
//         if (path != null)
//         {
//             for (int i = targetIndex; i < path.Length; i++)
//             {
//                 Gizmos.color = Color.black;
//                 Gizmos.DrawCube(path[i], Vector3.one);

//                 if (i == targetIndex)
//                 {
//                     Gizmos.DrawLine(transform.position, path[i]);
//                 }
//                 else
//                 {
//                     Gizmos.DrawLine(path[i - 1], path[i]);
//                 }
//             }
//         }
//     }
// }



using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour
{
    public float speed = 10;
    private Vector3[] path;
    private int targetIndex;
    private List<Coin> coins;
    private Coin targetCoin;
    public Transform target;

    void Update()
    {
        // Find all coins in the scene
        if(Input.GetKeyDown(KeyCode.C)){
        coins = new List<Coin>(Object.FindObjectsByType<Coin>(FindObjectsSortMode.None));
        MoveToNextCoin();
        }
        if(Input.GetKeyDown(KeyCode.X)){
            // Debug.Log("Path is Found !!") ; 
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }
    }

    void MoveToNextCoin()
    {
        if (coins.Count > 0)
        {
            // Find the closest coin
            targetCoin = FindClosestCoin();
            if (targetCoin != null)
            {
                PathRequestManager.RequestPath(transform.position, targetCoin.transform.position, OnPathFound);
            }
        }
    }

    Coin FindClosestCoin()
    {
        Coin closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Coin coin in coins)
        {
            float distance = Vector3.Distance(transform.position, coin.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = coin;
            }
        }

        return closest;
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        if (path == null || path.Length == 0)
            yield break;

        Vector3 currentWaypoint = path[0];
        while (true)
        {
            if (Vector3.Distance(transform.position, currentWaypoint) < 0.1f)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    CollectCoin(targetCoin);
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    void CollectCoin(Coin coin)
    {
        if (coin != null)
        {
            coins.Remove(coin);
            Destroy(coin.gameObject);
            MoveToNextCoin();
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one * 0.5f);

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
