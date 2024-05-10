using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random ; 
public static class ProceduralGenerationAlgorithmes
{
    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition , int walkLength )
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>() ; 

        path.Add(startPosition) ; 
        var previousPosition = startPosition ; 
        for(int i=0 ; i<walkLength ; i++){
            var newPosition = previousPosition + Direction2D.GetRandomCardinalDirection() ; 
            path.Add(newPosition) ; 
            previousPosition = newPosition ; 
        }
        return path ; 
    }

    public static List<Vector2Int> RandomWalkCorridor(Vector2Int startPosition , int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>() ; 
        var direction = Direction2D.GetRandomCardinalDirection() ;  
        var currentPosition = startPosition ; 
        corridor.Add(currentPosition) ; 
        for(int i=0 ; i<corridorLength ; i++){
            currentPosition += direction ; 
            corridor.Add(currentPosition) ; 
        }
        return corridor ; 

    }



    public static List<BoundsInt> BinarySpacePartitioning(BoundsInt spaceToSplit , int minWidth , int minHeight)
    {
        Queue<BoundsInt> roomQueue = new Queue<BoundsInt>() ; 
        
        List<BoundsInt> roomsList = new List<BoundsInt>() ; 

        roomQueue.Enqueue(spaceToSplit) ; 
        while(roomQueue.Count > 0 ){
            var room = roomQueue.Dequeue() ; 
            if(room.size.y >= minHeight && room.size.x >= minWidth)
            {
                if(UnityEngine.Random.value < 0.5f)
                {
                    if(room.size.y >= minHeight * 2)
                    {
                        SplitHorizantally( minHeight , roomQueue , room) ; 
                    }
                    else if(room.size.x >= minWidth * 2 )
                    {
                        SplitVertically(minWidth , roomQueue , room) ; 
                    }
                    else if(room.size.y >= minHeight && room.size.x >= minWidth) {
                        roomsList.Add(room) ; 
                    }
                }
                else 
                {
                     if(room.size.y >= minWidth * 2)
                    {
                        SplitVertically(minWidth  , roomQueue , room) ; 
                    }
                    else if(room.size.x >= minHeight * 2 )
                    {
                        SplitHorizantally( minHeight , roomQueue , room) ; 
                    }
                    else if(room.size.y >= minHeight && room.size.x >= minWidth) {
                        roomsList.Add(room) ; 
                    }
                }
            }
        }
        return roomsList ; 
    }

    private static void SplitVertically(int minWidth, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        var SplitX = Random.Range(1,room.size.x) ; 
        BoundsInt room1 = new BoundsInt(room.min ,new Vector3Int(SplitX , room.size.y , room.size.z) ) ; 
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x + SplitX , room.min.y , room.min.z) 
        , new Vector3Int(room.size.x - SplitX , room.size.y , room.size.z)) ;
        roomQueue.Enqueue(room1) ; 
        roomQueue.Enqueue(room2) ;  
    }

    private static void SplitHorizantally(int minHeight, Queue<BoundsInt> roomQueue, BoundsInt room)
    {
        var SplitY = Random.Range(1,room.size.y) ; 
        BoundsInt room1 = new BoundsInt(room.min, new Vector3Int(room.size.x , SplitY , room.size.z)) ; 
        BoundsInt room2 = new BoundsInt(new Vector3Int(room.min.x , room.min.y + SplitY , room.min.z) 
        , new Vector3Int(room.size.x , room.size.y - SplitY , room.size.z)) ; 
        roomQueue.Enqueue(room1) ; 
        roomQueue.Enqueue(room2) ;  
    }
public static class Direction2D{
    public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
    {
        new Vector2Int(0,1) , //UP 
        new Vector2Int(1,0) , //RIGHT
        new Vector2Int(-1,0) , //LEFT 
        new Vector2Int(0,-1) //DOWN
    } ; 
    public static  Vector2Int GetRandomCardinalDirection(){
        return cardinalDirectionsList[Random.Range(0 , cardinalDirectionsList.Count)] ; 
        }
    }
}