using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;
using UnityEngine.Animations;

public class BSP:MonoBehaviour
{
    public int width, height, minRoomSize, maxRoomSize;
    public float padding;
    int depth;
    List<Rect> roomSpaces;
    List<GameObject> rooms;
    public GameObject testObj;

    public void Split(Rect roomSpace)
    {
        if (roomSpace.width > maxRoomSize || roomSpace.height > maxRoomSize)
        {
            bool splitHorizontally = Random.value > 0.5f;
            if (splitHorizontally && roomSpace.width > maxRoomSize)
            {
                // Split horizontally
                int splitX = Random.Range(minRoomSize, (int)roomSpace.width - minRoomSize);
                Rect leftroom = new Rect(roomSpace.x, roomSpace.y, splitX, roomSpace.height);
                Rect rightroom = new Rect(roomSpace.x + splitX, roomSpace.y, roomSpace.width - splitX, roomSpace.height);
                Split(leftroom);
                Split(rightroom);
            }
            else if (roomSpace.height > maxRoomSize)
            {
                // Split vertically
                int splitY = Random.Range(minRoomSize, (int)roomSpace.height - minRoomSize);
                Rect toproom = new Rect(roomSpace.x, roomSpace.y, roomSpace.width, splitY);
                Rect bottomroom = new Rect(roomSpace.x, roomSpace.y + splitY, roomSpace.width, roomSpace.height - splitY);
                Split(toproom);
                Split(bottomroom);
            }
            else
            {
                roomSpaces.Add(roomSpace);
            }
        }
        else
        {
            roomSpaces.Add(roomSpace);
        }
    }

    void Start()
    {
        roomSpaces = new List<Rect>();
        rooms = new List<GameObject>();
        Rect initialRoom = new Rect(0,0,width, height);
        Split(initialRoom);
        Draw();
        GenerateRooms();
        Hallway buildHallway = new Hallway();
        buildHallway.Hallways(rooms);
    }

    Vector3 CalculateCenter(Vector3 edge, int width, int height)
    {
        Vector3 center = new Vector3(edge.x + width / 2, edge.y, edge.z + height / 2);
        return center;
    }
    void Draw()
    {
        foreach(var roomSpace in roomSpaces)
        {
            Debug.DrawLine(new Vector3(roomSpace.x,0,roomSpace.y), new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y), Color.blue, 100f);
            Debug.DrawLine(new Vector3(roomSpace.x + roomSpace.width,0,roomSpace.y), new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y + roomSpace.height), Color.blue, 100f);
            Debug.DrawLine(new Vector3(roomSpace.x + roomSpace.width,0,roomSpace.y + roomSpace.height), new Vector3(roomSpace.x, 0, roomSpace.y + roomSpace.height), Color.blue, 100f);
            Debug.DrawLine(new Vector3(roomSpace.x,0,roomSpace.y + roomSpace.height), new Vector3(roomSpace.x, 0, roomSpace.y), Color.blue, 100f);
        }
    }
    void GenerateRooms()
    {
        foreach(var roomSpace in roomSpaces)
        {
            bool splitHorizontally = Random.value > 0.5f;
            if(splitHorizontally)
            {
                GameObject io = Instantiate(testObj, new Vector3(roomSpace.x + roomSpace.width / 2,0,roomSpace.y + roomSpace.height / 2), Quaternion.identity);
                io.transform.localScale = new Vector3(Random.Range(minRoomSize, roomSpace.width - padding / 2), 10, Random.Range(minRoomSize, roomSpace.height - padding / 2));
                io.transform.AddComponent<Room>();
                rooms.Add(io);
            }
        }
    }
}
public class Room:MonoBehaviour{

    public bool isConnected;
}