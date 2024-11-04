using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnalyzer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AnalyzeMesh(transform.GetComponent<MeshFilter>().mesh);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AnalyzeMesh(Mesh mesh)
    {
        Vector3[] vert = mesh.vertices;
        int[] tri = mesh.triangles;
        List<Face> faces = new List<Face>();
        for(int i = 0; i < tri.Length; i += 3)
        {
            Vector3[] verts =
            {
                new Vector3(vert[tri[i]].x,vert[tri[i]].y,vert[tri[i]].z) + transform.position,
                new Vector3(vert[tri[i + 1]].x,vert[tri[i + 1]].y,vert[tri[i + 1]].z) + transform.position,
                new Vector3(vert[tri[i + 2]].x,vert[tri[i + 2]].y,vert[tri[i + 2]].z) + transform.position,
            };
            Face f = new Face(verts);
            faces.Add(f);
        }
        foreach(Face f in faces)
        {
            if(Vector3.Angle(f.GetNormalToFace(), Vector3.up) < 45)
            {
                Debug.DrawRay(f.GetFaceCenter(), f.GetNormalToFace(), Color.red, 100f);
            }
        }
    }
}

public class Face
{
    public Vector3[] verts = {
    new Vector3(),
    new Vector3(),
    new Vector3(),
    };

    public Face(Vector3[] verts)
    {
        this.verts = verts;
    }

    public Vector3 GetNormalToFace()
    {
        Vector3 v1 = verts[1] - verts[0];
        Vector3 v2 = verts[2] - verts[1];

        Vector3 n = Vector3.Cross(v1, v2).normalized;

        return n;
    }
    public Vector3 GetFaceCenter()
    {
        Vector3 centeroid = (verts[0] + verts[1] + verts[2]) / 3;

        return centeroid;
    }
}
