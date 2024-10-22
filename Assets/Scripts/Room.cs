using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public bool isConnected;
    public Vector2 position;
    public GameObject go;
    public List<Edge> edges = new List<Edge>();
    public Room(Vector2 pos, GameObject go)
    {
        position = pos;
        this.go = go;
    }
}
