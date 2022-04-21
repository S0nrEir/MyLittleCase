using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 参数化曲线路径
/// </summary>
public class CurveGenerator : MonoBehaviour
{
    //参数化曲线路径生成的基本思路：
    //对于任意两点起点和终点。指定起点和终点的的位置和切线方向，在其间生成segement个中间点，
    //对于点i，其切线方向由其前后两个点的位置决定，这样就得到了点的切线方向（曲线方向）和位置。
    //每个点都如此带入，并且
    private void Start ()
    {
        _lineRender = GetComponent<LineRenderer>();
        if (_lineRender == null)
        {
            Debug.LogError( "<color=red>faild to get line render !!!</color>" );
            return;
        }

        if (_extremPointList == null || _extremPointList.Count == 0)
        {
            Debug.LogError("<color=red>extrem point list is null or count is 0!!!</color>");
            return;
        }


    }

    /// <summary>
    /// 将一条曲线等长分段
    /// </summary>
    //private List<Tuple<float, float>> GetEqualLengthToSegment ()
    //{
    //    var segmentList = new List<Tuple<float, float>>();
    //    //=5 每段0.2
    //    float time_i = 1f / SEGMENT_COUNT;
    //    for (var i = 0; i <= SEGMENT_COUNT; i++)
    //    {
    //        float length_i = L * (1f * i / SEGMENT_COUNT);//0.2...0.4...0.6
    //        if (i == 0)
    //            //第一个
    //            segmentList.Add( new Tuple<float, float>( length_i, 0f ) );
    //        else if (i == SEGMENT_COUNT)
    //            //最后一个
    //            segmentList.Add( new Tuple<float, float>( length_i, 1f ) );
    //        else
    //        {
    //            //牛顿法求解函数,将曲线分成N个等长线段

    //        }
    //    }
    //}




    private LineRenderer _lineRender;

    /// <summary>
    /// 起始点切线方向
    /// </summary>
    [SerializeField] private Vector3 _startPointTangent;

    /// <summary>
    /// 终点参数方向
    /// </summary>
    [SerializeField] private Vector3 _endPointTangent;

    /// <summary>
    /// 关键端点集合，第一个为头，第二个为尾
    /// </summary>
    [SerializeField] private List<Transform> _extremPointList;

    /// <summary>
    /// 预设的划分段数
    /// </summary>
    public const int SEGMENT_COUNT = 0x5;

    /// <summary>
    /// 生成的临时对象
    /// </summary>
    [SerializeField] private GameObject _spawnTempObject;

    /// <summary>
    /// 生成的模板item
    /// </summary>
    [SerializeField] private GameObject _templateObject;

    /// <summary>
    /// 曲线长度
    /// </summary>
    private float L = 100f;
    
    /// <summary>
    /// 等长分段的长度
    /// </summary>
    //private float _perLineLength = 0.5f;

    

}
