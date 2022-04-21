using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Culling : MonoBehaviour
{
    // Start is called before the first frame update
    void Start ()
    {
        _camera = GetComponent < Camera > ();
        _nearClipPlaneHeight = 2 * _camera.nearClipPlane * Mathf.Tan( _camera.fieldOfView / 2 );
        _farClipPlaneHeight = 2 * _camera.farClipPlane * Mathf.Tan( _camera.fieldOfView / 2 );

        //构建投影矩阵
        //这里记得角度转弧度
        _mFrustum = new Matrix4x4
            (
                new Vector4( (GetCOT( _camera.fieldOfView / 2  * Mathf.Deg2Rad) / _camera.aspect), 0, 0, 0 ),
                new Vector4( 0, GetCOT( _camera.fieldOfView / 2 * Mathf.Deg2Rad ), 0, 0 ),
                new Vector4( 0, 0, ((_camera.farClipPlane + _camera.nearClipPlane) / (_camera.farClipPlane - _camera.nearClipPlane) * -1), -1 ),
                new Vector4( 0, 0, ((2 * _camera.nearClipPlane * _camera.farClipPlane) / (_camera.farClipPlane - _camera.nearClipPlane)) * -1, 0 )
            );
        //COS YU
        //Sin

        var objs = GameObject.FindGameObjectsWithTag( "CullingObjects" );
        foreach (var raw in objs)
        {
            var len = raw.transform.childCount;
            for (int i = 0; i < len; i++)
            {
                var go = raw.transform.GetChild( i );
                if (go.GetComponent<MeshRenderer>() == null)
                    continue;

                _cullingObjects.Add( go.transform );
                if (!_renderCache.ContainsKey( go.transform ))
                    _renderCache.Add( go.transform, go.GetComponent<MeshRenderer>() );
            }
        }
    }

    /// <summary>
    /// 余切函数，cos∩和sin∩
    /// </summary>
    private float GetCOT (float delta)
    {
        var cos = Mathf.Cos( delta );
        var sin = Mathf.Sin( delta );
        return cos / sin;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var tran in _cullingObjects)
        {
            //物体的世界坐标变换到相机空间
            var cameraSpacePos = _camera.worldToCameraMatrix * new Vector4( tran.position.x, tran.position.y, tran.position.z, 1 );

            tran.gameObject.SetActive( IsInView( cameraSpacePos ) );
            //_isInView = IsInView( cameraSpacePos );
            //_currRender = _renderCache[tran];
            //if (_isInView)
            //{
            //    _currRender.enabled = true;
            //}
            //else
            //{
            //    _currRender.enabled = false;
            //}
        }
    }

    /// <summary>
    /// 是否处于视锥体视野范围内
    /// </summary>
    private bool IsInView (Vector3 point)
    {
        var res = _mFrustum * new Vector4( point.x, point.y, point.z, 1 );
        return (res.x <= res.w) && (res.x >= res.w * -1) &&
               (res.y <= res.w) && (res.y >= res.w * -1) &&
               (res.z <= res.w) && (res.z >= res.w * -1);
        
        //return (res.x >= res.w) && (res.x <= res.w * -1) &&
        //       (res.y >= res.w) && (res.y <= res.w * -1) &&
        //       (res.z >= res.w) && (res.z <= res.w * -1);
    }

    private Camera _camera;

    //六个剪裁面：[0] = 左、[1] = 右、[2] = 下、[3] = 上、[4] = 近、[5] = 远
    private Plane[] _planes;

    /// <summary>
    /// 近裁平面
    /// </summary>
    private float _nearClipPlaneHeight;

    /// <summary>
    /// 远裁平面
    /// </summary>
    private float _farClipPlaneHeight;

    /// <summary>
    /// 投影矩阵
    /// </summary>
    private Matrix4x4 _mFrustum;

    /// <summary>
    /// 相机空间变换矩阵，将物体坐标从世界空间变换到相机空间
    /// </summary>
    private Matrix4x4 _cameraSpaceMatx;

    private Transform _clipedTrans;
    private List<Transform> _cullingObjects = new List<Transform>();
    private Dictionary<Transform, MeshRenderer> _renderCache = new Dictionary<Transform, MeshRenderer>();

    private bool _isInView = true;
    private MeshRenderer _currRender = null;
}
