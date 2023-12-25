using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 表示一个三角面是否和切割线相交的类
/// </summary>
public class Intersections
{
    #region Static functions

    /// <summary>
    /// Based on https://gdbooks.gitbooks.io/3dcollisions/content/Chapter2/static_aabb_plane.html
    /// 通过aabb包围盒检查plane和mesh是否相交
    /// </summary>
    public static bool BoundPlaneIntersect(Mesh mesh, ref Plane plane)
    {
        // Compute projection interval radius
        float r = mesh.bounds.extents.x * Mathf.Abs(plane.normal.x) +
            mesh.bounds.extents.y * Mathf.Abs(plane.normal.y) +
            mesh.bounds.extents.z * Mathf.Abs(plane.normal.z);

        // Compute distance of box center from plane
        float s = Vector3.Dot(plane.normal, mesh.bounds.center) - (-plane.distance);

        // Intersection occurs when distance s falls within [-r,+r] interval
        return Mathf.Abs(s) <= r;
    }

    #endregion

    // Initialize fixed arrays so that we don't initialize them every time we call TrianglePlaneIntersect
    private readonly Vector3[] v;
    private readonly Vector2[] u;
    /// <summary>
    /// 顶点索引
    /// </summary>
    private readonly int[] t;

    /// <summary>
    /// 表示三角形的顶点在positive还是negative中
    /// </summary>
    private readonly bool[] positive;

    // Used in intersect method
    private Ray edgeRay;

    public Intersections()
    {
        v = new Vector3[3];
        u = new Vector2[3];
        t = new int[3];
        positive = new bool[3];
    }

    /// <summary>
    /// Find intersection between a plane and a line segment defined by vectors first and second.
    /// </summary>
    public ValueTuple<Vector3, Vector2> Intersect
        (
            Plane plane, //切割面
            Vector3 first, //切割点
            Vector3 second, //前一个点
            Vector2 uv1, //切割点对应的UV
            Vector2 uv2 //前一个点对应的UV
        )
    {
        //设置一条从切割点(lonelyPoint)到它前一个点的射线
        edgeRay.origin = first;
        edgeRay.direction = (second - first).normalized;

        //射线到平面的距离
        float dist;
        float maxDist = Vector3.Distance(first, second);

        //射线是否与平面相交
        if (!plane.Raycast(edgeRay, out dist))
            // Intersect in wrong direction...
            throw new UnityException("Line-Plane intersect in wrong direction");
        else if (dist > maxDist)
            // Intersect outside of line segment
            throw new UnityException("Intersect outside of line");

        var returnVal = new ValueTuple<Vector3, Vector2>
        {
            //拿到射线打在平面上的点，这作为切割点的第一个起始点
            Item1 = edgeRay.GetPoint(dist)
        };

        var relativeDist = dist / maxDist;
        //根据切割点到临近点的距离取百分比，做插值，作为重新映射纹理的依据
        returnVal.Item2.x = Mathf.Lerp(uv1.x, uv2.x, relativeDist);
        returnVal.Item2.y = Mathf.Lerp(uv1.y, uv2.y, relativeDist);
        return returnVal;
    }

    /*
     *       |      |  /|
     *       |      | / |P1       
     *       |      |/  |         
     *       |    I1|   |
     *       |     /|   |
     *      y|    / |   |
     *       | P0/__|___|P2
     *       |      |I2
     *       |      |
     *       |___________________
     */
    /// <summary>
    /// 检查三角面是否与平面相交
    /// </summary>
    public bool TrianglePlaneIntersect(List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, int startIdx, ref Plane plane, TempMesh posMesh, TempMesh negMesh, Vector3[] intersectVectors)
    {
        // Store triangle, vertex and uv from indices
        //保存要分离的mesh的一个三角形包含的顶点和贴图
        for(var i = 0; i < 3; ++i)
        {
            //保存三角面顶点索引
            t[i] = triangles[startIdx + i];
            //保存保存三角面索引对应的顶点
            v[i] = vertices[t[i]];
            //保存对应的UV
            u[i] = uvs[t[i]];
        }

        // Store wether the vertex is on positive mesh
        //不管顶点是否处于正面，都将其保存
        //设置并且获取源mesh某个三角形是否处于切割的positive mesh中，如果不在，那么就在negative mesh中
        //一个三角形如果被分割后，其中必定有一个顶点和当前的切割mesh分离，
        //在之前的步骤中通过检查点到切面的距离来判断一个点属于positive mesh还是negative mesh，
        //在有三角形被切割的情况下，两个切割mesh内部分别保存了划分到自己的顶点集合(AddVertex)，
        //在这里将源mesh的三角形和顶点给入，就可以检查三角形的一个顶点是否在该切割mesh中（positive或nagetive）
        //简单来说，就是在addVertex的时候，两个切割mesh分别保存自己被划分到自己这一面的顶点和在源mesh中对应的三角网格索引位置
        //#todo这里改成自己的实现方案试试？
        posMesh.ContainsKeys(triangles, startIdx, positive);

        // If they're all on the same side, don't do intersection
        //源mesh三角面的所有顶点都在同一个切割mesh中，这说明这个三角面没有被切割，它可能都在posMesh或negativeMesh中
        if (positive[0] == positive[1] && positive[1] == positive[2])
        {
            // All points are on the same side. No intersection
            // Add them to either positive or negative mesh
            //#todo添加该三角面
            //走到这里说明这个三角面不是被切割的三角面
            (positive[0] ? posMesh : negMesh).AddOgTriangle(t);
            return false;
        }

        // Find lonely point
        //找出被切割的点在三角形索引的第几个位置，这里记录的都是他们在原三角面中的顺序位置，而非索引
        //所以lonelyPoint PrevPoint nextPoint这三个变量记录的信息永远是0，1，2，只是顺序可能不一样
        int lonelyPoint = 0;
        if (positive[0] != positive[1])
            lonelyPoint = positive[0] != positive[2] ? 0 : 1;
        else
            lonelyPoint = 2;

        // Set previous point in relation to front face order
        //设置切割点的上一个关联顶点
        int prevPoint = lonelyPoint - 1;
        if (prevPoint == -1) 
            prevPoint = 2;

        // Set next point in relation to front face order
        //设置切割点的下一个关联顶点
        int nextPoint = lonelyPoint + 1;
        if (nextPoint == 3)
            nextPoint = 0;

        // Get the 2 intersection points
        // 找出切割线和三角面相交的两个点(I1和I2两个点)
        ValueTuple<Vector3, Vector2> newPointPrev = Intersect(plane, v[lonelyPoint], v[prevPoint], u[lonelyPoint], u[prevPoint]);
        ValueTuple<Vector3, Vector2> newPointNext = Intersect(plane, v[lonelyPoint], v[nextPoint], u[lonelyPoint], u[nextPoint]);

        //Set the new triangles and store them in respective tempmeshes
        //检查切割点是positive还是negative，将对应的新三角面的另外两个顶点添加进对应的tempMesh中
        (positive[lonelyPoint] ? posMesh : negMesh).AddSlicedTriangle(t[lonelyPoint], newPointNext.Item1, newPointPrev.Item1, newPointNext.Item2, newPointPrev.Item2);

        (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[prevPoint], newPointPrev.Item1, newPointPrev.Item2, t[nextPoint]);

        (positive[prevPoint] ? posMesh : negMesh).AddSlicedTriangle(t[nextPoint], newPointPrev.Item1, newPointNext.Item1, newPointPrev.Item2, newPointNext.Item2);

        // We return the edge that will be in the correct orientation for the positive side mesh
        if (positive[lonelyPoint])
        {
            intersectVectors[0] = newPointPrev.Item1;
            intersectVectors[1] = newPointNext.Item1;
        } else
        {
            intersectVectors[0] = newPointNext.Item1;
            intersectVectors[1] = newPointPrev.Item1;
        }
        return true;
    }



}
