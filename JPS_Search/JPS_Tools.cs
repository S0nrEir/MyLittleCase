using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{
    //jps工具类
    public static class JPS_Tools
    {
        /// <summary>
        /// 获取直线方向上的第一个跳点，如果是自身，则返回自身
        /// </summary>
        public static void GetStraightLineJps_New ( JPS_Node node, JPS_Node target, Vector2Int straightDirection, List<JPS_Node> jps_list = null )
        {
            if (jps_list is null)
                jps_list = new List<JPS_Node>();

            if (straightDirection == Vector2.zero)
                return;

            if (node is null || node.IsObs)
                return;

            if (JPS_Search_Mgr.I.ContainsInCloseDic( node ))
                return;

            if (node.ID == target.ID)
            {
                jps_list.Add( target );
                return;
            }

            if (IsJumpPoint_New( node, straightDirection, out var biasNeibDir ) && !JPS_Search_Mgr.I.ContainsInCloseDic( node ))
            {
                node.AddDir( straightDirection );
                foreach (var dir in biasNeibDir)
                {
                    if (dir == Vector2Int.zero)
                        continue;

                    node.AddDir( dir );
                }
                jps_list.Add( node );
                return;
            }

            //下面走循环一直检查
            var temp = JPS_Entrance.I.Get( node.X + straightDirection.x, node.Y + straightDirection.y );
            while (temp != null && !temp.IsObs)
            {
                if (IsJumpPoint_New( temp, straightDirection, out biasNeibDir ))
                {
                    if (JPS_Search_Mgr.I.ContainsInCloseDic( temp ))
                    {
                        temp = JPS_Entrance.I.Get( temp.X + straightDirection.x, temp.Y + straightDirection.y );
                        continue;
                    }

                    if (temp.ID == target.ID)
                    {
                        jps_list.Add( temp );
                        temp.AddDir( straightDirection );
                        return;
                    }

                    temp.AddDir( straightDirection );
                    foreach (var dir in biasNeibDir)
                    {
                        if (dir == Vector2Int.zero)
                            continue;

                        temp.AddDir( dir );
                    }
                    jps_list.Add( temp );
                    return;
                }
                temp = JPS_Entrance.I.Get( temp.X + straightDirection.x, temp.Y + straightDirection.y );        
            }
        }

        /// <summary>
        /// 检查一个点是否为跳点
        /// </summary>
        public static bool IsJumpPoint_New ( JPS_Node node, Vector2Int direction, out Vector2Int[] biasNeibDir )
        {

            biasNeibDir = new Vector2Int[2]
                {
                    Vector2Int.zero,
                    Vector2Int.zero
                };

            var scanType = GetTileScaneDir( direction );
            if (direction == Vector2Int.zero)
                return false;

            Vector2Int neibOffset_1 = new Vector2Int( 0, 0 );
            Vector2Int neibOffset_2 = new Vector2Int( 0, 0 );
            Vector2Int backOffset_1 = new Vector2Int( 0, 0 );
            Vector2Int backOffset_2 = new Vector2Int( 0, 0 );

            //水平方向上下左右
            if (scanType == TileScanDirection.Straight)
            {
                //竖向偏移在两边
                if (direction.x == 0)
                {
                    neibOffset_1.x = 1;
                    neibOffset_2.x = -1;

                    //竖向偏移在neib的下边
                    //朝上
                    if (direction.y > 0)
                    {
                        backOffset_1.y = -1;
                        backOffset_2.y = -1;
                    }
                    //朝下
                    else
                    {
                        backOffset_1.y = 1;
                        backOffset_2.y = 1;
                    }
                }
                //水平偏移在上下
                else
                {
                    neibOffset_1.y = 1;
                    neibOffset_2.y = -1;

                    //朝右,障碍检测向左（后）
                    if (direction.x > 0)
                    {
                        backOffset_1.x = -1;
                        backOffset_2.x = -1;
                    }
                    //朝左
                    else
                    {
                        backOffset_2.x = 1;
                        backOffset_1.x = 1;
                    }
                }
            }
            //斜向x和y皆不等于0
            else
            {
                //右朝向
                if (direction.x > 0)
                {
                    //右上还是右下
                    if (direction.y > 0)//右上
                    {
                        neibOffset_1.x = -1;
                        neibOffset_1.y = 1;
                        neibOffset_2.x = 1;
                        neibOffset_2.y = -1;

                        backOffset_1.y = -1;
                        backOffset_2.x = -1;
                    }
                    else//右下
                    {
                        neibOffset_1.x = 1;
                        neibOffset_1.y = 1;
                        neibOffset_2.x = -1;
                        neibOffset_2.y = -1;

                        backOffset_1.x = -1;
                        backOffset_2.y = 1;
                    }
                }
                //左朝向
                else//左上
                {
                    if (direction.y > 0)
                    {
                        neibOffset_1.x = -1;
                        neibOffset_1.y = -1;
                        neibOffset_2.x = 1;
                        neibOffset_2.y = 1;

                        backOffset_1.x = 1;
                        backOffset_2.y = -1;
                    }
                    else//左下
                    {
                        neibOffset_1.x = -1;
                        neibOffset_1.y = 1;
                        neibOffset_2.x = 1;
                        neibOffset_2.y = -1;

                        backOffset_1.x = 1;
                        backOffset_2.y = 1;
                    }
                }
            }
            var hasForceNeib = false;
            var ins = JPS_Entrance.I;
            JPS_Node neibNode = ins.Get( node.X + neibOffset_1.x, node.Y + neibOffset_1.y, true );
            if (neibNode != null && !neibNode.IsObs)
            {
                neibNode = ins.Get( neibNode.X + backOffset_1.x, neibNode.Y + backOffset_1.y, true );
                if (neibNode != null && neibNode.IsObs)
                {
                    biasNeibDir[0] = neibOffset_1;
                    hasForceNeib = true;
                }
            }

            neibNode = ins.Get( node.X + neibOffset_2.x, node.Y + neibOffset_2.y, true );
            if (neibNode != null && !neibNode.IsObs)
            {
                neibNode = ins.Get( neibNode.X + backOffset_2.x, neibNode.Y + backOffset_2.y, true );
                if (neibNode != null && neibNode.IsObs)
                {
                    biasNeibDir[1] = neibOffset_1;
                    hasForceNeib = true;
                }
            }

            return hasForceNeib;
        }

        /// <summary>
        /// 两点间的欧氏距离
        /// </summary>
        public static double EuclideanDistance ( JPS_Node p, JPS_Node pp )
        {
            var dx = Mathf.Abs( p.X - pp.X );
            var dy = Mathf.Abs( p.Y - pp.Y );
            return Mathf.Sqrt( dx * dx + dy * dy );
        }

        /// <summary>
        /// 获取某一斜向的遍历直线跳点集合
        /// </summary>
        public static List<JPS_Node> GetBiasStraightLineJPs (JPS_Node node,Vector2Int biasDirection, JPS_Node target, List<JPS_Node> temp_jp_list = null)
        {
            temp_jp_list = temp_jp_list is null ? new List<JPS_Node>() : temp_jp_list;
            if (node is null || node.IsObs)
                return temp_jp_list;

            if (biasDirection.x == 0 && biasDirection.y == 0)
                return temp_jp_list;

            if (node.ID == target.ID)
            {
                temp_jp_list.Add( node );
                return temp_jp_list;
            }

            if (biasDirection.x > 0)
            {
                //#TODO测试log
                Debug.Log( $"<color=puple>decomp direction:{TILE_DIRECTION.DIRECTION_RIGHT}</color>" );
                //GetStraightLineJPs( node, TILE_DIRECTION.DIRECTION_RIGHT, temp_jp_list, target );
                GetStraightLineJps_New( node, target, new Vector2Int( TILE_DIRECTION.DIRECTION_RIGHT.x, TILE_DIRECTION.DIRECTION_RIGHT.y), temp_jp_list );
                //GetStraightLineJPs( node, biasDirection.y > 0 ? TILE_DIRECTION.DIRECTION_UP : TILE_DIRECTION.DIRECTION_DOWN, temp_jp_list, target );
            }
            else
            {
                //#TODO测试log
                Debug.Log( $"<color=puple>decomp direction:{TILE_DIRECTION.DIRECTION_RIGHT}</color>" );
                //GetStraightLineJPs( node, TILE_DIRECTION.DIRECTION_LEFT, temp_jp_list, target );
                GetStraightLineJps_New( node, target, new Vector2Int( TILE_DIRECTION.DIRECTION_LEFT.x, TILE_DIRECTION.DIRECTION_LEFT.y ), temp_jp_list );
            }

            var log = biasDirection.y > 0 ? TILE_DIRECTION.DIRECTION_UP : TILE_DIRECTION.DIRECTION_DOWN;
            Debug.Log( $"<color=puple>decomp direction:{log}</color>" );
            //GetStraightLineJPs( node, biasDirection.y > 0 ? TILE_DIRECTION.DIRECTION_UP : TILE_DIRECTION.DIRECTION_DOWN, temp_jp_list, target );
            if (biasDirection.y > 0)
            {
                GetStraightLineJps_New( node, target, new Vector2Int( TILE_DIRECTION.DIRECTION_UP.x, TILE_DIRECTION.DIRECTION_UP.y ), temp_jp_list );
            }
            else
            {
                GetStraightLineJps_New( node, target, new Vector2Int( TILE_DIRECTION.DIRECTION_DOWN.x, TILE_DIRECTION.DIRECTION_DOWN.y ), temp_jp_list );
            }

            return temp_jp_list;
        }

        /// <summary>
        /// 获取方向
        /// </summary>
        public static TileDirectionEnum GetTileDir ( (int x, int y) dir )
        {
            var type = GetTileScaneDir( dir );
            if (type == TileScanDirection.None)
                return TileDirectionEnum.None;

            if (type == TileScanDirection.Straight)
            {
                if (dir.x == 0)
                    return dir.y > 0 ? TileDirectionEnum.Up : TileDirectionEnum.Down;

                return dir.x > 0 ? TileDirectionEnum.Right : TileDirectionEnum.Left;
            }
            else
            {
                if (dir.y > 0)
                    return dir.x > 0 ? TileDirectionEnum.RightUp : TileDirectionEnum.LeftUp;

                return dir.x > 0 ? TileDirectionEnum.RightDown : TileDirectionEnum.LeftDown;
            }
        }

        public static TileDirectionEnum GetTileDir ( Vector2Int dir )
        {
            var type = GetTileScaneDir( dir );
            if (type == TileScanDirection.None)
                return TileDirectionEnum.None;

            if (type == TileScanDirection.Straight)
            {
                if (dir.x == 0)
                    return dir.y > 0 ? TileDirectionEnum.Up : TileDirectionEnum.Down;

                return dir.x > 0 ? TileDirectionEnum.Right : TileDirectionEnum.Left;
            }
            else
            {
                if (dir.y > 0)
                    return dir.x > 0 ? TileDirectionEnum.RightUp : TileDirectionEnum.LeftUp;

                return dir.x > 0 ? TileDirectionEnum.RightDown : TileDirectionEnum.LeftDown;
            }
        }

        /// <summary>
        /// 获取探测方向
        /// </summary>
        public static TileScanDirection GetTileScaneDir ( (int x, int y) dir)
        {
            if (dir.x == 0 && dir.y == 0)
                return TileScanDirection.None;

            return dir.x != 0 && dir.y != 0 ? TileScanDirection.Bias : TileScanDirection.Straight;
        }

        public static TileScanDirection GetTileScaneDir ( Vector2Int dir )
        {
            if (dir == Vector2Int.zero)
                return TileScanDirection.None;

            return dir.x != 0 && dir.y != 0 ? TileScanDirection.Bias : TileScanDirection.Straight;
        }
    }
}
