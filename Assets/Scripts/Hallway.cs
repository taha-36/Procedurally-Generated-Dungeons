using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hallway
{
    float maxDistance = 30f;
    public Hallway()
    {

    }
    public void Hallways(List<GameObject> rooms)
    {
        List<GameObject> connectedRooms = new List<GameObject>();
        connectedRooms.Add(rooms[0]);
        rooms[0].GetComponent<Room>().isConnected = true;
        while (connectedRooms.Count < rooms.Count)
        {
            GameObject closestRoom = null;
            GameObject currentRoom = null;
            float closestDistance = Mathf.Infinity;
            foreach (var connectedRoom in connectedRooms)
            {
                currentRoom = connectedRoom;
                foreach (var room in rooms)
                {
                    if (!room.GetComponent<Room>().isConnected)
                    {
                        float distance = Vector3.Distance(connectedRoom.transform.position, room.transform.position);

                        if(distance < closestDistance && distance <= maxDistance)
                        {
                            closestDistance = distance;
                            closestRoom = room;
                        }
                    }
                }
            }
            if (closestRoom != null)
            {
                Debug.DrawLine(currentRoom.transform.position,closestRoom.transform.position, Color.cyan, 100f);
                connectedRooms.Add(closestRoom);
                closestRoom.GetComponent<Room>().isConnected = true;
            }
            else
            {
                Debug.LogWarning("No unconnected rooms found.");
                break;
            }
        }
    }
}
