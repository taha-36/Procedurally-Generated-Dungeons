using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Build.Content;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.Animations;

public class DungeonsGeneration:MonoBehaviour
{
    public int width, height, minRoomSize, maxRoomSize;
    public float padding;
    int depth;
    List<Rect> roomSpaces;
    List<Room> rooms;
    public GameObject testObj;


    void Start()
    {
        //Initialize The Binary Space Partitioning Algorithm
        Rect initialRoom = new Rect(0,0,width, height);
        BSP bSP = new BSP();
        bSP.Split(initialRoom, minRoomSize, maxRoomSize);
        roomSpaces = bSP.roomSpaces;

        //Generate The Rooms Classes And GameObjects
        RoomsGeneration rg = transform.AddComponent<RoomsGeneration>();
        rg.minRoomSize = minRoomSize;
        rg.maxRoomSize = maxRoomSize;
        rg.testObj = testObj;
        rg.padding = padding;
        rg.roomSpaces = roomSpaces;
        rg.GenerateRooms();
        rooms = rg.rooms;

        //Initialize The Delaunay Triangulation Algorithm
        DelaunayTriangulation dTriangulation = new DelaunayTriangulation();
        dTriangulation.ComputeBowyerWatson(ListNodes(rooms));
        AssignEdgesToRooms(rooms, dTriangulation.triangles);

        //Finding The Minimum Spanning Tree Of the Delaunay Graph
        MST mST = new MST();
        mST.FindMST(rooms);

    }
    void AssignEdgesToRooms(List<Room> rooms, List<Triangle> triangles)
    {
        foreach (var room in rooms)
        {
            foreach(var triangle in triangles)
            {
                List<Edge> edgesOfTri = triangle.GetEdges();
                foreach (var edgeInTri in edgesOfTri)
                {
                    if(edgeInTri.p1 == room.position || edgeInTri.p2 == room.position)
                    {
                        room.edges.Add(edgeInTri);
                    }
                }
            }
        }
    }
    List<Vector2> ListNodes(List<Room> rooms)
    {
        List<Vector2> nodes = new List<Vector2>();

        foreach(var room in rooms)
        {
            nodes.Add(room.position);
        }
        return nodes;
    }
}