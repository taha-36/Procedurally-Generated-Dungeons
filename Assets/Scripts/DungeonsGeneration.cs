using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class DungeonsGeneration : MonoBehaviour
{
    public int width, height, minRoomSize, maxRoomSize;
    int depth;
    List<Rect> roomSpaces = new List<Rect>();
    List<Room> rooms = new List<Room>();
    List<Room> mstOut = new List<Room>();
    List<Node> allHallways = new List<Node>();
    public GameObject roomObj;
    public GameObject hallway1, hallway2, hallway3, hallway4, hallway5, block;
    public Transform aStar;
    PathFinding pf;
    Grid grid;
    RoomsGeneration rg;
    public Mesh wall, corner, doorWay, ground;
    public Material wallMat, groundCeilingMat, columnMat, doorWayMat;
    private void Start()
    {
        StartCoroutine(AllInit());
    }
    public void Initialize()
    {
        grid = aStar.GetComponent<Grid>();
        //Initialize The Binary Space Partitioning Algorithm
        Rect initialRoom = new Rect(0, 0, width, height);
        BSP bSP = new BSP();
        bSP.nodeDiameter = grid.nodeRadius * 2;
        bSP.Split(initialRoom, minRoomSize, maxRoomSize);
        roomSpaces = bSP.roomSpaces;

        //Generate The Rooms Classes And GameObjects
        rg = new RoomsGeneration();
        rg.wall = wall;
        rg.column = corner;
        rg.doorWay = doorWay;
        rg.ground = ground;
        rg.columnMat = columnMat;
        rg.wallMat = wallMat;
        rg.groundCeilingMat = groundCeilingMat;
        GenerateRooms();

        //Initialize The Delaunay Triangulation Algorithm
        DelaunayTriangulation dTriangulation = new DelaunayTriangulation();
        dTriangulation.ComputeBowyerWatson(ListNodes(rooms));
        AssignEdgesToRooms(rooms, dTriangulation.triangles);

        //Finding MinnimumSpanningTree
        MST mST = new MST();
        mstOut = mST.FindMST(rooms);
    }
    public void Grid()
    {
        grid.CreateGrid();
    }
    public void Hallways()
    {
        pf = aStar.GetComponent<PathFinding>();
        for (int i = 0; i < mstOut.Count; i += 2)
        {
            Room r1 = mstOut[i];
            Room r2 = mstOut[i + 1];

            var result = FindClosestSidesOfRooms(ref r1, ref r2);

            Node n1 = grid.NodeFromWorldPoint(result.Item1.doorPos - result.Item1.doorRot * Vector3.forward.normalized * 2);
            Node n2 = grid.NodeFromWorldPoint(result.Item2.doorPos - result.Item2.doorRot * Vector3.forward.normalized * 2);
            List<Node> path = pf.Findpath(n1.position, n2.position);

            for (int x = 0; x < path.Count; x++)
            {
                if(x != 0 && x != path.Count - 1)
                {
                    if (!grid.grid[path[x].gridX, path[x].gridY].containsHallway)
                    {
                        allHallways.Add(path[x]);
                        grid.grid[path[x].gridX, path[x].gridY].containsHallway = true;
                    }
                    int index = allHallways.IndexOf(path[x]);
                    Vector3 frsDir = (path[x + 1].position - path[x].position).normalized;
                    Vector3 scndDir = (path[x - 1].position - path[x].position).normalized;
                    if (!allHallways[index].supportedDir.Contains(frsDir))
                        allHallways[index].supportedDir.Add(frsDir);
                    if (!allHallways[index].supportedDir.Contains(scndDir))
                        allHallways[index].supportedDir.Add(scndDir);
                }
                else if(x == 0)
                {
                    if (!grid.grid[path[x].gridX, path[x].gridY].containsHallway)
                    {
                        allHallways.Add(path[x]);
                        grid.grid[path[x].gridX, path[x].gridY].containsHallway = true;
                    }
                    int index = allHallways.IndexOf(path[x]);
                    Vector3 frsDir = (result.Item1.doorPos - new Vector3(path[x].position.x, 0, path[x].position.z)).normalized;
                    Vector3 scndDir = (new Vector3(path[x + 1].position.x, 0, path[x + 1].position.z) - new Vector3(path[x].position.x, 0, path[x].position.z)).normalized;
                    if (!allHallways[index].supportedDir.Contains(frsDir))
                        allHallways[index].supportedDir.Add(frsDir);
                    if (!allHallways[index].supportedDir.Contains(scndDir))
                        allHallways[index].supportedDir.Add(scndDir);
                    allHallways[index].startNode = true;
                    allHallways[index].doorDir = (result.Item1.doorRot * Vector3.forward).normalized;
                }
                else
                {
                    if (!grid.grid[path[x].gridX, path[x].gridY].containsHallway)
                    {
                        allHallways.Add(path[x]);
                        grid.grid[path[x].gridX, path[x].gridY].containsHallway = true;
                    }
                    int index = allHallways.IndexOf(path[x]);
                    Vector3 frsDir = (result.Item2.doorPos - new Vector3(path[x].position.x, 0, path[x].position.z)).normalized;
                    Vector3 scndDir = (new Vector3(path[x - 1].position.x, 0, path[x - 1].position.z) - new Vector3(path[x].position.x, 0, path[x].position.z)).normalized;
                    if (!allHallways[index].supportedDir.Contains(frsDir))
                        allHallways[index].supportedDir.Add(frsDir);
                    if (!allHallways[index].supportedDir.Contains(scndDir))
                        allHallways[index].supportedDir.Add(scndDir);
                    allHallways[index].endNode = true;
                    allHallways[index].doorDir = (result.Item2.doorRot * Vector3.forward).normalized;
                }
            }
        }
        foreach (Node n in allHallways)
        {
            AllignHallway(n);
        }
        foreach (Room r in rooms)
        {
            foreach(Door d in r.doors)
            {
                if(!d.connected)
                {
                    Instantiate(block, d.doorPos, d.doorRot);
                }
            }
        }
    }
    void AssignEdgesToRooms(List<Room> rooms, List<Triangle> triangles)
    {
        foreach (var room in rooms)
        {
            foreach (var triangle in triangles)
            {
                List<Edge> edgesOfTri = triangle.GetEdges();
                foreach (var edgeInTri in edgesOfTri)
                {
                    if (edgeInTri.p1 == room.position || edgeInTri.p2 == room.position)
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

        foreach (var room in rooms)
        {
            nodes.Add(room.position);
        }
        return nodes;
    }
    public void GenerateRooms()
    {
        float nodeDiameter = grid.nodeRadius * 2;
        foreach (var roomSpace in roomSpaces)
        {
            bool generateRoom = Random.value > 0.5f;
            if (generateRoom)
            {
                int scaleX = (int)(roomSpace.width - 2 * nodeDiameter);
                int scaleY = (int)(roomSpace.height - 2 * nodeDiameter);
                GameObject io = Instantiate(roomObj, new Vector3(roomSpace.x + nodeDiameter, 1, roomSpace.y + nodeDiameter), Quaternion.identity);
                io.layer = LayerMask.NameToLayer("Room");
                Room room = new Room(new Vector2(io.transform.position.x, io.transform.position.z), io);
                rooms.Add(room);
                rg.MatrixiateRoom(io.transform.position, new Vector2Int(scaleX / 4, scaleY / 4), room);
            }
        }
    }
    (Door, Door) FindClosestSidesOfRooms(ref Room r1, ref Room r2)
    {
        float closestDistance = Mathf.Infinity;
        Door cS1 = null;
        Door cS2 = null;
        for (int i = 0; i < 2; i++)
        {
            for (int z = 0; z < 2; z++)
            {
                float dist = Vector3.Distance(r1.doors[i].doorPos, r2.doors[z].doorPos);

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    cS1 = r1.doors[i];
                    cS2 = r2.doors[z];
                }
            }
        }
        cS1.connected = true;
        cS2.connected = true;
        return (cS1, cS2);
    }

    private void Update()
    {
        Graphics.DrawMeshInstanced(wall, 0, wallMat, rg.wallMatrices);
        Graphics.DrawMeshInstanced(doorWay, 0, wallMat, rg.doorMatrices);
        Graphics.DrawMeshInstanced(doorWay, 1, doorWayMat, rg.doorMatrices);
        Graphics.DrawMeshInstanced(ground, 0, groundCeilingMat, rg.groundCeilingMatrices);
        Graphics.DrawMeshInstanced(corner, 0, columnMat, rg.columnMatrices);
    }

    IEnumerator AllInit()
    {
        Initialize();
        yield return new WaitForSeconds(0.1f);
        Grid();
        Hallways();
    }
    void AllignHallway(Node node)
    {
        if(node.supportedDir.Count == 2)
        {
            if (Vector3.Cross(node.supportedDir[0], node.supportedDir[1]) == Vector3.zero)
            {
                if (node.startNode || node.endNode)
                {
                    Quaternion rot = Quaternion.LookRotation(node.doorDir);
                    Instantiate(hallway2, new Vector3(node.position.x, 0, node.position.z), rot);
                }
                else
                {
                    Quaternion rot = Quaternion.LookRotation(node.supportedDir[0]);
                    Instantiate(hallway1, new Vector3(node.position.x, 0, node.position.z), rot);
                }
            }
            else
            {
                Quaternion rotOption1 = Quaternion.LookRotation(node.supportedDir[0], Vector3.up);
                Quaternion rotOption2 = Quaternion.LookRotation(node.supportedDir[1], Vector3.up);
                Transform dir = hallway3.transform;
                dir.rotation = rotOption1;
                if(Vector3.Dot(dir.right, node.supportedDir[1]) > 0)
                {
                    Instantiate(hallway3, new Vector3(node.position.x, 0, node.position.z), rotOption1);
                }
                else
                {
                    Instantiate(hallway3, new Vector3(node.position.x, 0, node.position.z), rotOption2);
                }
            }
        }
        else if(node.supportedDir.Count == 3)
        {
            if (Vector3.Dot(node.supportedDir[0], node.supportedDir[1]) == 0 && Vector3.Dot(node.supportedDir[0], node.supportedDir[2]) == 0)
            {
                Quaternion rot = Quaternion.LookRotation(node.supportedDir[0], Vector3.up);
                Instantiate(hallway4, new Vector3(node.position.x, 0, node.position.z), rot);
            }
            else if(Vector3.Dot(node.supportedDir[1], node.supportedDir[0]) == 0 && Vector3.Dot(node.supportedDir[1], node.supportedDir[2]) == 0)
            {
                Quaternion rot = Quaternion.LookRotation(node.supportedDir[1], Vector3.up);
                Instantiate(hallway4, new Vector3(node.position.x, 0, node.position.z), rot);
            }
            else
            {
                Quaternion rot = Quaternion.LookRotation(node.supportedDir[2], Vector3.up);
                Instantiate(hallway4, new Vector3(node.position.x, 0, node.position.z), rot);
            }
        }
        else
        {
            Instantiate(hallway5, new Vector3(node.position.x, 0, node.position.z), Quaternion.identity);
        }
    }
}