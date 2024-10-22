using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool roomBlocked;
    public Vector3 position;

    public Node(Vector3 position, bool roomBlocked)
    {
        this.position = position;
        this.roomBlocked = roomBlocked;
    }
}
