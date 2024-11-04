using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomsGeneration
{
    public Mesh wall, column, doorWay, ground;
    public Material wallMat, groundCeilingMat, columnMat;
    Mesh mesh = new Mesh();
    public List<Matrix4x4> wallMatrices = new List<Matrix4x4>();
    public List<Matrix4x4> columnMatrices = new List<Matrix4x4>();
    public List<Matrix4x4> doorMatrices = new List<Matrix4x4>();
    public List<Matrix4x4> groundCeilingMatrices = new List<Matrix4x4>();

    int wallInstance = 0;
    int columnInstance = 0;
    int doorInstance = 0;
    int groundCeilingInstance = 0;


    public void MatrixiateRoom(Vector3 position, Vector2Int dimensions, Room room)
    {

        //Creating The Indices Of Doorways
        int xDoorIndex = Random.Range(0, dimensions.x);
        int yDoorIndex = Random.Range(0, dimensions.y);
        int r = Random.Range(0, 2);
        int r1 = Random.Range(0, 2);

        //Creating Horizontal Walls
        for (int x = 0; x < dimensions.x; x++)
        {
            Vector3 lowerLine = new Vector3(4 * x + 2 + position.x, 0, position.z);
            Vector3 upperLine = new Vector3(4 * x + 2 + position.x, 0, dimensions.y * 4 + position.z);
            if (x == xDoorIndex && r == 0)
            {
                Matrix4x4 uMatrix1 = Matrix4x4.TRS(upperLine, Quaternion.identity * Quaternion.AngleAxis(180, Vector3.up), Vector3.one);
                wallMatrices.Add(uMatrix1);
            }
            else if (x == xDoorIndex && r == 1)
            {
                Matrix4x4 lMatrix1 = Matrix4x4.TRS(lowerLine, Quaternion.identity, Vector3.one);
                wallMatrices.Add(lMatrix1);
            }
            else
            {
                Matrix4x4 lMatrix = Matrix4x4.TRS(lowerLine, Quaternion.identity, Vector3.one);
                Matrix4x4 uMatrix = Matrix4x4.TRS(upperLine, Quaternion.identity * Quaternion.AngleAxis(180, Vector3.up), Vector3.one);
                wallMatrices.Add(lMatrix);
                wallMatrices.Add(uMatrix);
            }
        }

        //Creating vertical Walls
        for (int y = 0; y < dimensions.y; y++)
        {
            Vector3 leftLine = new Vector3(position.x, 0, y * 4 + 2 + position.z);
            Vector3 rightLine = new Vector3(dimensions.x * 4 + position.x, 0, y * 4 + 2 + position.z);
            if (y == yDoorIndex && r1 == 0)
            {
                Matrix4x4 rMatrix = Matrix4x4.TRS(rightLine, Quaternion.identity * Quaternion.AngleAxis(-90, Vector3.up), Vector3.one);
                wallMatrices.Add(rMatrix);
            }
            else if (y == yDoorIndex && r1 == 1)
            {
                Matrix4x4 lMatrix = Matrix4x4.TRS(leftLine, Quaternion.identity * Quaternion.AngleAxis(90, Vector3.up), Vector3.one);
                wallMatrices.Add(lMatrix);
            }
            else
            {
                Matrix4x4 lMatrix = Matrix4x4.TRS(leftLine, Quaternion.identity * Quaternion.AngleAxis(90, Vector3.up), Vector3.one);
                Matrix4x4 rMatrix = Matrix4x4.TRS(rightLine, Quaternion.identity * Quaternion.AngleAxis(-90, Vector3.up), Vector3.one);
                wallMatrices.Add(lMatrix);
                wallMatrices.Add(rMatrix);
            }
        }

        //Creating Columns
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                Vector3 pos = new Vector3(x * dimensions.x * 4 + 0.2f - 0.4f * x + position.x, 0, y * dimensions.y * 4 + 0.2f - 0.4f * y + position.z);
                Matrix4x4 mat = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one);
                columnMatrices.Add(mat);
            }
        }

        //Creating Doorways
        Vector3 hDoorPos = new Vector3(xDoorIndex * 4 + 2 + position.x, 0, r * dimensions.y * 4 + position.z);
        Vector3 vDoorPos = new Vector3(dimensions.x * 4 * r1 + position.x, 0, yDoorIndex * 4 + 2 + position.z);
        room.doors[0].doorPos = hDoorPos;
        room.doors[1].doorPos = vDoorPos;
        room.doors[0].doorRot = Quaternion.identity * Quaternion.AngleAxis(180 * r, Vector3.up);
        room.doors[1].doorRot = Quaternion.identity * Quaternion.AngleAxis(180 * r1, Vector3.up) * Quaternion.AngleAxis(90, Vector3.up);
        Matrix4x4 hDoorMat = Matrix4x4.TRS(hDoorPos, Quaternion.identity * Quaternion.AngleAxis(180 * r, Vector3.up), Vector3.one);
        Matrix4x4 vDoorMat = Matrix4x4.TRS(vDoorPos, Quaternion.identity * Quaternion.AngleAxis(180 * r1, Vector3.up) * Quaternion.AngleAxis(90, Vector3.up), Vector3.one);

        doorMatrices.Add(hDoorMat);
        doorMatrices.Add(vDoorMat);


        //Creating Ground
        for (int y = 0; y < dimensions.y; y++)
        {
            for (int x = 0; x < dimensions.x; x++)
            {
                Vector3 groundPos = new Vector3(x * 4 + 2 + position.x, 0, y * 4 + 2 + position.z);
                Vector3 ceilingPos = new Vector3(x * 4 + 2 + position.x, wall.bounds.size.y, y * 4 + 2 + position.z);

                Matrix4x4 ground = Matrix4x4.TRS(groundPos, Quaternion.identity, Vector3.one);
                Matrix4x4 ceiling = Matrix4x4.TRS(ceilingPos, Quaternion.identity * Quaternion.AngleAxis(180, Vector3.forward), Vector3.one);

                groundCeilingMatrices.Add(ground);
                groundCeilingMatrices.Add(ceiling);

            }
        }

        //The Combine Buffer
        CombineInstance[] combineBuffer;

        //Creating Wall Colliders
        while (wallInstance < wallMatrices.Count)
        {
            int count = Mathf.Min((int)(65353 / wall.vertexCount), wallMatrices.Count - wallInstance);
            combineBuffer = new CombineInstance[count];
            for (int i = 0; i < count; i++)
            {
                combineBuffer[i].mesh = wall;
                Vector3 meshPos = (Vector3)wallMatrices[wallInstance].GetColumn(3) - room.go.transform.position;
                Quaternion meshRot = Quaternion.LookRotation(wallMatrices[wallInstance].GetColumn(2), wallMatrices[wallInstance].GetColumn(1));
                combineBuffer[i].transform = Matrix4x4.TRS(meshPos, meshRot, Vector3.one);
                wallInstance++;
            }
            mesh.CombineMeshes(combineBuffer, true, true);
            room.go.AddComponent<MeshCollider>().sharedMesh = mesh;
        }

        //Creating Door Colliders
        combineBuffer = new CombineInstance[2];
        for (int i = 0; i < combineBuffer.Length; i++)
        {
            combineBuffer[i].mesh = doorWay;
            Vector3 meshPos = (Vector3)doorMatrices[doorInstance].GetColumn(3) - room.go.transform.position;
            Quaternion meshRot = Quaternion.LookRotation(doorMatrices[doorInstance].GetColumn(2), doorMatrices[doorInstance].GetColumn(1));
            combineBuffer[i].transform = Matrix4x4.TRS(meshPos, meshRot, Vector3.one);
            doorInstance++;
        }
        mesh.CombineMeshes(combineBuffer, true, true);
        room.go.AddComponent<MeshCollider>().sharedMesh = mesh;

        //Creating Ground Colliders
        combineBuffer = new CombineInstance[groundCeilingMatrices.Count - groundCeilingInstance];
        for (int i = 0; i < combineBuffer.Length; i++)
        {
            combineBuffer[i].mesh = ground;
            Vector3 meshPos = (Vector3)groundCeilingMatrices[groundCeilingInstance].GetColumn(3) - room.go.transform.position;
            Quaternion meshRot = Quaternion.LookRotation(groundCeilingMatrices[groundCeilingInstance].GetColumn(2), groundCeilingMatrices[groundCeilingInstance].GetColumn(1));
            combineBuffer[i].transform = Matrix4x4.TRS(meshPos, meshRot, Vector3.one);
            groundCeilingInstance++;
        }
        mesh.CombineMeshes(combineBuffer, true, true);
        room.go.AddComponent<MeshCollider>().sharedMesh = mesh;


        //Creating Columns Colliders
        combineBuffer = new CombineInstance[4];
        for(int i = 0; i < combineBuffer.Length; i++)
        {
            combineBuffer[i].mesh = column;
            Vector3 meshPos = (Vector3)columnMatrices[columnInstance].GetColumn(3) - room.go.transform.position;
            Quaternion meshRot = Quaternion.LookRotation(columnMatrices[columnInstance].GetColumn(2), columnMatrices[columnInstance].GetColumn(1));
            combineBuffer[i].transform = Matrix4x4.TRS(meshPos, meshRot, Vector3.one);
            columnInstance++;
        }
        mesh.CombineMeshes(combineBuffer, true, true);
        room.go.AddComponent<MeshCollider>().sharedMesh = mesh;
    }
}

