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
    }
}
