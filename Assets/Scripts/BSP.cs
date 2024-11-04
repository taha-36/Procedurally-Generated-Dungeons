using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class BSP
{
    
    public List<Rect> roomSpaces = new List<Rect>();
    public float nodeDiameter;
    public void Split(Rect roomSpace, int minRoomSize, int maxRoomSize)
    {
        if (roomSpace.width > maxRoomSize * nodeDiameter || roomSpace.height > maxRoomSize * nodeDiameter)
        {
            bool splitHorizontally = Random.value > 0.5f;
            if (splitHorizontally && roomSpace.width > maxRoomSize * nodeDiameter)
            {
                // Split horizontally
                float splitX = Random.Range(minRoomSize * nodeDiameter, (int)roomSpace.width - minRoomSize * nodeDiameter);
                int roundTemp = Mathf.RoundToInt(splitX / nodeDiameter);
                float roundedUpSplitX = roundTemp * nodeDiameter;
                Rect leftroom = new Rect(roomSpace.x, roomSpace.y, roundedUpSplitX, roomSpace.height);
                Rect rightroom = new Rect(roomSpace.x + roundedUpSplitX, roomSpace.y, roomSpace.width - roundedUpSplitX, roomSpace.height);
                Split(leftroom, minRoomSize, maxRoomSize);
                Split(rightroom, minRoomSize, maxRoomSize);
            }
            else if (roomSpace.height > maxRoomSize * nodeDiameter)
            {
                // Split vertically
                float splitY = Random.Range(minRoomSize * nodeDiameter, (int)roomSpace.height - minRoomSize * nodeDiameter);
                int roundTemp = Mathf.RoundToInt(splitY / nodeDiameter);
                float roundedUpSplitY = roundTemp * nodeDiameter;
                Rect toproom = new Rect(roomSpace.x, roomSpace.y, roomSpace.width, roundedUpSplitY);
                Rect bottomroom = new Rect(roomSpace.x, roomSpace.y + roundedUpSplitY, roomSpace.width, roomSpace.height - roundedUpSplitY);
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

    void Draw()
    {
        foreach (var roomSpace in roomSpaces)
        {
            Debug.DrawLine(new Vector3(roomSpace.x, 0, roomSpace.y), new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y), Color.blue, Mathf.Infinity);
            Debug.DrawLine(new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y), new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y + roomSpace.height), Color.blue, Mathf.Infinity);
            Debug.DrawLine(new Vector3(roomSpace.x + roomSpace.width, 0, roomSpace.y + roomSpace.height), new Vector3(roomSpace.x, 0, roomSpace.y + roomSpace.height), Color.blue, Mathf.Infinity);
            Debug.DrawLine(new Vector3(roomSpace.x, 0, roomSpace.y + roomSpace.height), new Vector3(roomSpace.x, 0, roomSpace.y), Color.blue, Mathf.Infinity);
        }
    }
}
