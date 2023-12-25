using System.Collections.Generic;
using UnityEngine;

namespace VoxelSystem.Demo
{
    //source code by:https://github.com/mattatz/unity-voxel
    [RequireComponent( typeof( MeshFilter ) )]
    public class CPUDemo : MonoBehaviour
    {
        [SerializeField] protected Mesh mesh;

        /// <summary>
        /// 体素精度，越大生成的体素越小，精度越高，模型越细
        /// </summary>
        [SerializeField] protected int resolution = 24;

        [SerializeField] protected bool useUV = false;

        /// <summary>
        /// 临时位置标记模型
        /// </summary>
        [SerializeField] private GameObject _positionGO = null;

        void Start()
        {
            //Vector3 zero = new Vector3( 0, 0, 0 );
            //Vector3 t0 = new Vector3( 10, 0,10 );
            //Vector3 t1 = new Vector3( -10, 0, 10 );
            //Debug.Log( "0:   " + Vector3.Dot( Vector3.forward.normalized, ( t0 - zero ).normalized ) );
            //Debug.Log( "1:   " + Vector3.Dot( Vector3.forward.normalized, ( t1 - zero ).normalized ) );

            //return;

            //体素列表
            //
            List<Voxel_t> voxels;
            float unit;
            //体素化网格
            CPUVoxelizer.Voxelize( mesh, resolution, out voxels, out unit, _positionGO, transform );

            var filter = GetComponent<MeshFilter>();
            filter.sharedMesh = VoxelMesh.Build( voxels.ToArray(), unit, useUV );
        }

    }

}


