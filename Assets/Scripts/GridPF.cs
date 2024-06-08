using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPF : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public LayerMask enemyMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector2 worldBottomLeft = (Vector2)transform.position;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);

                // Debugging walkability
                if (walkable)
                {
                    // Debug.Log($"Node at ({x}, {y}) is walkable");
                }
                else
                {
                    // Debug.Log($"Node at ({x}, {y}) is unwalkable");
                }
            }
        }
    }

    public void UpdateGrid()
    {
        foreach (Node node in grid)
        {
            Vector3 worldPoint = node.worldPosition;
            node.walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask) || Physics.CheckSphere(worldPoint, nodeRadius, enemyMask));
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if ( x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = worldPosition.x / gridWorldSize.x;
        float percentY = worldPosition.y / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    void OnDrawGizmos()
    {
        Vector3 cubeCenter = transform.position + new Vector3(gridWorldSize.x / 2, gridWorldSize.y / 2, 0);
        Gizmos.DrawWireCube(cubeCenter, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                Gizmos.DrawCube((Vector3)n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }
}
