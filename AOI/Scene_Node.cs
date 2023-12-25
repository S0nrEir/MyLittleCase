using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AOI
{
    /// <summary>
    /// 表示真实场景的一个坐标节点
    /// </summary>
    public struct Scene_Node
    {

        /// <summary>
        /// 设置
        /// </summary>
        public void Setup(Vector2Int coord_,Vector2 worldPos_)
        {
            Coord = coord_;
            WorldPos = worldPos_;
            _block = false;
            _has_player = false;
        }

        /// <summary>
        /// 玩家占位标记
        /// </summary>
        public bool _has_player;

        /// <summary>
        /// 不可通行
        /// </summary>
        public bool _block;

        /// <summary>
        /// 网格化地图坐标
        /// </summary>
        public Vector2Int Coord { get; private set; }

        /// <summary>
        /// 世界位置
        /// </summary>
        public Vector2 WorldPos { get; private set; }
    }
}