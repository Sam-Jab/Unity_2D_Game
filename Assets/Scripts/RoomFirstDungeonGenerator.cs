using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int minRoomWidth = 4, minRoomHeight = 4;
    [SerializeField]
    private int dungeonWidth = 20, dungeonHeight = 20;
    [SerializeField]
    [Range(0, 10)]
    private int offset = 1;
    [SerializeField]
    private bool randomWalkRooms = false;

    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject enemyPrefab;
    [SerializeField]
    private GameObject coinPrefab;

     [SerializeField]
    private GameObject BossPrefab;
    [SerializeField]
    public GameObject[] itemPrefabs;

    public  List<GameObject> instantiatedItems = new List<GameObject>()  ;
    private HashSet<Vector2Int> floor, corridors;

    private ItemPlacementHelper itemPlacementHelper;

    private Dictionary<Vector2Int, List<Vector2Int>> graph;
    protected override void RunProceduralGeneration()
    {
        
        CreateRooms();
        // PlaceBoss(playerStartPosition , BossPrefab) ; 
    }

    private void CreateRooms()
    {
        try{
        DestroyItems();
        }catch{}
        List<BoundsInt> roomsList = ProceduralGenerationAlgorithmes.BinarySpacePartitioning(new BoundsInt((Vector3Int)startPosition,
        new Vector3Int(dungeonWidth, dungeonHeight, 0)), minRoomWidth, minRoomHeight);

        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();

        if (randomWalkRooms)
        {
            floor = CreateRoomsRandomly(roomsList);
        }
        else
        {
            floor = CreateSimpleRooms(roomsList);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            var roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);
            roomCenters.Add(roomCenter);
        }

        if (roomCenters.Count == 0)
        {
            Debug.LogError("No rooms created. Check your room generation parameters.");
            return;
        }

        List<Vector2Int> roomCentersForCorridors = new List<Vector2Int>(roomCenters);
        HashSet<Vector2Int> corridors = ConnectRooms(roomCentersForCorridors);
        corridors = IncreaseCorridorSize(corridors);
        floor.UnionWith(corridors);

        // BuildGraph(roomCenters , corridors);

        tileMapVisualizer.PaintFloorTiles(floor);
        WallGenerator.CreateWalls(floor, tileMapVisualizer);

        itemPlacementHelper = new ItemPlacementHelper(floor, corridors);
        PlaceEnemies(roomCenters);
        PlaceCoins(roomCenters);
        Vector2Int playerPosition = PlacePlayer(playerPrefab , roomCenters) ; 
        PlaceBoss(playerPosition, BossPrefab , roomsList ) ; 
    }

    private void PlaceEnemies(List<Vector2Int> roomCenters)
    {
        int n = roomCenters.Count;

        for (int i = 0; i < n; i++)
        {
            PlaceItemsInOpenSpace(enemyPrefab);
        }
    }

    private void PlaceCoins(List<Vector2Int> roomCenters)
    {
        int n = roomCenters.Count * 2;

        for (int i = 0; i < n; i++)
        {
            PlaceItemsInOpenSpace(coinPrefab);
        }
    }

    public void DestroyItems()
    {

        instantiatedItems.Clear();

        GameObject[] enemy = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] coin = GameObject.FindGameObjectsWithTag("Coin");
        try{
        foreach(GameObject en in enemy){
            DestroyImmediate(en);
            Destroy(en) ; 
        }

        foreach(GameObject c in coin){
            DestroyImmediate(c);
            Destroy(c) ; 
        }
        
        }catch{

        }

    }

    private Vector2Int PlacePlayer(GameObject player , List<Vector2Int> roomCenters )
    {
        if (itemPlacementHelper == null)
        {
            itemPlacementHelper = new ItemPlacementHelper(floor, corridors);
        }

        Vector2? itemPosition = itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 100, new Vector2Int(1, 1), false);

        if (itemPosition != null && player != null)
        {
            player.transform.position = itemPosition.Value;
            // return Vector2Int.RoundToInt(itemPosition.Value) ; 
        }

        Vector2Int playerPosition = Vector2Int.RoundToInt(itemPosition.Value) ;
        
        Vector2Int closestRoomCenter = Vector2Int.zero;
        
        float minDistance = 10f ;

        foreach (var roomCenter in roomCenters)
        {
            float distance = Vector2Int.Distance(playerPosition, roomCenter);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestRoomCenter = roomCenter;
            }
    }
    return closestRoomCenter;
    }

    private void PlaceBoss(Vector2Int playerStartPosition, GameObject boss , List<BoundsInt> roomsList)
    {
        if (itemPlacementHelper == null)
        {
            itemPlacementHelper = new ItemPlacementHelper(floor, corridors);
        }

        List<Vector2Int> roomCenters = new List<Vector2Int>();
        foreach (var room in roomsList)
        {
            var roomCenter = (Vector2Int)Vector3Int.RoundToInt(room.center);
            roomCenters.Add(roomCenter);
        }

        Vector2Int farthestRoomCenter = GetFarthestRoom(playerStartPosition , roomCenters);

        if (boss != null)
        {
            boss.transform.position = new Vector3(farthestRoomCenter.x, farthestRoomCenter.y, 0);
        }
    }

 private Vector2Int GetFarthestRoom(Vector2Int playerStartPosition, List<Vector2Int> roomCenters)
    {
        Vector2Int farthestRoom = Vector2Int.zero;
        float maxDistance = 0f;

        foreach (Vector2Int roomCenter in roomCenters)
        {
            float distance = Vector2Int.Distance(playerStartPosition, roomCenter);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoom = roomCenter;
            }
        }

        return farthestRoom;
    }



    private void PlaceItemsInOpenSpace(GameObject obj)
    {
        if (itemPlacementHelper == null)
        {
            itemPlacementHelper = new ItemPlacementHelper(floor, corridors);
        }

        Vector2? itemPosition = itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 100, new Vector2Int(1, 1), false);

        if (itemPosition != null)
        {
            GameObject gameObject1 = Instantiate(obj, itemPosition.Value, Quaternion.identity);
            instantiatedItems.Add(gameObject1);
        }
    }

    private HashSet<Vector2Int> CreateRoomsRandomly(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        for (int i = 0; i < roomsList.Count; i++)
        {
            var roomBounds = roomsList[i];
            var roomCenter = new Vector2Int(Mathf.RoundToInt(roomBounds.center.x), Mathf.RoundToInt(roomBounds.center.y));
            var roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);
            foreach (var position in roomFloor)
            {
                if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) && position.y >= (roomBounds.yMin - offset) && position.y <= (roomBounds.yMax - offset))
                {
                    floor.Add(position);
                }
            }
        }
        return floor;
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
            if (destination.y > position.y)
            {
                position += Vector2Int.up;
            }
            else if (destination.y < position.y)
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
            }
            else if (destination.x < position.x)
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
            if (currentDistance < distance)
            {
                distance = currentDistance;
                closest = position;
            }
        }
        return closest;
    }

    private HashSet<Vector2Int> CreateSimpleRooms(List<BoundsInt> roomsList)
    {
        HashSet<Vector2Int> floor = new HashSet<Vector2Int>();
        foreach (var room in roomsList)
        {
            for (int col = offset; col < room.size.x - offset; col++)
            {
                for (int row = offset; row < room.size.y - offset; row++)
                {
                    Vector2Int position = (Vector2Int)room.min + new Vector2Int(col, row);
                    floor.Add(position);
                }
            }
        }
        return floor;
    }


    private HashSet<Vector2Int> IncreaseCorridorSize(HashSet<Vector2Int> corridors)
    {
        HashSet<Vector2Int> enlargedCorridors = new HashSet<Vector2Int>();
        foreach (var corridorTile in corridors)
        {
            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    enlargedCorridors.Add(corridorTile + new Vector2Int(x, y));
                }
            }
        }
        return enlargedCorridors;
    }
}
