using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random ;
using UnityEngine.Tilemaps;

public class RoomFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int minRoomWidth = 4 , minRoomHeight = 4 ; 
    [SerializeField]
    private int dungeonWidth =20 , dungeonHeight = 20 ; 
    [SerializeField]
    [Range(0 , 10)]
    private int offset = 1 ; 
    [SerializeField]
    private bool randomWalkRooms = false ; 

    [SerializeField]
    private GameObject playerPrefab ; 
    [SerializeField]
    private GameObject enemyPrefab ; 

    [SerializeField]
    private GameObject coinPrefab ; 

     [SerializeField]
    public GameObject[] itemPrefabs;

    private List<GameObject> instantiatedItems = new List<GameObject>();
    private HashSet<Vector2Int> floor , corridors ; 
    
    private ItemPlacementHelper itemPlacementHelper; 

    
    protected override void RunProceduralGeneration()
    {
        // DestroyItems() ;  
        CreateRooms() ;  
       
        PlacePlayer(playerPrefab) ; 
        // PlaceEnemies() ; 
        // PlaceCoins() ; 
    }

    private void CreateRooms(){

        List<BoundsInt> roomsList = ProceduralGenerationAlgorithmes.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition 
        , new Vector3Int(dungeonWidth , dungeonHeight , 0  )) , minRoomWidth , minRoomHeight) ;

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>() ; 
        
        if(randomWalkRooms){
            floor = CreateRoomsRandomly(roomsList) ; 
        }
        else
        {
            floor = CreateSimpleRooms(roomsList) ; 
        } 
        List<Vector2Int> roomCenters = new List<Vector2Int>() ;  
        foreach (var room in roomsList)
        {
        var roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);
            roomCenters.Add(roomCenter);
            // Debug.Log("Room center: " + roomCenter);
        }
        if (roomCenters.Count == 0)
        {
            Debug.LogError("No rooms created. Check your room generation parameters.");
            return;
        }

        // Make a copy of roomCenters for use in corridor generation
        List<Vector2Int> roomCentersForCorridors = new List<Vector2Int>(roomCenters);

        HashSet<Vector2Int> corridors = ConnectRooms(roomCentersForCorridors) ; 
        floor.UnionWith(corridors) ; 
        
        tileMapVisualizer.PaintFloorTiles(floor) ; 
        WallGenerator.CreateWalls(floor , tileMapVisualizer) ; 
        DestroyItems() ; 
        // PlaceEnemies(roomCenters) ;  
        itemPlacementHelper = new ItemPlacementHelper(floor, corridors);
        PlaceEnemies(roomCenters) ; 
        PlaceCoins(roomCenters) ;

    }


    private void PlaceEnemies(List<Vector2Int> roomCenters)
    { 
        int  n = roomCenters.Count ;  

        for (int i = 0; i < n ; i++)
        {
        PlaceItems(enemyPrefab) ; 
        }
    }

    private void PlaceCoins(List<Vector2Int> roomCenters)
    { 
        int n = roomCenters.Count * 2  ; 

        for (int i = 0; i < n ; i++)
        {
        PlaceItems(coinPrefab) ; 
        }
    }


    public void DestroyItems()
    {
        foreach (GameObject item in instantiatedItems)
        {
            
            //if(!playerPrefab)
            // Debug.LogWarning("lopp"+i);
            Destroy(item);

            // DestroyImmediate(item , true) ; 

            
        }
        
        instantiatedItems.Clear(); // Clear the list after destroying items
    }

    private void PlacePlayer(GameObject player)
    {
        // Ensure itemPlacementHelper is initialized
        if (itemPlacementHelper == null)
        {
            itemPlacementHelper = new ItemPlacementHelper(floor, corridors);
        }

        // Example: Place item of type PlacementType.OpenSpace
        Vector2? itemPosition = itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 100, new Vector2Int(1, 1), false);

        if (itemPosition != null && player != null)
        {
            player.transform.position = itemPosition.Value ;  
            //instantiatedItems.Add(player);
        }
    }
    private void PlaceItems(GameObject obj )
    {
        // Ensure itemPlacementHelper is initialized
        if (itemPlacementHelper == null)
        {
            itemPlacementHelper = new ItemPlacementHelper(floor, corridors);
        }

        // Example: Place item of type PlacementType.OpenSpace
        Vector2? itemPosition = itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 100, new Vector2Int(1, 1), false);

        if (itemPosition != null)
        {
            // Instantiate item GameObject at itemPosition.Value
            // GameObject itemPrefab = itemPrefabs[UnityEngine.Random.Range(0, itemPrefabs.Length)];
            GameObject gameObject1 = Instantiate(obj, itemPosition.Value, Quaternion.identity);
            instantiatedItems.Add(gameObject1);
        }

        // if (itemPosition1 != null)
        // {
        //     // Instantiate item GameObject at itemPosition.Value
        //     GameObject itemPrefab = itemPrefabs[UnityEngine.Random.Range(0, itemPrefabs.Length)];
        //     GameObject gameObject2 = Instantiate(itemPrefab, itemPosition.Value, Quaternion.identity);
        //     instantiatedItems.Add(gameObject2);
        // }
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        // List<Vector2Int> floors = new List<Vector2Int>() ;  
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if(position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                    // floors.Add(position) ; 
                }
            }
        }
        // PlaceEnemies(floors) ; 
        return floor ; 
    }

    private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
    {
        HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
        var currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
        roomCenters.Remove(currentRoomCenter);

        while (roomCenters.Count > 0)
        {
            Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
            roomCenters.Remove(closest);
            HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
            currentRoomCenter = closest;
            corridors.UnionWith(newCorridor);
            
        }
        return corridors;
    }
    private HashSet<Vector2Int> CreateCorridor(Vector2Int currentRoomCenter, Vector2Int destination)
    {
        HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
        var position = currentRoomCenter;
        corridor.Add(position);
        while (position.y != destination.y)
        {
            if(destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if(destination.y < position.y)
            {
                position += Vector2Int.down;
            }
            corridor.Add(position);
        }
        while (position.x != destination.x)
        {
            if (destination.x > position.x)
            {
                position += Vector2Int.right;
            }else if(destination.x < position.x)
            {
                position += Vector2Int.left;
            }
            corridor.Add(position);
        }
        return corridor;
    }

    private Vector2Int FindClosestPointTo(Vector2Int currentRoomCenter, List<Vector2Int> roomCenters)
    {
        Vector2Int closest = Vector2Int.zero;
        float distance = float.MaxValue;
        foreach (var position in roomCenters)
        {
            float currentDistance = Vector2.Distance(position, currentRoomCenter);
            if(currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>() ; 
        foreach( var room in roomsList)
        {
            for(int col=offset ; col < room.size.x - offset ; col++ )
            {
                for(int row = offset ; row < room.size.y - offset ; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col , row) ;
                    floor.Add(position) ; 
                }
            }
        }
        return floor ; 
    }
}
