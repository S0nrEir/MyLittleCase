using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;

namespace VoxelSystem
{

    public class VoxelMesh 
    {
	    /// <summary>
	    /// 生成一个以给定的中心点，偏移量，和分割数为基础的网格，生成顶点，法线，中心点，三角面索引
	    /// </summary>
	    public static Mesh Build(Voxel_t[] voxels, float unit, bool useUV = false) 
		{
			var vertices = new List<Vector3>();
			var uvs = new List<Vector2>();
			var triangles = new List<int>();
			var normals = new List<Vector3>();
			var centers = new List<Vector4>();
			
			var up = Vector3.up * unit;
			var hup = up * 0.5f;
			var hbottom = -hup;

			var right = Vector3.right * unit;
			var hright = right * 0.5f;

			var left = -right;
			var hleft = left * 0.5f;

			var forward = Vector3.forward * unit;
			var hforward = forward * 0.5f;
			var back = -forward;
			var hback = back * 0.5f;

			for(int i = 0, n = voxels.Length; i < n; i++) 
			{
				var v = voxels[i];
				//所有包含模型的体素
				if(v.fill > 0) 
				{
					// back
					//这里调用都是不传入segment，使用默认参数的版本，也就是步长分两段,0和1
					CalculatePlane
					(
						vertices, normals, centers, uvs, triangles,
						v, useUV, hback, right, up, Vector3.back
					);

					// right
					CalculatePlane
					(
						vertices, normals, centers, uvs, triangles,
						v, useUV, hright, forward, up, Vector3.right
					);

					// forward
					CalculatePlane
					(
						vertices, normals, centers, uvs, triangles,
						v, useUV, hforward, left, up, Vector3.forward
					);

					// left
					CalculatePlane
					(
						vertices, normals, centers, uvs, triangles,
						v, useUV, hleft, back, up, Vector3.left
					);

					// up
					CalculatePlane
					(
						vertices, normals, centers, uvs, triangles,
						v, useUV, hup, right, forward, Vector3.up
					);

					// down
					CalculatePlane
					(
						vertices, normals, centers, uvs, triangles,
						v, useUV, hbottom, right, back, Vector3.down
					);

				}
			}
			
			var mesh = new Mesh();
			mesh.indexFormat = IndexFormat.UInt32;
			mesh.vertices = vertices.ToArray();
			mesh.uv = uvs.ToArray();
			mesh.normals = normals.ToArray();
			mesh.tangents = centers.ToArray();
			mesh.SetTriangles(triangles.ToArray(), 0);
			mesh.RecalculateBounds();
			return mesh;
		}

		static void CalculatePlane 
		(
			List<Vector3> vertices, 
			List<Vector3> normals, 
			List<Vector4> centers, 
			List<Vector2> uvs, 
			List<int> triangles,
			Voxel_t voxel, 
			bool useUV, 
			Vector3 offset,//hback,hright,hforward,hleft,hup,hbottom，表示当前处理的方向
			Vector3 right, //right,forward,left,back,right,right
			Vector3 up, //up,up,up,up,forward,back
			Vector3 normal, //vector3.back,right,forward,left,up,down
			int rSegments = 2, //半径和垂直方向上的分割步长（分割数）
			int uSegments = 2
		)
		{
			//计算逆步长，rSegments和uSegements的倒数，用于在两个方向上均匀分割网格。
			//他们将用于确定顶点的位置
			float rInv = 1f / (rSegments - 1);
			float uInv = 1f / (uSegments - 1);
				
			int triangleOffset = vertices.Count;
            var center = voxel.position;
            //基于体素中心位置六方向的体素大小一半的偏移
            var transformed = center + offset;
            //外层遍历垂直方向，内层遍历半径水平方向
			for(int y = 0; y < uSegments; y++) 
			{
				//垂直方向上的归一化坐标，这用于确定顶点在垂直方向上的位置
				//y=0,ru=0
				//y=uSegments-1,ru=1
				//在遍历内将y映射在0~1之间，水平方向同理
				float ru = y * uInv;
				for(int x = 0; x < rSegments; x++) 
				{
					//水平方向上的归一化坐标
					//rr和ru将同样被映射为当前步长的百分比
					float rr = x * rInv;
					//体素八个顶点的位置基于：传入的指定方向加上垂直和水平方向轴两个正负方向上的偏移，
					vertices.Add(transformed + right * (rr - 0.5f) + up * (ru - 0.5f));
					normals.Add(normal);
					centers.Add(center);

                    if(useUV)
					    uvs.Add(voxel.uv); 
                    else
					    uvs.Add(new Vector2(rr, ru));
				}
				
				//最后一行不创建三角形
				if(y < uSegments - 1) 
				{
					var ioffset = y * rSegments + triangleOffset;
					for(int x = 0, n = rSegments - 1; x < n; x++) 
					{
						triangles.Add(ioffset + x);
						triangles.Add(ioffset + x + rSegments);
						triangles.Add(ioffset + x + 1);

						triangles.Add(ioffset + x + 1);
						triangles.Add(ioffset + x + rSegments);
						triangles.Add(ioffset + x + 1 + rSegments);
					}
				}
			}//end for
		}

    }

}


