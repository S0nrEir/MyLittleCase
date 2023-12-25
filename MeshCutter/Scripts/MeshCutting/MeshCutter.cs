using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCutter
{

    public TempMesh PositiveMesh { get; private set; }
    public TempMesh NegativeMesh { get; private set; }

    private List<Vector3> addedPairs;

    private readonly List<Vector3> ogVertices;
    private readonly List<int> ogTriangles;
    private readonly List<Vector3> ogNormals;
    private readonly List<Vector2> ogUvs;

    private readonly Vector3[] intersectPair;
    private readonly Vector3[] tempTriangle;

    private Intersections intersect;

    private readonly float threshold = 1e-6f;

    public MeshCutter(int initialArraySize)
    {
        PositiveMesh = new TempMesh(initialArraySize);
        NegativeMesh = new TempMesh(initialArraySize);

        addedPairs = new List<Vector3>(initialArraySize);
        ogVertices = new List<Vector3>(initialArraySize);
        ogNormals = new List<Vector3>(initialArraySize);
        ogUvs = new List<Vector2>(initialArraySize);
        ogTriangles = new List<int>(initialArraySize * 3);

        intersectPair = new Vector3[2];
        tempTriangle = new Vector3[3];

        intersect = new Intersections();
    }

    /// <summary>
    /// Slice a mesh by the slice plane.
    /// We assume the plane is already in the mesh's local coordinate frame
    /// Returns posMesh and negMesh, which are the resuling meshes on both sides of the plane 
    /// (posMesh on the same side as the plane's normal, negMesh on the opposite side)
    /// 通过一个plane切割一个mesh，假定plane已经在mesh的模型空间下。
    /// 返回正面和反面两个mesh，他们被规划在plane的两边，
    /// </summary>
    public bool SliceMesh(Mesh mesh, ref Plane slice)
    {
        // Let's always fill the vertices array so that we can access it even if the mesh didn't intersect
        //获取mesh的所有顶点
        mesh.GetVertices(ogVertices);

        // 1. Verify if the bounds intersect first
        //检查mesh和平面是否相交
        if (!Intersections.BoundPlaneIntersect(mesh, ref slice))
            return false;

        //获取三角形索引[0,1,2,2,3,0]
        mesh.GetTriangles(ogTriangles, 0);
        //获取网格的顶点法线
        mesh.GetNormals(ogNormals);
        //获取uv集合(0,0),(0,1),(1,0)
        mesh.GetUVs(0, ogUvs);

        PositiveMesh.Clear();
        NegativeMesh.Clear();
        addedPairs.Clear();

        //在新mesh中分离老顶点
        for(int i = 0; i < ogVertices.Count; ++i)
        {
            //检查源mesh每个顶点到平面的距离，>=0表示该顶点在plane的正面，否则z反面
            //将mesh的顶点分割成正面和反面两部分
            if (slice.GetDistanceToPoint(ogVertices[i]) >= 0)
                PositiveMesh.AddVertex(ogVertices, ogNormals, ogUvs, i);
            else
                NegativeMesh.AddVertex(ogVertices, ogNormals, ogUvs, i);
        }

        //如果有任意一个集合是空的，则代表没有切割面和mesh没有相交
        if (NegativeMesh.vertices.Count == 0 || PositiveMesh.vertices.Count == 0)
            return false;

        // 3. Separate triangles and cut those that intersect the plane
        //遍历每一个三角形，分离并且切割与切割面相交的三角形
        for (int i = 0; i < ogTriangles.Count; i += 3)
        {
            if (intersect.TrianglePlaneIntersect(ogVertices, ogUvs, ogTriangles, i, ref slice, PositiveMesh, NegativeMesh, intersectPair))
                addedPairs.AddRange(intersectPair);
        }

        if (addedPairs.Count > 0)
        {
            //FillBoundaryGeneral(addedPairs);
            FillBoundaryFace(addedPairs);
            return true;
        } else
        {
            throw new UnityException("Error: if added pairs is empty, we should have returned false earlier");
        }
    }

    public Vector3 GetFirstVertex()
    {
        if (ogVertices.Count == 0)
            throw new UnityException(
                "Error: Either the mesh has no vertices or GetFirstVertex was called before SliceMesh.");
        else
            return ogVertices[0];
    }

    #region Boundary fill method


    private void FillBoundaryGeneral(List<Vector3> added)
    {
        // 1. Reorder added so in order ot their occurence along the perimeter.
        MeshUtils.ReorderList(added);

        Vector3 center = MeshUtils.FindCenter(added);

        //Create triangle for each edge to the center
        tempTriangle[2] = center;

        for (int i = 0; i < added.Count; i += 2)
        {
            // Add fronface triangle in meshPositive
            tempTriangle[0] = added[i];
            tempTriangle[1] = added[i + 1];

            PositiveMesh.AddTriangle(tempTriangle);

            // Add backface triangle in meshNegative
            tempTriangle[0] = added[i + 1];
            tempTriangle[1] = added[i];

            NegativeMesh.AddTriangle(tempTriangle);
        }
    }


    private void FillBoundaryFace(List<Vector3> added)
    {
        // 1. Reorder added so in order ot their occurence along the perimeter.
        MeshUtils.ReorderList(added);

        // 2. Find actual face vertices
        var face = FindRealPolygon(added);

        // 3. Create triangle fans
        int t_fwd = 0,
            t_bwd = face.Count - 1,
            t_new = 1;
        bool incr_fwd = true;

        while (t_new != t_fwd && t_new != t_bwd)
        {
            AddTriangle(face, t_bwd, t_fwd, t_new);

            if (incr_fwd) t_fwd = t_new;
            else t_bwd = t_new;

            incr_fwd = !incr_fwd;
            t_new = incr_fwd ? t_fwd + 1 : t_bwd - 1;
        }
    }

    /// <summary>
    /// Extract polygon from the pairs of vertices.
    /// Per example, two vectors that are colinear is redundant and only forms one side of the polygon
    /// </summary>
    private List<Vector3> FindRealPolygon(List<Vector3> pairs)
    {
        List<Vector3> vertices = new List<Vector3>();
        Vector3 edge1, edge2;

        // List should be ordered in the correct way
        for (int i = 0; i < pairs.Count; i += 2)
        {
            edge1 = (pairs[i + 1] - pairs[i]);
            if (i == pairs.Count - 2)
                edge2 = pairs[1] - pairs[0];
            else
                edge2 = pairs[i + 3] - pairs[i + 2];

            // Normalize edges
            edge1.Normalize();
            edge2.Normalize();

            if (Vector3.Angle(edge1, edge2) > threshold)
                // This is a corner
                vertices.Add(pairs[i + 1]);
        }

        return vertices;
    }

    private void AddTriangle(List<Vector3> face, int t1, int t2, int t3)
    {
        tempTriangle[0] = face[t1];
        tempTriangle[1] = face[t2];
        tempTriangle[2] = face[t3];
        PositiveMesh.AddTriangle(tempTriangle);

        tempTriangle[1] = face[t3];
        tempTriangle[2] = face[t2];
        NegativeMesh.AddTriangle(tempTriangle);
    }
    #endregion
    
}

