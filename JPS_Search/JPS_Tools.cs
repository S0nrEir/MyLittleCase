using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{
    //jps工具类
    public static class JPS_Tools
    {
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

            //对于parent自身，先检查周围直线方向
            if ( node._dirList != null && node._dirList.Count != 0)
            {
                foreach (var dir in node._dirList)
                {
                    if (GetTileScaneDir( dir ) != TileScanDirection.Straight)
                        continue;

                    GetStraightLineJPs( node, dir, temp_jp_list, target );
                }//end for
            }

            //if (biasDirection.x > 0 )
            //{
            //    GetStraightLineJPs( node, TILE_DIRECTION.DIRECTION_RIGHT, temp_jp_list, target );
            //    GetStraightLineJPs( node, biasDirection.y > 0 ?  TILE_DIRECTION.DIRECTION_UP : TILE_DIRECTION.DIRECTION_DOWN, temp_jp_list, target );
            //}
            //else
            //{
            //    GetStraightLineJPs( node, TILE_DIRECTION.DIRECTION_LEFT, temp_jp_list, target );
            //    GetStraightLineJPs( node, biasDirection.y > 0 ? TILE_DIRECTION.DIRECTION_UP : TILE_DIRECTION.DIRECTION_DOWN, temp_jp_list, target );
            //}

            return temp_jp_list;
        }

        /// <summary>
        /// 获取某一直线方向上的跳点集合
        /// </summary>
        public static void GetStraightLineJPs( JPS_Node node, (int x, int y) direction, List<JPS_Node> temp_jp_list ,JPS_Node target)
        {
            if (temp_jp_list is null)
                return;

            if (direction.x == 0 && direction.y == 0)
                return;

            var ins = JPS_Entrance.I;
            //#todo优化 将方向统一成元组或vector2Int的形式
            var directionVec = new Vector2Int( direction.x, direction.y );
            //直线跳跃搜索的终止条件是直到遇到跳点或边界为止
            //斜向只检查一次就可以了
            //JPS_Node temp = ins.Get( node.X + direction.x, node.Y + direction.y );
            var temp = node;
            while (temp != null && !temp.IsObs)
            {
                if (temp.ID == target.ID)
                {
                    //temp_jp_list.Add( temp );
                    temp_jp_list.Add( target );
                    break;
                }
                if (IsJumpPoint( temp, directionVec ,out var biasNeibArr))
                {
                    if (!JPS_Search_Mgr.I.ContainsInCloseDic( temp ))
                    {
                        foreach (var dir in biasNeibArr)
                        {
                            if (dir.x == 0 && dir.y == 0)
                                continue;

                            temp.AddDir(dir);
                        }
                        temp.AddDir( direction );
                        temp_jp_list.Add( temp );
                        //node.AddDir( direction );
                        //temp_jp_list.Add( node );
                    }
                    break;
                }
                temp = ins.Get( temp.X + direction.x, temp.Y + direction.y );
            }
            return;
        }

        /// <summary>
        /// 检查一个node是否为跳点，在直线见方向上的任意点是否有forceNeib，是返回true
        /// </summary>
        public static bool IsJumpPoint ( JPS_Node node, Vector2Int direction , out (int x,int y)[] biasNeibDir)
        {
            biasNeibDir = new (int x, int y)[2];
            if (direction == Vector2Int.zero)
                return false;

            //临近节点坐标偏移
            Vector2Int neibOffset_1 = new Vector2Int(0,0);
            Vector2Int neibOffset_2 = new Vector2Int( 0, 0 );
            //后方检测点,对于斜向检测两个偏移不一致，所以要两个，从neib点做偏移而不是next
            Vector2Int backOffset_1 = new Vector2Int( 0, 0 );
            Vector2Int backOffset_2 = new Vector2Int( 0, 0 );

            //前方
            var nextNode = JPS_Entrance.I.Get( node.X + direction.x, node.Y + direction.y );
            if (nextNode is null || nextNode.IsObs)
                return false;

            //#TODO优化 用角度判断方向
            //水平方向上下左右
            if (direction.x == 0 || direction.y == 0)
            {
                //竖向偏移在两边
                if (direction.x == 0)
                {
                    neibOffset_1.x = 1;
                    neibOffset_1.y = 0;
                    neibOffset_2.x = -1;
                    neibOffset_2.y = 0;

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
                    neibOffset_1.x = 0;
                    neibOffset_1.y = 1;
                    neibOffset_2.x = 0;
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
            //右上(1,1) 左上(-1,1)
            //右下(1,-1) 左下(-1-1)
            else
            {
                //右朝向
                if (direction.x > 0)
                {
                    //右上还是右下
                    if (direction.y > 0)
                    {
                        neibOffset_1.x = -1;
                        neibOffset_1.y =  1;
                        neibOffset_2.x = 1;
                        neibOffset_2.y = -1;

                        backOffset_1.y = -1;
                        backOffset_2.x = -1;
                    }
                    else
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
                else
                {
                    //左上还是左下
                    if (direction.y > 0)
                    {
                        neibOffset_1.x = -1;
                        neibOffset_1.y = -1;
                        neibOffset_2.x = 1;
                        neibOffset_2.y = 1;

                        backOffset_1.x = 1;
                        backOffset_2.y = -1;
                    } 
                    else
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
            
            Debug.Log( $"<color=orange>node:{node}</color>" );
            Debug.Log( $"<color=orange>:direction:{direction}</color>" );
            Debug.Log( $"<color=orange>nextNode:{nextNode}</color>" );

            //检查两方向任意点是否为强迫邻居和跳点
            var neibNode = JPS_Entrance.I.Get( nextNode.X + neibOffset_1.x, nextNode.Y + neibOffset_1.y ,true);
            JPS_Node isObjsNode = null;
            var hasNeib = false;
            var log = (x:0, y:0);
            if (neibNode != null && !neibNode.IsObs)
            {
                isObjsNode = JPS_Entrance.I.Get( neibNode.X + backOffset_1.x, neibNode.Y + backOffset_1.y, true );

                Debug.Log( $"<color=white>isObsNode:{isObjsNode}</color>" );
                Debug.Log( $"<color=white>neibOffset_1:{neibOffset_1}</color>" );
                Debug.Log( $"<color=white>backOffset_1:{backOffset_1}</color>" );

                hasNeib |= isObjsNode != null && isObjsNode.IsObs;
                //#for test
                log = (isObjsNode.X - node.X, isObjsNode.Y - node.Y);
                if (hasNeib)
                {
                    Debug.Log($"log:{log.x},{log.y}");
                    biasNeibDir[0] = log;
                }
            }

            neibNode = JPS_Entrance.I.Get( nextNode.X + neibOffset_2.x, nextNode.Y + neibOffset_2.y, true );
            isObjsNode = null;
            if (neibNode != null && !neibNode.IsObs)
            {
                isObjsNode = JPS_Entrance.I.Get( neibNode.X + backOffset_2.x, neibNode.Y + backOffset_2.y ,true );

                Debug.Log( $"<color=white>isObsNode:{isObjsNode}</color>" );
                Debug.Log( $"<color=white>neibOffset_2:{neibOffset_2}</color>" );
                Debug.Log( $"<color=white>backOffset_2:{backOffset_2}</color>" );

                //JP检查
                //if (isObjsNode != null && isObjsNode.IsObs)
                //    return true;

                log = (isObjsNode.X - node.X, isObjsNode.Y - node.Y);
                if (hasNeib)
                {
                    Debug.Log( $"log:{log.x},{log.y}" );
                    biasNeibDir[0] = log;
                }
            }
            return hasNeib;
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

        public static (int x, int y)[] DecompBiasDirectioin ((int x,int y) dir)
        {
            if (GetTileScaneDir( dir ) != TileScanDirection.Bias)
                return null;

            if (dir.x > 0)
                return new (int x, int y)[] { (1, 0), (0, dir.y > 0 ? 1 : -1) };
            else
                return new (int x, int y)[] { (-1, 0), (0, dir.y > 0 ? 1 : -1) };
        }

        ////返回某一直线方向上的跳点集合
        //public static List<JPS_Node> StraightLineScan ( JPS_Node node, (int x, int y) way)
        //{
        //    var ins = JPS_Entrance.I;
        //    JPS_Node temp = ins.Get(node.X + way.x,node.Y + way.y);
        //    var direction = GetFindingDirection( node.X, node.Y, temp.X, temp.Y );
        //    //方向不对
        //    if (direction == Vector2.zero)
        //        return new List<JPS_Node>( 0 );

        //    var list = new List<JPS_Node>();
        //    while (temp != null && !temp.IsObs)
        //    {
        //        //是否为跳点
        //        if (!IsJumpPoint( temp, direction ))
        //        {
        //            temp = ins.Get( temp.X + direction.x, temp.Y + direction.y );
        //            continue;
        //        }

        //        list.Add( temp );
        //        temp = ins.Get( temp.X + direction.x, temp.Y + direction.y );
        //    }

        //    return list;
        //}


        /// <summary>
        /// 获得两点之间的方向，确保两点之间出于水平或斜线,(0,0) = 无方向
        /// </summary>
        //private static/* (int x,int y)*/Vector2Int GetFindingDirection (int prevX,int prevY,int nextX,int nextY)
        //{
        //    Vector2Int result = new Vector2Int( prevX - nextX, prevY - nextY );

        //    if (result.x == 0 && result.y == 0)
        //        return result;

        //    //上下
        //    if (result.x == 0)
        //        return new Vector2Int(0, result.y > 0 ? 1 : -1);

        //    //左右
        //    if (result.y == 0)
        //        return new Vector2Int( result.x > 0 ? 1 : -1, 0);

        //    //斜线的四种情况
        //    if (result.x < 0)
        //        //左上或左下
        //        return new Vector2Int( -1, result.y > 0 ? 1 : -1);
        //    else
        //        //右上或右下
        //        return new Vector2Int( 1, result.y > 0 ? 1 : -1);
        //}
    }
}
