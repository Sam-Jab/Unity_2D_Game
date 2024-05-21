using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TileMapVisualizer tileMapVisualizer = null ; 
    [SerializeField]
    protected Vector2Int startPosition = Vector2Int.zero ; 
    public void GenerateDungeon(){
        tileMapVisualizer.Clear() ; 
        // foreach(GameObject o in RoomFirstDungeonGenerator.instantiatedItems){
        //     Destroy(o);
        // }
        // RoomFirstDungeonGenerator.instantiatedItems.Clear();

        
        RunProceduralGeneration() ; 
    }

    protected  abstract void RunProceduralGeneration() ; 
}
