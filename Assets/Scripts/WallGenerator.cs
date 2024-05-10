using System ; 
using System.Collections;
using System.Collections.Generic;
using UnityEditor ; 
using UnityEngine;

public static class WallGenerator 
{
   public static void CreateWalls(HashSet<Vector2Int> floorPositions , TileMapVisualizer tileMapVisualizer) 
   {
        var basicWallpositions = FindWallsInDirections(floorPositions , Direction2D.cardinalDirectionsList) ; 
        foreach(var position in basicWallpositions ){
            tileMapVisualizer.PaintSingleBasicWall(position) ; 
        }
   }

   private static  HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions , List<Vector2Int> directionList)
   {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>() ; 
        foreach(var position in floorPositions){
            foreach(var direction in directionList){
                var neighbourPosition = position + direction ; 
                if(floorPositions.Contains(neighbourPosition) == false ){
                    wallPositions.Add(neighbourPosition) ; 
                }
            }
        }
        return wallPositions ; 
   }
}
