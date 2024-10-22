using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public LayerMask roomLayer;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    public void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        StartCoroutine(CreateGrid());
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if(grid != null)
        {
            foreach(Node n in grid)
            {
                Gizmos.color = (n.roomBlocked) ? Color.red : Color.white;
                Gizmos.DrawCube(n.position, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
    IEnumerator CreateGrid()
    {
        // Wait for 1 second or adjust the time to your needs
        yield return new WaitForSeconds(1f);
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + new Vector3(x * nodeDiameter + nodeRadius, 0, y * nodeDiameter + nodeRadius);
                bool roomBlocked = (Physics.CheckSphere(worldPoint, nodeRadius, roomLayer));
                grid[x, y] = new Node(worldPoint, roomBlocked);
            }
        }
    }
}
