using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Transform player;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask roomLayer;
    [HideInInspector]
    public Node[,] grid;
    public List<Node> path;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + new Vector3(x * nodeDiameter + nodeRadius, 0, y * nodeDiameter + nodeRadius);
                bool roomBlocked = (Physics.CheckSphere(worldPoint, nodeRadius, roomLayer));
                grid[x, y] = new Node(worldPoint, roomBlocked, x, y);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid != null)
        {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach(Node n in grid)
            {
                Gizmos.color = (n.roomBlocked) ? Color.red : Color.white;
                if (n == playerNode)
                {
                    Gizmos.color = Color.cyan;
                }
                if(path != null)
                {
                    if(path.Contains(n))
                    {
                        Gizmos.color = Color.green;
                    }
                }
                Gizmos.DrawCube(n.position, new Vector3(1, 0.1f, 1) * (nodeDiameter - 0.1f));
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }
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
    public Node NodeFromWorldPoint(Vector3 position)
    {
        float percentX = (position.x + gridWorldSize.x / 2 - transform.position.x) / gridWorldSize.x;
        float percenty = (position.z + gridWorldSize.y / 2 - transform.position.z) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percenty = Mathf.Clamp01(percenty);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percenty);

        return grid[x, y];
    }
}
