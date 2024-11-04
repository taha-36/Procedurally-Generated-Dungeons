using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool roomBlocked, containsHallway, endNode, startNode;
    public List<Vector3> supportedDir;
    public Vector3 position;
    public Vector3 lookDir;
    public Vector3 doorDir;
    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    public Node parent;

    public Node(Vector3 position, bool roomBlocked, int gridX, int gridY)
    {
        this.position = position;
        this.roomBlocked = roomBlocked;
        this.gridX = gridX;
        this.gridY = gridY;
        supportedDir = new List<Vector3>();
    }

    public int fCost
    {
        get {
            return gCost + hCost;
        }
    }
}
