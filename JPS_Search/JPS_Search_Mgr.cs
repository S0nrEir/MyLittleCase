using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JPS寻路管理器
/// </summary>
public class JPS_Search_Mgr
{
    /// <summary>
    /// JPS跳点搜索，返回一条从start到target的路径
    /// </summary>
    public List<JPS_Node> JPS ( JPS_Node star, JPS_Node target )
    {
        return null;
    }

    /// <summary>
    /// AStar
    /// </summary>
    public List<JPS_Node> AStar ( JPS_Node start, JPS_Node target )
    {
        Reset();
        List<JPS_Node> realPathList = new List<JPS_Node>();
        JPS_Node curr = start;
        JPS_Node[] neibs = null;
        AddToOpen( curr );
        while (_openSet.Count != 0)
        {
            curr = GetClosetInOpen( start, target );
            if (curr.ID == target.ID)
                return Gen( target );
            else
            {
                RemoveFromOpen( curr );
                AddToCloseDic( curr );
            }

            //周围点
            neibs = GetNeib( curr );
            foreach (var p in neibs)
            {
                if (p is null || p.IsObs || ContainsInCloseDic( p ))
                    continue;

                if (!ContainsInOpenDic( p ))
                {
                    p.Parent = curr;
                    AddToOpen( p );
                }
                else
                {
                    p.Parent = curr;
                }
            }
        }
        return null;
    }

    private List<JPS_Node> Gen ( JPS_Node node )
    {
        var list = new List<JPS_Node>();
        while (node != null)
        {
            list.Add( node );
            node = node.Parent;
        }
        return list;
    }

    private JPS_Node[] GetNeib ( JPS_Node node )
    {
        JPS_Node[] arr = new JPS_Node[_defaultWays.Length];
        var ins = JPS_Entrance.I;
        for (var i = 0; i < _defaultWays.Length; i++)
            arr[i] = ins.Get( node.X + _defaultWays[i].Item1, node.Y + _defaultWays[i].Item2 );

        return arr;
    }

    private void RemoveFromOpen ( JPS_Node node )
    {
        _openSet.Remove( node );
    }

    private void AddToOpen (JPS_Node node)
    {
        if (!_openSet.Contains( node ))
            _openSet.Add( node );
    }

    private JPS_Node GetClosetInOpen ( JPS_Node start, JPS_Node target )
    {
        double dis1, dis2;
        JPS_Node node = null;
        foreach (var p in _openSet)
        {
            if (node is null)
            {
                node = p;
                continue;
            }

            dis1 = EuclideanDistance( start, p ) + EuclideanDistance( p, target );
            dis2 = EuclideanDistance( start, node ) + EuclideanDistance( node, target );

            if (dis1 < dis2)
                node = p;
        }

        return node;
    }

    /// <summary>
    /// 添加到关闭列表
    /// </summary>
    private void AddToCloseDic (JPS_Node node)
    {
        if (_closeDic.ContainsKey( node.ID ))
            return;

        _closeDic.Add( node.ID , node);
    }

    private bool ContainsInCloseDic ( JPS_Node node ) => _closeDic.ContainsKey( node.ID );
    private bool ContainsInOpenDic ( JPS_Node node ) => _openSet.Contains( node );


    private void EnsureInit ()
    {
        if (_closeDic is null)
            _closeDic = new Dictionary<int, JPS_Node>();

        if (_openList is null)
            _openList = new List<JPS_Node>();

        if (_pathList is null)
            _pathList = new List<JPS_Node>();

        if (_openSet is null)
            _openSet = new HashSet<JPS_Node>();
    }

    /// <summary>
    /// 两点间的欧氏距离
    /// </summary>
    private double EuclideanDistance(JPS_Node p,JPS_Node pp)
    {
        var dx = Mathf.Abs( p.X - pp.X );
        var dy = Mathf.Abs( p.Y - pp.Y );
        return Mathf.Sqrt( dx * dx + dy * dy );
    }

    private void Reset ()
    {
        _closeDic.Clear();
        _openList.Clear();
        _pathList.Clear();
        _openSet.Clear();
    }

    private Dictionary<int, JPS_Node> _closeDic = null;
    private List<JPS_Node> _openList = null;//no use
    private HashSet<JPS_Node> _openSet = null;
    private List<JPS_Node> _pathList = null;

    private static JPS_Search_Mgr _i = null;

    public static JPS_Search_Mgr I
    {
        get
        {
            if (_i is null)
            {
                _i = new JPS_Search_Mgr();
                _i.EnsureInit();
            }

            var tupl = new Tuple<int, int>( 0, 0 );

            return _i;
        }
    }

    private static (int, int)[] _defaultWays = new (int, int)[]
        {
            (0,1),//👆
            (0,-1),//👇
            (-1,0),//👈
            (1,0),//👉
        };

    /// <summary>
    /// 默认四向通行
    /// </summary>
    private const int DEFAULT_FINDING_WAYS = 0x4;

}
