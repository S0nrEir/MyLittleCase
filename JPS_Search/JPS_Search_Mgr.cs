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
        public List<JPS_Node> JPS_New ( JPS_Node start, JPS_Node target )
        {
            Reset();
            _start = start;
            _target = target;
            JPS_Node currJP = start;
            currJP.SetJumpPoint( true );
            AddToOpen( currJP );
            List<(int x, int y)> dirList = null;
            var jpsTempList = new List<JPS_Node>();
            //斜向探测的坐标点
            JPS_Node parent = null;
            //直线探测的坐标点
            JPS_Node current = null;

            while (_openSet.Count != 0)
            {
                currJP = GetClosetInOpen( start, target );
                
                RemoveFromOpen( currJP );
                AddToCloseDic( currJP );

                if (currJP.ID == target.ID)
                {
                    return JPS_Gen( currJP );
                }
                else
                {
                    dirList = DirList( currJP );
                    parent = currJP; ;
                    //初始点直线探测然后斜向探测到地图边界或遇到障碍
                    foreach (var dir in dirList)
                    {
                        if (JPS_Tools.GetTileScaneDir( dir ) == TileScanDirection.Straight)
                            JPS_Tools.GetStraightLineJPs( currJP, dir, jpsTempList, target );
                        else
                        {
                            parent = JPS_Entrance.I.Get( parent.X + dir.x, parent.X + dir.y );
                            //斜向探测
                            while (parent != null && !parent.IsObs)
                            {
                                parent.AddDir( dir );
                                JPS_Tools.GetBiasStraightLineJPs( parent, new Vector2Int( dir.x, dir.y ), target, jpsTempList );
                            }
                        }
                    }
                    AddToOpen( jpsTempList );
                    jpsTempList.Clear();
                }
            }

            return null;
        }

        /// <summary>
        /// 获取一个node的dirlist，如果是起点则为八方向
        /// </summary>
        private List<(int x, int y)> DirList ( JPS_Node node )
        {
            if (node.ID == _start.ID)
                return new List<(int x, int y)>() 
                {
                    TILE_DIRECTION.DIRECTION_UP,
                    TILE_DIRECTION.DIRECTION_DOWN,
                    TILE_DIRECTION.DIRECTION_RIGHT,
                    TILE_DIRECTION.DIRECTION_LEFT,
                    //斜向
                    TILE_DIRECTION.DIRECTION_RIGHT_UP,
                    TILE_DIRECTION.DIRECTION_LEFT_UP,
                    TILE_DIRECTION.DIRECTION_LEFT_DOWN,
                    TILE_DIRECTION.DIRECTION_RIGHT_DOWN,
                };

            return node._dirList;
        }

        #region old_jps
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

                if(curr.ID != start.ID && curr.ID != target.ID)
                    JPS_Entrance.I.SetJPTile_Test( curr );

                if (curr.ID == target.ID)
                {
                    return JPS_Gen( curr );
                }
                else
                {
                    RemoveFromOpen( curr );
                    AddToCloseDic( curr );
                }

                //当前点水平四方向的跳点探索
                JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_UP,    temp_jp_list,target );
                JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_DOWN,  temp_jp_list,target );
                JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_LEFT,  temp_jp_list,target );
                JPS_Tools.GetStraightLineJPs( curr, TILE_DIRECTION.DIRECTION_RIGHT, temp_jp_list,target );

                //尝试优化一下，如果这次遍历直线方向找到了跳点，那就不进行斜向探测
                if (temp_jp_list.Count == 0)
                {
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
                                    target,
                                    temp_jp_list
                                )
                            );
                        //如果找到任何一个，直接跳出
                        if (temp_jp_list.Count != 0)
                            i = _biasWays.Length - 1;

                    }//end bias for   
                }

                foreach (var jp in temp_jp_list)
                {
                    if (jp is null || ContainsInCloseDic( jp ))
                        continue;
                    
                    jp.SetJumpPoint( true );
                    if (!ContainsInOpenDic( jp ))
                        AddToOpen( jp );

                    jp.Parent = curr;
                    if (jp.ID == target.ID)
                        return JPS_Gen( jp );
                }
                temp_jp_list.Clear();

            }
            
            return null;
        }
        #endregion

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

        /// <summary>
        /// 生成一条jps寻路路径，没有返回空
        /// </summary>
        private List<JPS_Node> JPS_Gen ( JPS_Node node )
        {
            //for test
            return new List<JPS_Node>(0);

            var list = new List<JPS_Node>();
            while (node != null)
            {
                list.Add( node );
                node = node.Parent;
            }
            return list;
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

        private void AddToOpen ( List<JPS_Node> nodeList )
        {
            if (nodeList is null || nodeList.Count == 0)
                return;

            foreach (var node in nodeList)
            {
                if (ContainsInCloseDic( node ))
                    continue;

                AddToOpen( node );
            }
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

        public bool ContainsInCloseDic ( JPS_Node node ) => _closeDic.ContainsKey( node.ID );
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
        private JPS_Node _start = null;
        private JPS_Node _target = null;

        public static JPS_Search_Mgr I
        {
            get
            {
                if (_i is null)
                {
                    _i = new JPS_Search_Mgr();
                    _i.EnsureInit();
                }

                return _i;
            }
        }

        private static (int, int)[] _defaultWays = new (int, int)[]
            {
                TILE_DIRECTION.DIRECTION_UP,//👆
                TILE_DIRECTION.DIRECTION_DOWN,//👇
                TILE_DIRECTION.DIRECTION_LEFT,//👈
                TILE_DIRECTION.DIRECTION_RIGHT,//👉
            };

        private static (int, int)[] _default_8_Ways = new (int, int)[]
            {
                TILE_DIRECTION.DIRECTION_UP,//👆
                TILE_DIRECTION.DIRECTION_DOWN,//👇
                TILE_DIRECTION.DIRECTION_LEFT,//👈
                TILE_DIRECTION.DIRECTION_RIGHT,//👉
                TILE_DIRECTION.DIRECTION_LEFT_UP,//↖
                TILE_DIRECTION.DIRECTION_LEFT_DOWN,//↙
                TILE_DIRECTION.DIRECTION_RIGHT_DOWN,//↘
                TILE_DIRECTION.DIRECTION_RIGHT_UP,//↗
            };

        private static (int x, int y)[] _biasWays = new (int, int)[]
            {
                TILE_DIRECTION.DIRECTION_LEFT_UP,//↖
                TILE_DIRECTION.DIRECTION_LEFT_DOWN,//↙
                TILE_DIRECTION.DIRECTION_RIGHT_DOWN,//↘
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
        public static readonly (int x, int y) DIRECTION_DOWN = (0, -1);
        public static readonly (int x, int y) DIRECTION_LEFT = (-1, 0);
        public static readonly (int x, int y) DIRECTION_RIGHT = (1, 0);
        //斜向四方向
        public static readonly (int x, int y) DIRECTION_RIGHT_UP = (1, 1);
        public static readonly (int x, int y) DIRECTION_RIGHT_DOWN = (1, -1);
        public static readonly (int x, int y) DIRECTION_LEFT_UP = (-1, 1);
        public static readonly (int x, int y) DIRECTION_LEFT_DOWN = (-1, -1);


    }

    //方向枚举
    public enum TileDirectionEnum
    {
        Up,
        Down,
        Left,
        Right,
        RightUp,
        LeftUp,
        LeftDown,
        RightDown,
        None
    }

    public enum TileScanDirection
    {
        Straight,
        Bias,
        None
    }
}
