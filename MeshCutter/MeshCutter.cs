using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{
    /// <summary>
    /// 初始化
    /// </summary>
    private void Start()
    {
        go_to_cut = null;
        _cam = GetComponent<Camera>();
        if ( _cam == null )
            Debug.LogError( "faild to get camera!!" );

        _line_render = GetComponent<LineRenderer>();
        if ( _line_render == null )
            Debug.LogError( "faild to get lineRender!" );

        if ( go_template == null )
            Debug.LogError( "mesh to cut is null!!" );
    }

    private void Update()
    {
        InputListening();
    }

    /// <summary>
    /// 输入监听
    /// </summary>
    private void InputListening()
    {
        if ( Input.GetKeyDown( KeyCode.R ) )
        {
            if ( go_to_cut != null )
            {
                GameObject.Destroy( go_to_cut );
                go_to_cut = null;
            }

            go_to_cut = GameObject.Instantiate( go_to_cut );
            go_to_cut.transform.position = go_template.transform.position;
            go_to_cut.transform.rotation = go_template.transform.rotation;
            go_to_cut.transform.localScale = go_template.transform.localScale;
            go_to_cut.SetActive( true );
        }

        if ( Input.GetMouseButtonDown( 0 ) )
        {
            is_dragging = true;
            _start_initted = true;
            start_view_pos = _cam.ScreenToViewportPoint( Input.mousePosition );
        }

        if ( Input.GetMouseButtonUp( 0 ) )
        {
            is_dragging = false;
            //起始点已经确定
            if ( _start_initted )
            {
                _start_initted = false;

                end_view_pos = _cam.ScreenToViewportPoint( Input.mousePosition );
                GenRay( start_view_pos, end_view_pos );
                GetCutDirection( end_ray );
                DrawViewSpaceLine( start_ray, end_ray );
                _plane_normal = GetCutterDirection( start_view_pos, end_view_pos, cut_direction );
                Cut();
            }
        }
    }

    /// <summary>
    /// 切割
    /// </summary>
    private void Cut()
    {
        ResetAll();
        Vector3 end_point_ws;
        if ( go_to_cut == null )
        {
            Debug.LogError( "go_to_cut is null!" );
            return;
        }

        var filter = go_to_cut.GetComponent<MeshFilter>();
        if ( filter == null )
        {
            Debug.LogError( "render == null" );
            return;
        }

        //取得模型网格
        var mesh = filter.mesh;
        //遍历所有顶点，将顶点变换到世界空间or将切割方向变换到模型空间
        //先变换滑动的结束位置，视口坐标变换到世界空间，然后转到模型空间，跟所有顶点做计算
        end_point_ws = _cam.ViewportToWorldPoint( end_view_pos );
        //end_point_os = go_to_cut.transform.worldToLocalMatrix * end_point_os;

        var triangles = mesh.triangles;
        var tLen = triangles.Length;
        //顶点的世界空间位置索引
        Vector3 vertex_ws_pos;
        Vector3 vertex;
        //这里的思路其实是遍历所有的模型顶点，将所有顶点区分为左边和右边（如果有切割面的话）
        //然后计算切割面和物体的交点以生成新的面
        for ( var i = 0; i < tLen; i++ )
        {
            vertex = mesh.vertices[triangles[i]];
            vertex_ws_pos = OS2WS( go_to_cut.transform, vertex );
            if ( IsLeft( vertex_ws_pos ) )
            {
                //#todo_UV
                _positiveMeshList.Add( vertex );
                _positiveVertIndexList.Add( triangles[i] );
            }
            else
            {
                _negativeMeshList.Add( vertex );
                _negativeVertIndexList.Add( triangles[i] );
            }
        }


    }

    private bool IsLeft( Vector3 vertex2EndDir )
    {
        return Vector3.Dot( _plane_normal, vertex2EndDir ) > 0f;
    }

    private void ResetAll()
    {
        _positiveMeshList.Clear();
        _negativeMeshList.Clear();

        _positiveUVList.Clear();
        _negativeUVList.Clear();

        _positiveVertIndexList.Clear();
        _negativeVertIndexList.Clear();

        _new_mesh.Clear();
        if ( _new_go != null )
        {
            GameObject.Destroy( _new_go );
            _new_go = null;
        }

    }

    /// <summary>
    /// 模型空间转世界空间
    /// </summary>
    private Vector3 OS2WS( Transform tran, Vector3 p )
    {
        return tran.localToWorldMatrix * p;
    }

    /// <summary>
    /// 根据起点和重点确定切割方向，思路是让两点形成线，切割方向为终点的方向，返回世界空间坐标
    /// </summary>
    private Vector3 GetCutterDirection( Vector3 start, Vector3 end, Vector3 bioNormal )
    {
        var plane_tangent = ( start - end ).normalized;
        var plane_normal = Vector3.Cross( plane_tangent, bioNormal );
        return _cam.ViewportToWorldPoint( plane_normal ).normalized;
    }

    /// <summary>
    /// 生成视口到相机近裁面的射线
    /// </summary>
    private void GenRay( Vector3 start, Vector3 end )
    {
        start_ray = _cam.ViewportPointToRay( start );
        end_ray = _cam.ViewportPointToRay( end );
    }

    /// <summary>
    /// 获取射线方向
    /// </summary>
    private void GetCutDirection( Ray end )
    {
        cut_direction = end.direction;
    }

    /// <summary>
    /// 在屏幕上画线
    /// </summary>
    private void DrawViewSpaceLine( Ray start, Ray end )
    {
        var start_point = start.GetPoint( _cam.nearClipPlane );
        var end_point = end.GetPoint( _cam.nearClipPlane );

        cut_direction = end_ray.direction;

        _line_render.SetPosition( 0, start_point );
        _line_render.SetPosition( 1, end_point );

    }

    private GameObject _new_go = null;
    private Mesh _new_mesh = new Mesh();

    //顶点位置
    private List<Vector3> _positiveMeshList = new List<Vector3>( 1 << 1 );
    private List<Vector3> _negativeMeshList = new List<Vector3>( 1 << 1 );
    //对应的uv
    private List<Vector2> _positiveUVList = new List<Vector2>( 1 << 1 );
    private List<Vector2> _negativeUVList = new List<Vector2>( 1 << 1 );
    //顶点在mesh中的索引
    private List<int> _positiveVertIndexList = new List<int>( 1 << 1 );
    private List<int> _negativeVertIndexList = new List<int>( 1 << 1 );

    /// <summary>
    /// 要切割的mesh
    /// </summary>
    [SerializeField] private GameObject go_template = null;
    private GameObject go_to_cut = null;

    /// <summary>
    /// 切割平面法线
    /// </summary>
    private Vector3 _plane_normal = Vector3.zero;

    /// <summary>
    /// 视口到相机近裁面某点的射线
    /// </summary>
    private Ray start_ray;

    /// <summary>
    /// 视口到相机近裁面某点的射线
    /// </summary>
    private Ray end_ray;

    /// <summary>
    /// 切割结束点
    /// </summary>
    private Vector3 end_view_pos = Vector3.zero;

    /// <summary>
    /// 切割方向
    /// </summary>
    private Vector3 cut_direction = Vector3.zero;

    /// <summary>
    /// 切割的起始点
    /// </summary>
    private Vector3 start_view_pos = Vector3.zero;
    private bool _start_initted = false;

    private bool is_dragging = false;

    private Camera _cam = null;
    private LineRenderer _line_render = null;
}
