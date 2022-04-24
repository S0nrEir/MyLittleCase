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
        public List<JPS_Node> JPS_New_New ( JPS_Node start, JPS_Node target )
        {
            Reset();
            _start = start;
            _target = target;
            JPS_Node currJP = start;
            currJP.SetJumpPoint( true );
            AddToOpen( currJP );
            List<(int x, int y)> dirList = null;
            var jpsTempList = new List<JPS_Node>();
            JPS_Node parent = null;
            while (_openSet.Count != 0)
            {
                currJP = GetClosetInOpen( start, target );
                JPS_Entrance.I.SetJPTile_Test( currJP );
                //for test
                if (currJP.ID != start.ID && currJP.ID != target.ID)
                    JPS_Entrance.I.SetJPTile_Test( currJP );

                if (currJP.ID == target.ID)
                    return JPS_Gen( currJP );
                else
                {
                    RemoveFromOpen( currJP );
                    AddToCloseDic( currJP );
                    dirList = DirList( currJP );
                    foreach (var dir in dirList)
                    {
                        //直线
                        if (JPS_Tools.GetTileScaneDir( dir ) == TileScanDirection.Straight)
                        {
                            //直线方向遇到邻接障碍物，该点作为跳点，自身不作为跳点
                            GetStraightLineJP( jpsTempList, currJP, new Vector2Int( dir.x, dir.y ),false );
                        }
                        //斜向
                        else
                        {
                            parent = currJP;
                            while (parent != null && !parent.IsObs)
                            {
                                Debug.Log( $"<color=purple>parent:{parent},biasDirection{dir}</color>" );
                                GetBiasLineJP( jpsTempList, parent, new Vector2Int( dir.x, dir.y ));
                                parent = JPS_Entrance.I.Get( parent.X + dir.x, parent.Y + dir.y );
                            }
                        }
                    }
                    AddToOpen( jpsTempList );
                    jpsTempList.Clear();
                }
            }
            return null;
        }


        private void GetStraightLineJP (List<JPS_Node> jpList,JPS_Node node,Vector2Int direction,bool AddSelf)
        {
            if (jpList is null)
                jpList = new List<JPS_Node>();

            if (direction == Vector2Int.zero)
                return;

            var temp = JPS_Entrance.I.Get( node.X + direction.x, node.Y + direction.y );
            Vector2Int[] forceNeibArr;
            while (temp != null && !temp.IsObs)
            {
                if (HasForceNeiborPoint( temp, direction, out forceNeibArr ))
                {
                    if (forceNeibArr != null && forceNeibArr.Length != 0)
                    {
                        foreach (var neibDir in forceNeibArr)
                        {
                            if (neibDir == Vector2Int.zero)
                                continue;

                            temp.AddDir( neibDir );
                        }
                    }
                    jpList.Add( temp );

                    if (AddSelf)
                        jpList.Add( node );

                    break;
                }
                else
                {
                    temp = JPS_Entrance.I.Get( temp.X + direction.x, temp.Y + direction.y );
                }
            }
        }

        private void GetBiasLineJP ( List<JPS_Node> jpList, JPS_Node node, Vector2Int biasDirection )
        {
            //分解方向
            if (JPS_Tools.GetTileScaneDir( biasDirection ) != TileScanDirection.Bias)
                return;

            if (jpList is null)
                jpList = new List<JPS_Node>();

            var decompedDir = DecompBiasDirection( biasDirection );
            if (decompedDir is null && decompedDir.Length == 0)
                return;

            _biasCacheList.Clear();
            //斜向分解成直线，检查自身
            foreach (var straightDir in decompedDir)
            {
                Debug.Log( $"<color=puple>decomp direction:{straightDir}</color>" );
                GetStraightLineJP( _biasCacheList, node, straightDir ,true);
            }

            jpList.AddRange( _biasCacheList );
        }

        private List<JPS_Node> _biasCacheList = null;

        /// <summary>
        /// 分解一个斜向
        /// </summary>
        private Vector2Int[] DecompBiasDirection (Vector2Int biasDirection)
        {
            if (biasDirection == Vector2Int.zero)
                return new Vector2Int[0];

            //右朝向
            if (biasDirection.x > 0)
            {
                return new Vector2Int[2]
                    {
                        new Vector2Int(1,0),
                        new Vector2Int(0,biasDirection.y > 0 ? 1:-1),
                    };
            }
            else
            {
                return new Vector2Int[2]
                   {
                        new Vector2Int(-1,0),
                        new Vector2Int(0,biasDirection.y > 0 ? 1:-1),
                    };
            }
        }

        /// <summary>
        /// 当前node是否有强制临界点，是返回true，并且返回强制邻接点方向
        /// </summary>
        private bool HasForceNeiborPoint (JPS_Node node ,Vector2Int direction,out Vector2Int[] forceNeibDir)
        {
            forceNeibDir = null;
            if (node is null || node.IsObs)
                return false;

            if (node.ID == _start.ID)
                return true;

            var scanType = JPS_Tools.GetTileScaneDir( direction );
            if (scanType == TileScanDirection.None)
                return false;

            //强迫邻接点的探测方向
            Vector2Int forceNeiborDir_1 = Vector2Int.zero;
            Vector2Int forceNeiborDir_2 = Vector2Int.zero;
            Vector2Int backOffset_1 = Vector2Int.zero;
            Vector2Int backOffset_2 = Vector2Int.zero;

            //#todo优化方向处理
            if (scanType == TileScanDirection.Straight)
            {
                //horizontal
                if (direction.y == 0)
                {
                    if (direction.x > 0)
                    {
                        forceNeiborDir_1.x = 1;
                        forceNeiborDir_1.y = 1;
                        backOffset_1.x = -1;

                        forceNeiborDir_2.x = 1;
                        forceNeiborDir_2.y = -1;
                        backOffset_2.x = -1;
                    }
                    else
                    {
                        forceNeiborDir_1.x = -1;
                        forceNeiborDir_1.y = 1;
                        backOffset_1.x = 1;

                        forceNeiborDir_2.x = -1;
                        forceNeiborDir_2.y = -1;
                        backOffset_1.x = 1;
                    }
                }
                //vertical
                else
                {
                    if (direction.y > 0)
                    {
                        forceNeiborDir_1.y = 1;
                        forceNeiborDir_1.x = 1;
                        backOffset_1.y = -1;

                        forceNeiborDir_2.x = -1;
                        forceNeiborDir_2.y = 1;
                        backOffset_1.y = -1;
                    }
                    else
                    {
                        forceNeiborDir_1.y = 1;
                        forceNeiborDir_1.x = -1;
                        backOffset_1.y = 1;

                        forceNeiborDir_2.x = -1;
                        forceNeiborDir_2.y = -1;
                        backOffset_1.y = 1;
                    }
                }
            }
            //bias，现在不用
            else
            {
                //右朝向
                if (direction.x > 0)
                {
                    //右上
                    if (direction.y > 0)
                    {
                        forceNeiborDir_1.x = -1;
                        forceNeiborDir_1.y = 1;
                        backOffset_1.y = -1;

                        forceNeiborDir_2.x = 1;
                        forceNeiborDir_2.y = -1;
                        backOffset_1.x = -1;
                    }
                    //右下
                    else
                    {
                        forceNeiborDir_1.x = 1;
                        forceNeiborDir_1.y = 1;
                        backOffset_1.x = -1;

                        forceNeiborDir_2.x = -1;
                        forceNeiborDir_2.y = -1;
                        backOffset_1.y = 1;
                    }
                }
                //左朝向
                else
                {
                    //左上
                    if (direction.y > 0)
                    {
                        forceNeiborDir_1.x = 1;
                        forceNeiborDir_1.y = 1;
                        backOffset_1.y = -1;

                        forceNeiborDir_2.x = -1;
                        forceNeiborDir_2.y = -1;
                        backOffset_1.x = 1;
                    }
                    //左下
                    else
                    {
                        forceNeiborDir_1.x = -1;
                        forceNeiborDir_1.y = 1;
                        backOffset_1.x = 1;

                        forceNeiborDir_2.x = 1;
                        forceNeiborDir_2.y = -1;
                        backOffset_1.y = 1;
                    }
                }
            }

            //forceNeib
            bool hasForceNeibor = false;
            forceNeibDir = new Vector2Int[2]
                {
                    Vector2Int.zero,
                    Vector2Int.zero
                };

            Debug.Log( $"<color=orange>node:{node}___direction:{direction}</color>" );
            var tempNode = JPS_Entrance.I.Get( node.X + forceNeiborDir_1.x, node.Y + forceNeiborDir_1.y );
            if (tempNode != null && !tempNode.IsObs)
            {
                tempNode = JPS_Entrance.I.Get( tempNode.X + backOffset_1.x, tempNode.Y + backOffset_1.y );

                Debug.Log( string.Format( "<color=while>forceNeiborDir_1:{0}___is tempNode:{1}___neibOffset_1:{2}</color>", ( forceNeiborDir_1.x, forceNeiborDir_1.y), tempNode, (backOffset_1.x,backOffset_1.y) ) );

                hasForceNeibor |= tempNode != null && tempNode.IsObs;
                if (tempNode != null && tempNode.IsObs)
                {
                    forceNeibDir[0] = new Vector2Int( forceNeiborDir_1.x, forceNeiborDir_1.y );
                    hasForceNeibor = true;
                }
            }

            tempNode = JPS_Entrance.I.Get( node.X + forceNeiborDir_2.x, node.Y + forceNeiborDir_2.y );
            if (tempNode != null && !tempNode.IsObs)
            {
                tempNode = JPS_Entrance.I.Get( tempNode.X + backOffset_2.x, tempNode.Y + backOffset_2.y );

                Debug.Log( string.Format( "<color=while>forceNeiborDir_1:{0}___is tempNode:{1}___neibOffset_1:{2}</color>", (forceNeiborDir_1.x, forceNeiborDir_1.y), tempNode, (backOffset_1.x, backOffset_1.y) ) );

                hasForceNeibor |= tempNode != null && tempNode.IsObs;
                if (tempNode != null && tempNode.IsObs)
                {
                    forceNeibDir[1] = new Vector2Int( forceNeiborDir_2.x, forceNeiborDir_2.y );
                    hasForceNeibor = true;
                }
            }

            return hasForceNeibor;
        }


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
                Debug.Log( $"<color=purple>currJP:{currJP}</color>" );
                //for test
                if (currJP.ID != start.ID && currJP.ID != target.ID)
                    JPS_Entrance.I.SetJPTile_Test( currJP );

                if (currJP.ID == target.ID)
                    return JPS_Gen( currJP );
                else
                {
                    RemoveFromOpen( currJP );
                    AddToCloseDic( currJP );

                    dirList = DirList( currJP );
                    foreach (var dir in dirList)
                    {
                        //直线
                        if (JPS_Tools.GetTileScaneDir( dir ) == TileScanDirection.Straight)
                        {
                            JPS_Tools.GetStraightLineJps_New( currJP, target, new Vector2Int(dir.x,dir.y), jpsTempList );
                        }
                        //斜向
                        else
                        {
                            parent = JPS_Entrance.I.Get( currJP.X, currJP.Y, true );
                            //斜向探测，遇到边界或障碍物终止
                            while (parent != null && !parent.IsObs)
                            {
                                if (parent.ID == target.ID)
                                    return Gen( parent );

                                var tempDir = new Vector2Int( dir.x, dir.y );
                                JPS_Tools.GetBiasStraightLineJPs( parent, tempDir, target, jpsTempList );
                                parent = JPS_Entrance.I.Get( parent.X + dir.x, parent.Y + dir.y );
                            }
                        }
                    }
                    AddToOpen( jpsTempList );
                    jpsTempList.Clear();
                }

                #region nouse
                //currJP = GetClosetInOpen( start, target );
                //Debug.Log( $"<color=purple>currJP:{currJP}</color>" );
                ////for test
                //if (currJP.ID != start.ID && currJP.ID != target.ID)
                //    JPS_Entrance.I.SetJPTile_Test( currJP );

                //if (currJP.ID == target.ID)
                //    return Gen( currJP );
                //else
                //{
                //    RemoveFromOpen( currJP );
                //    AddToCloseDic( currJP );

                //    dirList = DirList( currJP );
                //    //parent = currJP;
                //    foreach (var dir in dirList)
                //    {
                //        //直线
                //        if (JPS_Tools.GetTileScaneDir( dir ) == TileScanDirection.Straight)
                //        {
                //            current = currJP;
                //            JPS_Tools.GetStraightLineJPs( current, dir, jpsTempList, target );
                //        }
                //        //斜向
                //        else
                //        {
                //            parent = JPS_Entrance.I.Get( currJP.X, currJP.Y, true );
                //            //斜向探测，遇到边界或障碍物终止
                //            while (parent != null && !parent.IsObs)
                //            {
                //                if (parent.ID == target.ID)
                //                    return Gen( parent );

                //                var tempDir = new Vector2Int( dir.x, dir.y );
                //                Debug.Log( $"<color=red>parent={parent}</color>" );
                //                JPS_Tools.GetBiasStraightLineJPs( parent, tempDir, target, jpsTempList );
                //                parent = JPS_Entrance.I.Get( parent.X + dir.x, parent.Y + dir.y );
                //            }
                //        }
                //    }
                //    AddToOpen( jpsTempList );
                //    foreach (var jp in jpsTempList)
                //    {
                //        if (jp is null || jp.IsObs || ContainsInCloseDic( jp ))
                //            continue;

                //        jp.Parent = currJP;
                //    }

                //    jpsTempList.Clear();
                //}
                #endregion
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
                    TILE_DIRECTION.DIRECTION_RIGHT_DOWN,
                    TILE_DIRECTION.DIRECTION_LEFT_UP,
                    TILE_DIRECTION.DIRECTION_LEFT_DOWN,
                };

            var s = $"<color=blUe>node__{node},dir:";
            foreach (var dir in node._dirList)
            {
                s += $"{dir.x},{dir.y} ; ";
            }
            s += "</color>";
            Debug.Log( s );
            return node._dirList;
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
                if (ContainsInCloseDic( node ) || ContainsInOpenDic(node))
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
        public bool ContainsInOpenDic ( JPS_Node node ) => _openSet.Contains( node );


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

            if (_biasCacheList is null)
                _biasCacheList = new List<JPS_Node>();
        }

        private void Reset ()
        {
            _closeDic.Clear();
            _openList.Clear();
            _pathList.Clear();
            _openSet.Clear();
            _biasCacheList.Clear();

            _start = null;
            _target = null;
        }

        private Dictionary<int, JPS_Node> _closeDic = null;
        private List<JPS_Node> _openList = null;//no use
        private HashSet<JPS_Node> _openSet = null;
        private List<JPS_Node> _pathList = null;

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
