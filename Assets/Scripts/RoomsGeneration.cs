using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomsGeneration:MonoBehaviour
{
    public List<Rect> roomSpaces = new List<Rect>();
    public List<Room> rooms = new List<Room>();
    [HideInInspector]
    public GameObject testObj;
    [HideInInspector]
    public int minRoomSize, maxRoomSize;
    [HideInInspector]
    public float padding;
    public void GenerateRooms()
    {
        foreach (var roomSpace in roomSpaces)
        {
            bool splitHorizontally = Random.value > 0.5f;
            if (splitHorizontally)
            {
                GameObject io = Instantiate(testObj, new Vector3(roomSpace.x + roomSpace.width / 2, 0, roomSpace.y + roomSpace.height / 2), Quaternion.identity);
                io.transform.localScale = new Vector3(Random.Range(minRoomSize, roomSpace.width - padding / 2), 10, Random.Range(minRoomSize, roomSpace.height - padding / 2));
                io.layer = LayerMask.NameToLayer("Room");
                Room room = new Room(new Vector2(io.transform.position.x, io.transform.position.z), io);
                rooms.Add(room);
            }
        }
    }
}
