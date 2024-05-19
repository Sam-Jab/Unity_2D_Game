using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq  ;
using System;

public class ItemPlacementHelper 
{
    private static ItemPlacementHelper instance;

    // public static ItemPlacementHelper Instance
    // {
    //     get
    //     {
    //         if (instance == null)
    //         {
    //             instance = new ItemPlacementHelper();
    //         }
    //         return instance;
    //     }
    // }
    Dictionary<PlacementType , HashSet<Vector2Int>> tileByType = new Dictionary<PlacementType, HashSet<Vector2Int>>() ; 

    HashSet<Vector2Int> roomFloorNoCorridor ; 
    public ItemPlacementHelper(HashSet<Vector2Int> roomFloor , HashSet<Vector2Int> roomFloorNoCorridor)
    {
        Graph graph = new Graph(roomFloor) ; 
        this.roomFloorNoCorridor = roomFloorNoCorridor ; 
        foreach(var position in roomFloorNoCorridor)
        {
            int neighboursCount8Dir = graph.GetNeighbours8Directions(position).Count ; 
            PlacementType type = neighboursCount8Dir < 8 ? PlacementType.NearWall : PlacementType.OpenSpace ;

            if(tileByType.ContainsKey(type) == false )
                tileByType[type] = new HashSet<Vector2Int>() ;
            if(type == PlacementType.NearWall && graph.GetNeighbours4Directions(position).Count < 8)
            continue ; 
            tileByType[type].Add(position) ;  
        }
    }
    // public Vector2? GetItemPlacementPosition(PlacementType placementType , int iterationsMax , Vector2Int size , bool addOfsset)
    // {
    //     int itemArea = size.x * size.y ; 
    //     if(tileByType[placementType].Count < itemArea)
    //     return null ; 
    //     int iteration = 0 ; 
    //     while(iteration < iterationsMax)
    //     {
    //         iteration++ ; 
    //         int index = UnityEngine.Random.Range(0 , tileByType[placementType].Count) ; 
    //         Vector2Int position = tileByType[placementType].ElementAt(index) ; 

    //         if(itemArea > 1)
    //         {
    //             var (result , placementPositions) = PlaceBigItem(position , size , addOfsset) ; 
    //             if(result == false)
    //                 continue ;
    //             tileByType[placementType].ExceptWith(placementPositions) ; 
    //             tileByType[PlacementType.NearWall].ExceptWith(placementPositions) ;  

    //         }
    //         else
    //         {
    //             tileByType[placementType].Remove(position) ; 
    //         }
    //         return position ; 
    //     }
    // }

    public Vector2? GetItemPlacementPosition(PlacementType placementType, int iterationsMax, Vector2Int size, bool addOffset)
    {
    int itemArea = size.x * size.y;

    if (tileByType[placementType].Count < itemArea)
        return null;

    int iteration = 0;

    while (iteration < iterationsMax)
    {
        iteration++;

        int index = UnityEngine.Random.Range(0, tileByType[placementType].Count);
        Vector2Int position = tileByType[placementType].ElementAt(index);

        if (itemArea > 1)
        {
            var (result, placementPositions) = PlaceBigItem(position, size, addOffset);

            if (!result)
                continue;

            tileByType[placementType].ExceptWith(placementPositions);
            tileByType[PlacementType.NearWall].ExceptWith(placementPositions);
        }
        else
        {
            tileByType[placementType].Remove(position);
        }

        return position;
    }

    return null; // Need to handle the case when no valid position is found after all iterations
}



    private (bool result, List<Vector2Int> placementPositions) PlaceBigItem(Vector2Int originPosition, Vector2Int size, bool addOfsset)
    {
        List<Vector2Int> positions = new List<Vector2Int>() {originPosition } ;
        int maxX = addOfsset ? size.x + 1 : size.x ; 
        int maxY = addOfsset ? size.y + 1 : size.y ;
        int minX = addOfsset ? -1 : 0 ; 
        int minY = addOfsset ? -1 : 0 ;  

        for(int row= minX ; row <= maxX ; row++){
            for(int col = minY ; col <= maxY ; col++){
                if( col == 0 && row == 0)
                    continue ; 
                Vector2Int newPosToCheck = 
                new Vector2Int(originPosition.x + row , originPosition.y + col) ; 
                if(roomFloorNoCorridor.Contains(newPosToCheck) == false )
                    return (false , positions) ; 
                positions.Add(newPosToCheck) ; 
            }
        }
        return (true , positions) ;
    }

    public List<Vector2?> GetMultipleItemPlacementPositions(PlacementType placementType, int itemCount, int iterationsMax, Vector2Int size, bool addOffset)
    {
        List<Vector2?> itemPositions = new List<Vector2?>();

        for (int i = 0; i < itemCount; i++)
        {
            Vector2? itemPosition = GetItemPlacementPosition(placementType, iterationsMax, size, addOffset);
            if (itemPosition != null)
            {
                itemPositions.Add(itemPosition.Value);
            }
            else
            {
                Debug.LogWarning("Failed to find a valid position for item placement.");
            }
        }

        return itemPositions;
    }

    
}


public enum PlacementType {
    OpenSpace , 
    NearWall
}