using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Grid grid;
    private void Start()
    {
    }
    public List<Node> Findpath(Vector3 startPos, Vector3 endPos)
    {
        grid = GetComponent<Grid>();
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node endNode = grid.NodeFromWorldPoint(endPos);
        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        List<Node> path = new List<Node>();
        openSet.Add(startNode);
        while(openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for(int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost  && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == endNode)
            {
                path = RetracePath(startNode, endNode);
                return path;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                if(neighbour.roomBlocked || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + getDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = getDistance(neighbour, endNode);
                    neighbour.parent = currentNode;

                    if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        return path;
    }
    List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            if (currentNode.parent != null)
            {
                if (currentNode.gridX != currentNode.parent.gridX && currentNode.gridY != currentNode.parent.gridY)
                {
                    Node p1 = grid.grid[currentNode.gridX, currentNode.parent.gridY];
                    Node p2 = grid.grid[currentNode.parent.gridX, currentNode.gridY];

                    if(!p1.roomBlocked)
                    {
                        path.Add(p1);
                    }
                    else
                    {
                        path.Add(p2);
                    }
                }
            }
            currentNode = currentNode.parent;
        }
        path.Add(startNode);
        path.Reverse();
        for(int i = 0; i < path.Count; i++)
        {
            if(i != path.Count - 1)
                path[i].lookDir = (path[i + 1].position - path[i].position).normalized;
        }

        grid.path = path;
        return path;
    }

    int getDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }
}
