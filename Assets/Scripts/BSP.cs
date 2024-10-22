using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSP
{
    public List<Rect> roomSpaces = new List<Rect>();
    public void Split(Rect roomSpace, int minRoomSize, int maxRoomSize)
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
                Split(leftroom, minRoomSize, maxRoomSize);
                Split(rightroom, minRoomSize, maxRoomSize);
            }
            else if (roomSpace.height > maxRoomSize)
            {
                // Split vertically
                int splitY = Random.Range(minRoomSize, (int)roomSpace.height - minRoomSize);
                Rect toproom = new Rect(roomSpace.x, roomSpace.y, roomSpace.width, splitY);
                Rect bottomroom = new Rect(roomSpace.x, roomSpace.y + splitY, roomSpace.width, roomSpace.height - splitY);
                Split(toproom, minRoomSize, maxRoomSize);
                Split(bottomroom, minRoomSize, maxRoomSize);
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
    public void DrawRoomSpace()
    {
        foreach (var roomSpace in roomSpaces)
        {
            Debug.DrawLine(new Vector3(roomSpace.x, 0, roomSpace.y), new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y), Color.blue, 100f);
            Debug.DrawLine(new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y), new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y + roomSpace.height), Color.blue, 100f);
            Debug.DrawLine(new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y + roomSpace.height), new Vector3(roomSpace.x, 0, roomSpace.y + roomSpace.height), Color.blue, 100f);
            Debug.DrawLine(new Vector3(roomSpace.x, 0, roomSpace.y + roomSpace.height), new Vector3(roomSpace.x, 0, roomSpace.y), Color.blue, 100f);
        }
    }
}
