using System.Collections.Generic;
using UnityEngine;

namespace VoxelSystem
{

    public class CPUVoxelizer
    {

        public class Triangle
        {
            public Vector3 a, b, c;
            public Bounds bounds;
            public bool frontFacing;

            public Triangle( Vector3 a, Vector3 b, Vector3 c, Vector3 dir )
            {
                this.a = a;
                this.b = b;
                this.c = c;

                var cross = Vector3.Cross( b - a, c - a );
                this.frontFacing = ( Vector3.Dot( cross, dir ) <= 0f );

                var min = Vector3.Min( Vector3.Min( a, b ), c );
                var max = Vector3.Max( Vector3.Max( a, b ), c );
                bounds.SetMinMax( min, max );
            }

            public Vector2 GetUV( Vector3 p, Vector2 uva, Vector2 uvb, Vector2 uvc )
            {
                float u, v, w;
                Barycentric( p, out u, out v, out w );
                return uva * u + uvb * v + uvc * w;
            }

            // https://gamedev.stackexchange.com/questions/23743/whats-the-most-efficient-way-to-find-barycentric-coordinates
            public void Barycentric( Vector3 p, out float u, out float v, out float w )
            {
                Vector3 v0 = b - a, v1 = c - a, v2 = p - a;
                float d00 = Vector3.Dot( v0, v0 );
                float d01 = Vector3.Dot( v0, v1 );
                float d11 = Vector3.Dot( v1, v1 );
                float d20 = Vector3.Dot( v2, v0 );
                float d21 = Vector3.Dot( v2, v1 );
                float denom = 1f / ( d00 * d11 - d01 * d01 );
                v = ( d11 * d20 - d01 * d21 ) * denom;
                w = ( d00 * d21 - d01 * d20 ) * denom;
                u = 1.0f - v - w;
            }

        }

        // http://blog.wolfire.com/2009/11/Triangle-mesh-voxelization
        //体素化网格
        /// <summary>
        /// 体素化
        /// </summary>
        /// <param name="mesh">网格</param>
        /// <param name="resolution">体素精度</param>
        /// <param name="voxels">体素集合</param>
        /// <param name="unit">单位体素大小</param>
        /// <param name="tempGO">临时显示go</param>
        /// <param name="meshTrans">临时显示mesh的transform</param>
        public static void Voxelize( Mesh mesh, int resolution, out List<Voxel_t> voxels, out float unit, GameObject tempGO = null, Transform meshTrans = null )
        {
            //重新计算模型的包围盒
            mesh.RecalculateBounds();

            var bounds = mesh.bounds;
            //获取包围盒最长的一边的长度
            float maxLength = Mathf.Max( bounds.size.x, Mathf.Max( bounds.size.y, bounds.size.z ) );

            //包围盒的最小点和最大点
            //ShowPoint( meshTrans, bounds.min, tempGO, "min" );
            //ShowPoint( meshTrans, bounds.max, tempGO, "min" );

            //每单位体素的大小
            unit = maxLength / resolution;
            //半体素
            var hunit = unit * 0.5f;

            //注意这里是基于模型坐标的
            //体素检测的起始点
            var start = bounds.min - new Vector3( hunit, hunit, hunit );
            //ShowPoint( meshTrans, start, tempGO, "start" );
            //体素检测的终点
            var end = bounds.max + new Vector3( hunit, hunit, hunit );
            //ShowPoint( meshTrans, end, tempGO, "end" );
            var size = end - start;

            //要生成的三个方向的体素数量
            var width = Mathf.CeilToInt( size.x / unit );
            var height = Mathf.CeilToInt( size.y / unit );
            var depth = Mathf.CeilToInt( size.z / unit );

            //准备体素数据和包围盒数据
            var volume = new Voxel_t[width, height, depth];
            //用这些包围盒分割模型
            var boxes = new Bounds[width, height, depth];
            //体素大小
            var voxelSize = Vector3.one * unit;
            for ( int x = 0; x < width; x++ )
            {
                for ( int y = 0; y < height; y++ )
                {
                    for ( int z = 0; z < depth; z++ )
                    {
                        //将模型按体素大小平均切分后，在每个位置都生成一个包围盒
                        //生成新的包围盒，指定位置和大小，注意位置是基于模型空间的
                        var p = new Vector3( x, y, z ) * unit + start;
                        var aabb = new Bounds( p, voxelSize );
                        boxes[x, y, z] = aabb;
                    }
                }
            }//end for

            // build triangles
            //构建网格多边形
            var vertices = mesh.vertices;//顶点
            var uvs = mesh.uv;
            var uv00 = Vector2.zero;
            var indices = mesh.triangles;//三角面顶点集合
            var direction = Vector3.forward;

            //检查每个三角面包围盒内的体素，检查他们是否和模型顶点相交（？）
            for ( int i = 0, n = indices.Length; i < n; i += 3 )
            {
                //生成三角形并且生成他们的包围盒
                //根据模型顶点构建三角面的数据结构
                var tri = new Triangle
                    (
                        vertices[indices[i]],
                        vertices[indices[i + 1]],
                        vertices[indices[i + 2]],
                        direction
                    );

                Vector2 uva, uvb, uvc;
                if ( uvs.Length > 0 )
                {
                    uva = uvs[indices[i]];
                    uvb = uvs[indices[i + 1]];
                    uvc = uvs[indices[i + 2]];
                }
                //默认不要uv
                else
                {
                    uva = uvb = uvc = uv00;
                }

                //当前三角面的包围盒的最小和最大点距离体素检测起始点的位置
                //算出基于体素检测起始点的三角面包围盒的最小和最大位置
                var min = tri.bounds.min - start;
                var max = tri.bounds.max - start;

                //起始点到每模型三角面包围盒最小点要生成的体素数量
                //三角面的包围盒的最最小点，作为索引起始
                int iminX = Mathf.RoundToInt( min.x / unit );
                var iminY = Mathf.RoundToInt( min.y / unit );
                var iminZ = Mathf.RoundToInt( min.z / unit );

                //起始点到每模型三角面包围盒最大点要生成的体素数量
                //三角面的包围盒的最大点、作为索引终点
                int imaxX = Mathf.RoundToInt( max.x / unit );
                var imaxY = Mathf.RoundToInt( max.y / unit );
                var imaxZ = Mathf.RoundToInt( max.z / unit );
                // int iminX = Mathf.FloorToInt(min.x / unit), iminY = Mathf.FloorToInt(min.y / unit), iminZ = Mathf.FloorToInt(min.z / unit);
                // int imaxX = Mathf.CeilToInt(max.x / unit), imaxY = Mathf.CeilToInt(max.y / unit), imaxZ = Mathf.CeilToInt(max.z / unit);

                //修正，将x/y/z限制在width-1/height-1/depth-1到0之间
                iminX = Mathf.Clamp( iminX, 0, width - 1 );
                iminY = Mathf.Clamp( iminY, 0, height - 1 );
                iminZ = Mathf.Clamp( iminZ, 0, depth - 1 );
                imaxX = Mathf.Clamp( imaxX, 0, width - 1 );
                imaxY = Mathf.Clamp( imaxY, 0, height - 1 );
                imaxZ = Mathf.Clamp( imaxZ, 0, depth - 1 );

                // Debug.Log((iminX + "," + iminY + "," + iminZ) + " ~ " + (imaxX + "," + imaxY + "," + imaxZ));

                //该三角面是否面向正面(0,0,1)
                uint front = ( uint ) ( tri.frontFacing ? 1 : 0 );

                for ( int x = iminX; x <= imaxX; x++ )
                {
                    for ( int y = iminY; y <= imaxY; y++ )
                    {
                        for ( int z = iminZ; z <= imaxZ; z++ )
                        {
                            //拿到所有包含该三角面内的体素，然后找出和三角面相交的
                             if ( Intersects( tri, boxes[x, y, z] ) )
                            {
                                var voxel = volume[x, y, z];
                                voxel.position = boxes[x, y, z].center;
                                voxel.uv = tri.GetUV( voxel.position, uva, uvb, uvc );
                                //未填充
                                if ( ( voxel.fill & 1 ) == 0 )
                                {
                                    voxel.front = front;
                                }
                                else
                                {
                                    voxel.front = voxel.front & front;
                                }
                                voxel.fill = voxel.fill | 1;
                                volume[x, y, z] = voxel;
                            }
                        }
                    }
                }
            }//end for

            for ( int x = 0; x < width; x++ )
            {
                for ( int y = 0; y < height; y++ )
                {
                    for ( int z = 0; z < depth; z++ )
                    {
                        if ( volume[x, y, z].IsEmpty() ) continue;

                        int ifront = z;

                        Vector2 uv = Vector2.zero;
                        for ( ; ifront < depth; ifront++ )
                        {
                            if ( !volume[x, y, ifront].IsFrontFace() )
                            {
                                break;
                            }
                            uv = volume[x, y, ifront].uv;
                        }

                        if ( ifront >= depth ) 
                            break;

                        var iback = ifront;

                        // step forward to cavity
                        for ( ; iback < depth && volume[x, y, iback].IsEmpty(); iback++ ) { }

                        if ( iback >= depth ) break;

                        // check if iback is back voxel
                        if ( volume[x, y, iback].IsBackFace() )
                        {
                            // step forward to back face
                            for ( ; iback < depth && volume[x, y, iback].IsBackFace(); iback++ ) { }
                        }

                        // fill from ifront to iback
                        for ( int z2 = ifront; z2 < iback; z2++ )
                        {
                            var p = boxes[x, y, z2].center;
                            var voxel = volume[x, y, z2];
                            voxel.position = p;
                            voxel.uv = uv;
                            voxel.fill = 1;
                            volume[x, y, z2] = voxel;
                        }

                        z = iback;
                    }
                }
            }//end for

            voxels = new List<Voxel_t>();
            for ( int x = 0; x < width; x++ )
            {
                for ( int y = 0; y < height; y++ )
                {
                    for ( int z = 0; z < depth; z++ )
                    {
                        if ( !volume[x, y, z].IsEmpty() )
                        {
                            voxels.Add( volume[x, y, z] );
                        }
                    }
                }
            }//end for
        }

        /// <summary>
        /// 检查一个三角面是否和一个包围盒相交
        /// </summary>
        public static bool Intersects( Triangle tri, Bounds aabb )
        {
            //将三角形从原始空间转换到AABB空间，检测是否相交
            float p0, p1, p2, r;

            //包围盒的中心点和半径
            Vector3 center = aabb.center;
            var extents = aabb.max - center;

            //三角形的三个顶点相对于中心点的方向
            Vector3 v0 = tri.a - center;
            var v1 = tri.b - center;
            var v2 = tri.c - center;

            //三条边互相指向的方向向量
            Vector3 f0 = v1 - v0;
            var f1 = v2 - v1;
            var f2 = v0 - v2;

            //a00`将被用作一个可能的分离轴，用于检测三角形和AABB是否相交。具体来说，代码将会计算三角形的每个顶点在这个轴上的投影，并检查这些投影是否都在AABB在该轴上的投影之外。如果是这样，那么根据分离轴定理，可以得出三角形和AABB不相交。
            //在3D图形中，通常使用一种叫做分离轴定理的方法来进行碰撞检测。基本思想是，如果可以找到一个轴，使得两个物体在这个轴上的投影不重叠，那么这两个物体就不会碰撞。
            //所以，这一部分实际上是在测试9个可能的分离轴（由a00到a22表示），看是否存在一个轴可以将两个物体分离。如果所有的轴都不能将物体分离，那就可以判断这两个物体发生了碰撞。
            //这里实际上就是对每个顶点和包围盒做碰撞检测

            //获取三角形每条边的分离轴
            Vector3 a00 = new Vector3( 0, -f0.z, f0.y ),
                a01 = new Vector3( 0, -f1.z, f1.y ),
                a02 = new Vector3( 0, -f2.z, f2.y ),
                a10 = new Vector3( f0.z, 0, -f0.x ),
                a11 = new Vector3( f1.z, 0, -f1.x ),
                a12 = new Vector3( f2.z, 0, -f2.x ),
                a20 = new Vector3( -f0.y, f0.x, 0 ),
                a21 = new Vector3( -f1.y, f1.x, 0 ),
                a22 = new Vector3( -f2.y, f2.x, 0 );

            // Test axis a00
            //三角形每个顶点在分离轴上的投影
            //因为分离轴的特性，a00垂直于f0，所以在后续的aabb投影计算中，f0有两个分量是可以投影在a00上的，这取决于a00是了交换f0的哪些分量计算出来的
            p0 = Vector3.Dot( v0, a00 );
            p1 = Vector3.Dot( v1, a00 );
            p2 = Vector3.Dot( v2, a00 );

            //包围盒在f0上的投影的半径
            //思路类似于点乘求投影，计算包围盒在f0.z和f0.y上的长度
            //如果任何一个顶点的投影长度超过了这个长度，那么就可以认为这两个物体是分离的，也就是没有碰撞
            r = extents.y * Mathf.Abs( f0.z ) + extents.z * Mathf.Abs( f0.y );
            
            //检查三角形和aabb在分离轴上的投影是否重叠，没有重叠返回false
            //这里其实是在检查三角形的顶点和碰撞盒在分离轴上的投影是否相交，大于r表示顶点位置超出了碰撞盒在分离轴上的投影，所以不相交
            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a01z
            p0 = Vector3.Dot( v0, a01 );
            p1 = Vector3.Dot( v1, a01 );
            p2 = Vector3.Dot( v2, a01 );
            r = extents.y * Mathf.Abs( f1.z ) + extents.z * Mathf.Abs( f1.y );

            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a02
            p0 = Vector3.Dot( v0, a02 );
            p1 = Vector3.Dot( v1, a02 );
            p2 = Vector3.Dot( v2, a02 );
            r = extents.y * Mathf.Abs( f2.z ) + extents.z * Mathf.Abs( f2.y );

            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a10
            p0 = Vector3.Dot( v0, a10 );
            p1 = Vector3.Dot( v1, a10 );
            p2 = Vector3.Dot( v2, a10 );
            r = extents.x * Mathf.Abs( f0.z ) + extents.z * Mathf.Abs( f0.x );
            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a11
            p0 = Vector3.Dot( v0, a11 );
            p1 = Vector3.Dot( v1, a11 );
            p2 = Vector3.Dot( v2, a11 );
            r = extents.x * Mathf.Abs( f1.z ) + extents.z * Mathf.Abs( f1.x );

            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a12
            p0 = Vector3.Dot( v0, a12 );
            p1 = Vector3.Dot( v1, a12 );
            p2 = Vector3.Dot( v2, a12 );
            r = extents.x * Mathf.Abs( f2.z ) + extents.z * Mathf.Abs( f2.x );

            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a20
            p0 = Vector3.Dot( v0, a20 );
            p1 = Vector3.Dot( v1, a20 );
            p2 = Vector3.Dot( v2, a20 );
            r = extents.x * Mathf.Abs( f0.y ) + extents.y * Mathf.Abs( f0.x );

            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a21
            p0 = Vector3.Dot( v0, a21 );
            p1 = Vector3.Dot( v1, a21 );
            p2 = Vector3.Dot( v2, a21 );
            r = extents.x * Mathf.Abs( f1.y ) + extents.y * Mathf.Abs( f1.x );

            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            // Test axis a22
            p0 = Vector3.Dot( v0, a22 );
            p1 = Vector3.Dot( v1, a22 );
            p2 = Vector3.Dot( v2, a22 );
            r = extents.x * Mathf.Abs( f2.y ) + extents.y * Mathf.Abs( f2.x );

            if ( Mathf.Max( -Mathf.Max( p0, p1, p2 ), Mathf.Min( p0, p1, p2 ) ) > r )
            {
                return false;
            }

            if ( Mathf.Max( v0.x, v1.x, v2.x ) < -extents.x || Mathf.Min( v0.x, v1.x, v2.x ) > extents.x )
            {
                return false;
            }

            if ( Mathf.Max( v0.y, v1.y, v2.y ) < -extents.y || Mathf.Min( v0.y, v1.y, v2.y ) > extents.y )
            {
                return false;
            }

            if ( Mathf.Max( v0.z, v1.z, v2.z ) < -extents.z || Mathf.Min( v0.z, v1.z, v2.z ) > extents.z )
            {
                return false;
            }
            //三角面的两条边取法线，tri.a在normal方向上的投影长度作为plane的距离，构建一个plane
            //检查该plane是否和aabb相交
            var normal = Vector3.Cross( f1, f0 ).normalized;
            var pl = new Plane( normal, Vector3.Dot( normal, tri.a ) );
            return Intersects( pl, aabb );
        }

        public static bool Intersects( Plane pl, Bounds aabb )
        {
            Vector3 center = aabb.center;
            var extents = aabb.max - center;
            //求出平面法线在AABB内各方向上能达到的最大长度，作为检查是否相交的依据
            var r = extents.x * Mathf.Abs( pl.normal.x ) + extents.y * Mathf.Abs( pl.normal.y ) + extents.z * Mathf.Abs( pl.normal.z );
            //s=平面和AABB的实际距离
            var s = Vector3.Dot( pl.normal, center ) - pl.distance;
            //如果平面和边界框的最大距离r小于平面和AABB中心的实际距离则不相交
            return Mathf.Abs( s ) <= r;
        }


        private static void ShowPoint( Transform tran, Vector3 point, GameObject positionGO, string name )
        {
            if ( tran == null || positionGO == null )
                return;

            var go = UnityEngine.GameObject.Instantiate( positionGO );
            var boundsSize = new Vector3( point.x, point.y, point.z );
            go.transform.position = tran.TransformPoint( boundsSize );
            go.transform.localScale = new Vector3( .3f, .3f, .3f );
            go.name = name;
            go.SetActive( true );
        }
    }

}


