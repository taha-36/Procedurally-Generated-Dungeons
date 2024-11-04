using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MST
{
    public MST() { }
    public List<Room> FindMST(List<Room> nodes)
    {
        List<Room> visitedNodes = new List<Room>();
        List<Room> outPut = new List<Room>();
        visitedNodes.Add(nodes[0]);
        nodes[0].isConnected = true;
        while (visitedNodes.Count < nodes.Count)
        {
            List<Edge> checkingEdges = new List<Edge>();
            float leastWeight = Mathf.Infinity;
            Room closestRoom = null;
            Room currentRoom = null;
            foreach (var node in visitedNodes)
            {
                List<Edge> edgesOfNode = node.edges;
                foreach (var edge in edgesOfNode)
                {
                    if (!FindRoom(edge.OtherPoint(edge, node.position), nodes).isConnected)
                    {
                        checkingEdges.Add(edge);
                    }
                }
            }
            foreach (Edge edge in checkingEdges)
            {
                Room v1 = FindRoom(edge.p1, nodes);
                Room v2 = FindRoom(edge.p2, nodes);
                float distance = Vector2.Distance(edge.p1, edge.p2);

                if (distance < leastWeight)
                {
                    leastWeight = distance;
                    if (v1.isConnected)
                    {
                        closestRoom = v2;
                        currentRoom = v1;
                    }
                    else
                    {
                        closestRoom = v1;
                        currentRoom = v2;
                    }
                }
            }
            if (closestRoom != null)
            {
                visitedNodes.Add(closestRoom);
                outPut.Add(currentRoom);
                outPut.Add(closestRoom);
                closestRoom.isConnected = true;
            }
        }
        /*for (int i = 0; i < outPut.Count - 1; i += 2)
        {
            Debug.DrawLine(new Vector3(outPut[i].position.x, 0, outPut[i].position.y), new Vector3(outPut[i + 1].position.x, 0, outPut[i + 1].position.y), Color.red, 100f);
        }*/
        return outPut;
    }

    Room FindRoom(Vector2 point, List<Room> rooms)
    {
        Room foundRoom = null;
        foreach (var room in rooms)
        {
            if (room.position == point)
            {
                foundRoom = room;
            }
        }
        return foundRoom;
    }
}
