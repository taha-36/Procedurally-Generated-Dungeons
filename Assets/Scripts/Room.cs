using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public bool isConnected;
    public Vector2 position;
    public GameObject go;
    public List<Edge> edges = new List<Edge>();
    public List<Door> doors = new List<Door>();
    public Room(Vector2 pos, GameObject go)
    {
        Door door1 = new Door();
        Door door2 = new Door();
        doors.Add(door1);
        doors.Add(door2);

        position = pos;
        this.go = go;
    }
}

public class Door
{
    public Vector3 doorPos = Vector3.zero;
    public Quaternion doorRot = Quaternion.identity;
    public bool connected;
}
