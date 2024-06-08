using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Pathfinding : MonoBehaviour
{
    PathRequestManager requestManager;
    public GridPF grid;
    bool isProcessingPath;

    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<GridPF>();
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        isProcessingPath = true;
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + Mathf.RoundToInt(neighbour.weight * 20); // Include weight here
                    // Debug.Log($"Current Node: ({currentNode.gridX}, {currentNode.gridY}), Neighbour: ({neighbour.gridX}, {neighbour.gridY}), Weight: {neighbour.weight}, New Cost: {newMovementCostToNeighbour}");

                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        // Debug.Log($"Updating Neighbour: ({neighbour.gridX}, {neighbour.gridY}), gCost: {neighbour.gCost}, hCost: {neighbour.hCost}, fCost: {neighbour.fCost}");

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        isProcessingPath = false;
    }

    public bool IsProcessingPath()
    {
        return isProcessingPath;
    }

    public Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        // Debug.Log("Waypoints: " + string.Join(", ", waypoints.Select(wp => wp.ToString()).ToArray()));
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
