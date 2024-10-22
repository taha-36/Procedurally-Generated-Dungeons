using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;

public class DelaunayTriangulation
{
    public List<Triangle> triangles = new List<Triangle>();

    public void ComputeBowyerWatson(List<Vector2> points)
    {
        // Step 1: Create a super triangle that contains all the points
        Triangle superTriangle = CreateSuperTriangle(points);
        triangles.Add(superTriangle);

        // Step 2: Add points one by one to the triangulation
        foreach (var point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();
            List<Edge> polygon = new List<Edge>();

            // Step 3: Find all triangles whose circumcircle contains the point
            foreach (var triangle in triangles)
            {
                if (IsPointInCircumcircle(point, triangle))
                {
                    badTriangles.Add(triangle);
                }
            }

            // Step 4: Find the boundary (edges) of the "hole"
            foreach (var triangle in badTriangles)
            {
                foreach (var edge in triangle.GetEdges())
                {
                    bool isShared = false;
                    foreach (var other in badTriangles)
                    {
                        if (triangle != other && other.HasEdge(edge))
                        {
                            isShared = true;
                            break;
                        }
                    }
                    if (!isShared)
                    {
                        polygon.Add(edge);
                    }
                }
            }

            // Remove bad triangles
            triangles.RemoveAll(t => badTriangles.Contains(t));

            // Step 5: Retriangulate the hole
            foreach (var edge in polygon)
            {
                triangles.Add(new Triangle(edge.p1, edge.p2, point));
            }
        }

        // Step 6: Remove triangles connected to the super triangle
        triangles.RemoveAll(t => t.HasVertex(superTriangle.v1) || t.HasVertex(superTriangle.v2) || t.HasVertex(superTriangle.v3));

        // Optional: Visualization (add your visualization code here)
/*        foreach(var triangle in triangles)
        {
            Debug.DrawLine(new Vector3(triangle.v1.x, 0, triangle.v1.y), new Vector3(triangle.v2.x, 0, triangle.v2.y), Color.cyan, 100f);
            Debug.DrawLine(new Vector3(triangle.v2.x, 0, triangle.v2.y), new Vector3(triangle.v3.x, 0, triangle.v3.y), Color.cyan, 100f);
            Debug.DrawLine(new Vector3(triangle.v3.x, 0, triangle.v3.y), new Vector3(triangle.v1.x, 0, triangle.v1.y), Color.cyan, 100f);
        }*/
    }

    // Function to create a super triangle that contains all points
    Triangle CreateSuperTriangle(List<Vector2> points)
    {
        float minX = Mathf.Min(points.ConvertAll(p => p.x).ToArray());
        float maxX = Mathf.Max(points.ConvertAll(p => p.x).ToArray());
        float minY = Mathf.Min(points.ConvertAll(p => p.y).ToArray());
        float maxY = Mathf.Max(points.ConvertAll(p => p.y).ToArray());

        float dx = maxX - minX;
        float dy = maxY - minY;

        Vector2 v1 = new Vector2(minX - 10 * dx, minY - dy);
        Vector2 v2 = new Vector2(minX + 10 * dx, minY - dy);
        Vector2 v3 = new Vector2((minX + maxX) / 2, maxY + 10 * dy);

        return new Triangle(v1, v2, v3);
    }

    // Check if point lies inside triangle's circumcircle
    bool IsPointInCircumcircle(Vector2 p, Triangle t)
    {
        float ab = t.v1.sqrMagnitude;
        float cd = t.v2.sqrMagnitude;
        float ef = t.v3.sqrMagnitude;

        float circum_x = (ab * (t.v3.y - t.v2.y) + cd * (t.v1.y - t.v3.y) + ef * (t.v2.y - t.v1.y)) /
                         (t.v1.x * (t.v3.y - t.v2.y) + t.v2.x * (t.v1.y - t.v3.y) + t.v3.x * (t.v2.y - t.v1.y)) / 2;

        float circum_y = (ab * (t.v3.x - t.v2.x) + cd * (t.v1.x - t.v3.x) + ef * (t.v2.x - t.v1.x)) /
                         (t.v1.y * (t.v3.x - t.v2.x) + t.v2.y * (t.v1.x - t.v3.x) + t.v3.y * (t.v2.x - t.v1.x)) / 2;

        Vector2 circumCenter = new Vector2(circum_x, circum_y);
        float circumRadius = (t.v1 - circumCenter).magnitude;

        return (p - circumCenter).magnitude <= circumRadius;
    }
}

public class Triangle
{
    public Vector2 v1, v2, v3;

    public Triangle(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        v1 = p1;
        v2 = p2;
        v3 = p3;
    }

    public bool HasVertex(Vector2 v)
    {
        return v1 == v || v2 == v || v3 == v;
    }

    public List<Edge> GetEdges()
    {
        return new List<Edge> {
            new Edge(v1, v2),
            new Edge(v2, v3),
            new Edge(v3, v1)
        };
    }

    public bool HasEdge(Edge edge)
    {
        return GetEdges().Exists(e => e.Equals(edge));
    }
}

public class Edge
{
    public Vector2 p1, p2;

    public Edge(Vector2 a, Vector2 b)
    {
        p1 = a;
        p2 = b;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Edge)) return false;

        Edge other = (Edge)obj;
        return (p1 == other.p1 && p2 == other.p2) || (p1 == other.p2 && p2 == other.p1);
    }

    public override int GetHashCode()
    {
        return p1.GetHashCode() + p2.GetHashCode();
    }

    public Vector2 OtherPoint(Edge e, Vector2 point)
    {
        if (point == e.p1)
        {
            return p2;
        }
        else
        {
            return p1;
        }
    }
}
