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

        /// 获取某一斜向的遍历直线跳点集合
        public static List<JPS_Node> GetBiasStraightLineJPs (JPS_Node node,Vector2Int biasDirection, List<JPS_Node> temp_jp_list = null)
        {
            if(temp_jp_list is null)
                temp_jp_list = new List<JPS_Node>();

            if (node != null || !node.IsObs)
                return temp_jp_list;

            temp_jp_list.AddRange( GetStraightLineJPs( node, (0,1) ) );
            temp_jp_list.AddRange( GetStraightLineJPs( node, (0,-1)) );
            temp_jp_list.AddRange( GetStraightLineJPs( node, (1,0) ) );
            temp_jp_list.AddRange( GetStraightLineJPs( node, (-1,0)) );

            if (temp_jp_list.Count != 0)
                return temp_jp_list;

            return GetBiasStraightLineJPs( JPS_Entrance.I.Get(node.X + biasDirection.x,node.Y + biasDirection.y) ,biasDirection , temp_jp_list);
        }

        public static List<JPS_Node> JumpPoint (JPS_Node node,(int,int)[] ways)
        {
            var list = new List<JPS_Node>();
            var tempList = new List<JPS_Node>();
            JPS_Node temp = null;
            //周围方向找跳点并且返回
            for (var i = 0; i < ways.Length; i++)
            {
                //temp = JPS_Entrance.I.Get( node.X + ways[i].Item1, node.Y + ways[i].Item2 );
                ////边界或障碍物
                //if (temp is null || temp.IsObs)
                //    continue;

                tempList = StraightLineScan( node, ways[i] );
                list.AddRange( tempList );
            }
            return list;
        }

        /// <summary>
        /// GetStraightLineJPs的list返回形式
        /// </summary>
        /// <returns></returns>
        public static List<JPS_Node> GetStraightLineList ( JPS_Node node, (int x, int y) direction )
        {
            var list = new List<JPS_Node>();
            list.AddRange( GetStraightLineJPs( node, direction) );
            return list;
        }

        /// <summary>
        /// 获取某一直线方向上的跳点集合
        /// </summary>
        public static JPS_Node[] GetStraightLineJPs( JPS_Node node, (int x, int y) direction)
        {
            if (direction.x == 0 && direction.y == 0)
                return new JPS_Node[0];

            var ins = JPS_Entrance.I;
            //var list = new List<JPS_Node>();
            var arr = new JPS_Node[2];
            //#todo优化 将方向统一成元组或vector2Int的形式
            var directionVec = new Vector2Int( direction.x, direction.y );
            //直线跳跃搜索的终止条件是直到遇到跳点或边界为止
            //斜向只检查一次就可以了
            JPS_Node temp = ins.Get( node.X + direction.x, node.Y + direction.y );
            while (temp != null || !temp.IsObs)
            {
                if (IsJumpPoint( temp, directionVec ))
                {
                    arr[0] = node;
                    arr[1] = temp;
                    break;
                }
                temp = ins.Get( node.X + direction.x, node.Y + direction.y );
            }
            return arr;
        }

        /// 检查一个node是否为跳点，是返回true
        private static bool IsJumpPoint ( JPS_Node node, Vector2Int direction )
        {
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
            //检查两方向任意点是否为强迫邻居和跳点
            var neibNode = JPS_Entrance.I.Get( nextNode.X + neibOffset_1.x, nextNode.Y + neibOffset_1.y );
            JPS_Node isObjsNode = null;
            if (neibNode != null && !neibNode.IsObs)
            {
                isObjsNode = JPS_Entrance.I.Get( neibNode.X + backOffset_1.x, neibNode.Y + backOffset_1.y );
                //JP检查
                if (isObjsNode != null && isObjsNode.IsObs)
                    return true;

                isObjsNode = JPS_Entrance.I.Get( neibNode.X + backOffset_2.x, neibNode.Y + backOffset_2.y );
                if (isObjsNode != null && isObjsNode.IsObs)
                    return true;
            }
            return false;
        }

        //返回某一直线方向上的跳点集合
        public static List<JPS_Node> StraightLineScan ( JPS_Node node, (int x, int y) way)
        {
            var ins = JPS_Entrance.I;
            JPS_Node temp = ins.Get(node.X + way.x,node.Y + way.y);
            var direction = GetFindingDirection( node.X, node.Y, temp.X, temp.Y );
            //方向不对
            if (direction == Vector2.zero)
                return new List<JPS_Node>( 0 );

            var list = new List<JPS_Node>();
            while (temp != null && !temp.IsObs)
            {
                //是否为跳点
                if (!IsJumpPoint( temp, direction ))
                {
                    temp = ins.Get( temp.X + direction.x, temp.Y + direction.y );
                    continue;
                }

                list.Add( temp );
                temp = ins.Get( temp.X + direction.x, temp.Y + direction.y );
            }

            return list;
        }


        /// <summary>
        /// 获得两点之间的方向，确保两点之间出于水平或斜线,(0,0) = 无方向
        /// </summary>
        private static/* (int x,int y)*/Vector2Int GetFindingDirection (int prevX,int prevY,int nextX,int nextY)
        {
            Vector2Int result = new Vector2Int( prevX - nextX, prevY - nextY );

            if (result.x == 0 && result.y == 0)
                return result;

            //上下
            if (result.x == 0)
                return new Vector2Int(0, result.y > 0 ? 1 : -1);

            //左右
            if (result.y == 0)
                return new Vector2Int( result.x > 0 ? 1 : -1, 0);

            //斜线的四种情况
            if (result.x < 0)
                //左上或左下
                return new Vector2Int( -1, result.y > 0 ? 1 : -1);
            else
                //右上或右下
                return new Vector2Int( 1, result.y > 0 ? 1 : -1);
        }
    }
}
