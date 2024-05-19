using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using Random = UnityEngine.Random ; 


public class CorridorFirstDungeonGenerator : SimpleRandomWalkMapGenerator
{
    [SerializeField]
    private int corridorLength= 50 , corridorCount = 20 ; 
    [SerializeField]
    [Range(0.01f , 1)]
    private float roomPercent = 0.8f ;
    [SerializeField]
    public GameObject[] itemPrefabs;
    [SerializeField]
    public Tile[] itemTiles ;  

    //PCG Data 
    private Dictionary<Vector2Int , HashSet<Vector2Int>> roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>() ; 

    private HashSet<Vector2Int> floorPositions , corridorsPositions ; 

    //Gizmo Data 
    // private List<Color> roomColors = new List<Color>() ;
    // [SerializeField]
    // private bool showRoomGizmo = false , showCorridorsGizmo ; 



    private List<GameObject> instantiatedItems = new List<GameObject>();


    private ItemPlacementHelper itemPlacementHelper; 
    protected override void RunProceduralGeneration()
    {
        DestroyItems() ; 
        CorridorFirstGeneration() ; 
        PlaceItems() ; 
    }

    private void CorridorFirstGeneration()
    {
        
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>() ; 
        
        HashSet<Vector2Int> potentialRoomPositions = new HashSet<Vector2Int>() ; 

        CreateCorridors(floorPositions , potentialRoomPositions) ;  

        HashSet<Vector2Int> RoomPositions = CreateRooms(potentialRoomPositions) ;

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPositions) ; 

        CreateRoomAtDeadEnds(deadEnds , RoomPositions) ; 
        
        floorPositions.UnionWith(RoomPositions) ;   
        
        tileMapVisualizer.PaintFloorTiles(floorPositions) ; 
        
        WallGenerator.CreateWalls(floorPositions , tileMapVisualizer) ; 
        itemPlacementHelper = new ItemPlacementHelper(floorPositions, corridorsPositions);

    }
    private void DestroyItems()
    {
        foreach (GameObject item in instantiatedItems)
        {
            DestroyImmediate(item , true) ; 
            // Destroy(item);
        }
        instantiatedItems.Clear(); // Clear the list after destroying items
    }
    


    private void PlaceItems()
    {
        // Ensure itemPlacementHelper is initialized
        if (itemPlacementHelper == null)
        {
            itemPlacementHelper = new ItemPlacementHelper(floorPositions, corridorsPositions);
        }

        // Example: Place item of type PlacementType.OpenSpace
        Vector2? itemPosition = itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 100, new Vector2Int(1, 1), false);
        Vector2? itemPosition1 = itemPlacementHelper.GetItemPlacementPosition(PlacementType.NearWall, 100, new Vector2Int(1, 1), false);

        if (itemPosition != null)
        {
            // Instantiate item GameObject at itemPosition.Value
            GameObject itemPrefab = itemPrefabs[UnityEngine.Random.Range(0, itemPrefabs.Length)];
            GameObject gameObject1 = Instantiate(itemPrefab, itemPosition.Value, Quaternion.identity);
            instantiatedItems.Add(gameObject1);
        }
    }

//     private void PlaceItems()
// {
//     // Ensure itemPlacementHelper is initialized
//     if (itemPlacementHelper == null)
//     {
//         itemPlacementHelper = new ItemPlacementHelper(floorPositions, corridorsPositions);
//     }

//     List<Vector2Int> DeadEnds = FindAllDeadEnds(floorPositions) ; 
//     foreach (var deadEndPosition in DeadEnds)
//     {
//         // Check if the current position is a dead end
//         if (floorPositions.Contains(deadEndPosition))
//         {
//             // Place items in dead-end room
//             Vector2? itemPosition = itemPlacementHelper.GetItemPlacementPosition(PlacementType.OpenSpace, 100, new Vector2Int(1, 1), false);

//             if (itemPosition != null)
//             {
//                 // Instantiate item GameObject at itemPosition.Value
//                 GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
//                 GameObject itemInstance = Instantiate(itemPrefab, itemPosition.Value, Quaternion.identity);
//                 instantiatedItems.Add(itemInstance);
//             }
//         }
//     }
// }

    private void CreateRoomAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomFloors)
    {
        foreach( var position in deadEnds)
        {
            if(roomFloors.Contains(position) == false ){
                var room = RunRandomWalk(randomWalkParameters , position) ; 
                roomFloors.UnionWith(room) ; 
            }
        }
        // PlaceItems() ; 
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPositions ) 
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>() ; 

        foreach(var position in floorPositions )
        {
            int neighboursCount = 0 ; 
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
            if(floorPositions.Contains(position + direction )) neighboursCount++ ; 
            }
            if(neighboursCount == 1) deadEnds.Add(position) ; 
        }
        return deadEnds ; 
    }


    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPositions)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>() ; 
        int roomToCreateCount = Mathf.RoundToInt(potentialRoomPositions.Count * roomPercent) ; 

        List<Vector2Int> roomsToCreate = potentialRoomPositions.OrderBy(x => Guid.NewGuid()).Take(roomToCreateCount).ToList() ; 

        foreach(var roomPosition in roomsToCreate ){
            var roomFloor = RunRandomWalk(randomWalkParameters , roomPosition) ; 
            
            // SaveRoomData(roomPosition , roomFloor) ; 
            roomPositions.UnionWith(roomFloor) ; 
        }
        return roomPositions ; 
    }

    // private void ClearRoomData(){
    //     roomsDictionary.Clear() ; 
    //     roomColors.Clear() ;
    // }

    // private void SaveRoomData(Vector2Int roomPosition, HashSet<Vector2Int> roomFloor)
    // {
    //     roomsDictionary[roomPosition] = roomFloor ; 
    //     roomColors.Add(UnityEngine.Random.ColorHSV()) ; 
    // }

    private void CreateCorridors(HashSet<Vector2Int> floorPositions ,HashSet<Vector2Int> potentialRoomPositions )
    {
       var currentPosition = startPosition ;
       
       potentialRoomPositions.Add(currentPosition) ; 

       for(int i=0 ; i< corridorCount ; i++)
       {
            var  corridor = ProceduralGenerationAlgorithmes.RandomWalkCorridor(currentPosition , corridorLength) ; 
            currentPosition = corridor[corridor.Count - 1] ; 
            potentialRoomPositions.Add(currentPosition) ; 
            floorPositions.UnionWith(corridor) ; 
       } 
       corridorsPositions = new HashSet<Vector2Int>(floorPositions) ; 
    }
}
