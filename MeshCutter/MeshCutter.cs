using System.Collections.Generic;
using UnityEngine;

public class MeshCutter : MonoBehaviour
{

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
        //���ɷ���
        if ( Input.GetKeyDown( KeyCode.R ) )
        {
            if ( go_to_cut != null )
            {
                GameObject.Destroy( go_to_cut );
                go_to_cut = null;
            }

            go_to_cut = GameObject.Instantiate( go_template, null );
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
            //��ʼ���Ѿ�ȷ��
            if ( _start_initted )
            {
                _start_initted = false;

                end_view_pos = _cam.ScreenToViewportPoint( Input.mousePosition );
                GenRay( start_view_pos, end_view_pos );
                GetCutDirection( end_ray );
                DrawViewSpaceLine( start_ray, end_ray );
                GetCutterDirection( start_view_pos, end_view_pos, cut_direction );
            }
        }
    }

    private void Cut()
    {
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

        //ȡ��ģ������
        var mesh = filter.mesh;
        //�������ж��㣬������任������ռ�or���и��任��ģ�Ϳռ�

    }

    /// <summary>
    /// ���������ص�ȷ���и��˼·���������γ��ߣ��и��Ϊ�յ�ķ���
    /// </summary>
    private Vector3 GetCutterDirection( Vector3 start, Vector3 end, Vector3 bioNormal )
    {
        var plane_tangent = ( start - end ).normalized;
        var plane_normal = Vector3.Cross( plane_tangent, bioNormal );
        return plane_normal.normalized;
    }

    /// <summary>
    /// �����ӿڵ���������������
    /// </summary>
    private void GenRay( Vector3 start, Vector3 end )
    {
        start_ray = _cam.ViewportPointToRay( start );
        end_ray = _cam.ViewportPointToRay( end );
    }

    /// <summary>
    /// ��ȡ���߷���
    /// </summary>
    private void GetCutDirection( Ray end )
    {
        cut_direction = end.direction;
    }

    /// <summary>
    /// ����Ļ�ϻ���
    /// </summary>
    private void DrawViewSpaceLine( Ray start, Ray end )
    {
        var start_point = start.GetPoint( _cam.nearClipPlane );
        var end_point = end.GetPoint( _cam.nearClipPlane );

        cut_direction = end_ray.direction;

        _line_render.SetPosition( 0, start_point );
        _line_render.SetPosition( 1, end_point );

    }

    /// <summary>
    /// Ҫ�и��mesh
    /// </summary>
    [SerializeField] private GameObject go_template = null;
    private GameObject go_to_cut = null;

    /// <summary>
    /// �ӿڵ����������ĳ�������
    /// </summary>
    private Ray start_ray;

    /// <summary>
    /// �ӿڵ����������ĳ�������
    /// </summary>
    private Ray end_ray;

    /// <summary>
    /// �и������
    /// </summary>
    private Vector3 end_view_pos = Vector3.zero;

    /// <summary>
    /// �и��
    /// </summary>
    private Vector3 cut_direction = Vector3.zero;

    /// <summary>
    /// �и����ʼ��
    /// </summary>
    private Vector3 start_view_pos = Vector3.zero;
    private bool _start_initted = false;

    private bool is_dragging = false;

    private Camera _cam = null;
    private LineRenderer _line_render = null;
}
