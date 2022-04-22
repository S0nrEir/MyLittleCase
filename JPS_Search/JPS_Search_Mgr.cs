using JPS;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{

    /// <summary>
    /// JPS寻路管理器
    /// </summary>
    public class JPS_Search_Mgr
    {
        /// <summary>
        /// JPS跳点搜索，返回一条从start到target的路径，八方向
        /// </summary>
        public List<JPS_Node> JPS ( JPS_Node start, JPS_Node target )
        {
            //对于四方向，只进行直线遍历
            //八方向增加四条斜线
            Reset();
            JPS_Node curr;
            AddToOpen( start );
            //保存拿到的跳点集合
            List<JPS_Node> temp_jp_list = new List<JPS_Node>();

            while (_openSet.Count != 0)
            {
                curr = GetClosetInOpen( start, target );
                JPS_Entrance.I.SetJPTile_Test( curr );
                if (curr.ID == target.ID)
                {
                    return Gen( curr );
                }
                else
                {
                    RemoveFromOpen( curr );
                    AddToCloseDic( curr );
                }

                //当前点四方向
                temp_jp_list.AddRange( JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_UP ) );
                temp_jp_list.AddRange( JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_Down ) );
                temp_jp_list.AddRange( JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_Right ) );
                temp_jp_list.AddRange( JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_Left ) );

                for (var i = 0; i < _biasWays.Length; i++)
                {
                    //当前跳点为起点的 所有新跳点的集合
                    temp_jp_list.AddRange
                        ( 
                            JPS_Tools.GetBiasStraightLineJPs
                            (
                                //curr, 
                                JPS_Entrance.I.Get( curr.X + _biasWays[i].x, curr.Y + _biasWays[i].y ),
                                new Vector2Int( _biasWays[i].x, _biasWays[i].y ),
                                null
                            ) 
                        );

                    foreach (var jp in temp_jp_list)
                    {
                        if (ContainsInCloseDic( jp ))
                            continue;

                        jp.SetJumpPoint( true );
                        if (!ContainsInOpenDic( jp ))
                            AddToOpen( jp );

                        jp.Parent = curr;
                    }
                    temp_jp_list.Clear();
                }//end bias for
            }
            
            return null;
        }


        /// <summary>
        /// AStar
        /// </summary>
        public List<JPS_Node> AStar ( JPS_Node start, JPS_Node target )
        {
            Reset();
            JPS_Node curr = start;
            JPS_Node[] neibs;
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
                        AddToOpen( p );
                    }
                    //else
                    //{
                    //    p.Parent = curr;
                    //}
                    p.Parent = curr;
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

        private void AddToOpen ( JPS_Node node )
        {
            if (!_openSet.Contains( node ))
                _openSet.Add( node );
        }

        private JPS_Node GetClosetInOpen ( JPS_Node start, JPS_Node target )
        {
            //#todo优化 复杂地形跳点也很多，遍历取出寻路代价最低的跳点效率太慢了
            double dis1, dis2;
            JPS_Node node = null;
            foreach (var p in _openSet)
            {
                if (node is null)
                {
                    node = p;
                    continue;
                }

                dis1 = JPS_Tools.EuclideanDistance( start, p ) + JPS_Tools.EuclideanDistance( p, target );
                dis2 = JPS_Tools.EuclideanDistance( start, node ) + JPS_Tools.EuclideanDistance( node, target );
                node = dis1 < dis2 ? p : node;

                //if (dis1 < dis2)
                //    node = p;
            }

            return node;
        }

        /// <summary>
        /// 添加到关闭列表
        /// </summary>
        private void AddToCloseDic ( JPS_Node node )
        {
            if (_closeDic.ContainsKey( node.ID ))
                return;

            _closeDic.Add( node.ID, node );
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

            if (_jpsList is null)
                _jpsList = new List<JPS_Node>();
        }

        private void Reset ()
        {
            _closeDic.Clear();
            _openList.Clear();
            _pathList.Clear();
            _openSet.Clear();
            _jpsList.Clear();
        }

        private Dictionary<int, JPS_Node> _closeDic = null;
        private List<JPS_Node> _openList = null;//no use
        private HashSet<JPS_Node> _openSet = null;
        private List<JPS_Node> _pathList = null;
        private List<JPS_Node> _jpsList = null;//跳点列表

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
                TILE_DIRECTION.DIRECTION_UP,//👆
                TILE_DIRECTION.DIRECTION_Down,//👇
                TILE_DIRECTION.DIRECTION_Left,//👈
                TILE_DIRECTION.DIRECTION_Right,//👉
            };

        private static (int, int)[] _default_8_Ways = new (int, int)[]
            {
                TILE_DIRECTION.DIRECTION_UP,//👆
                TILE_DIRECTION.DIRECTION_Down,//👇
                TILE_DIRECTION.DIRECTION_Left,//👈
                TILE_DIRECTION.DIRECTION_Right,//👉
                TILE_DIRECTION.DIRECTION_LEFT_UP,//↖
                TILE_DIRECTION.DIRECTION_LEFT_DOWN,//↙
                TILE_DIRECTION.DIRECTION_RIGHT_Down,//↘
                TILE_DIRECTION.DIRECTION_RIGHT_UP,//↗
            };

        private static (int x, int y)[] _biasWays = new (int, int)[]
            {
                TILE_DIRECTION.DIRECTION_LEFT_UP,//↖
                TILE_DIRECTION.DIRECTION_LEFT_DOWN,//↙
                TILE_DIRECTION.DIRECTION_RIGHT_Down,//↘
                TILE_DIRECTION.DIRECTION_RIGHT_UP,//↗
            };



        /// <summary>
        /// 默认四向通行
        /// </summary>
        private const int DEFAULT_FINDING_WAYS = 0x4;

    }

    public class TILE_DIRECTION
    {
        //水平四方向
        public static readonly (int x, int y) DIRECTION_UP = (0, 1);
        public static readonly (int x, int y) DIRECTION_Down = (0, -1);
        public static readonly (int x, int y) DIRECTION_Left = (-1, 0);
        public static readonly (int x, int y) DIRECTION_Right = (1, 0);
        //斜向四方向
        public static readonly (int x, int y) DIRECTION_RIGHT_UP = (1, 1);
        public static readonly (int x, int y) DIRECTION_RIGHT_Down = (1, -1);
        public static readonly (int x, int y) DIRECTION_LEFT_UP = (-1, 1);
        public static readonly (int x, int y) DIRECTION_LEFT_DOWN = (-1, -1);
    }
}
