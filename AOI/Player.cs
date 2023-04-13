using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace AOI
{
    /// <summary>
    /// 表示一个玩家
    /// </summary>
    public class Player
    {
        public Player(int id_ ,Vector2Int coord_,Tile tile_)
        {
            ID = id_;
            Coord = coord_;
            _tile = tile_;
        }

        private Tile _tile;
        public Vector2Int Coord { get; private set; }
        public int ID { get; private set; } = -1;

        public override string ToString()
        {
            return $"{ID},x:{Coord.x},y:{Coord.y}";
        }
    }
}
