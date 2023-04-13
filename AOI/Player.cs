using UnityEngine;

namespace AOI
{
    /// <summary>
    /// 表示一个玩家
    /// </summary>
    public class Player
    {
        public void SetCoord( int x, int y )
        {
            Coord = new Vector2Int( x, y );
        }

        public Player( int id_, Vector2Int coord_ )
        {
            ID = id_;
            Coord = coord_;
        }

        public Vector2Int Coord { get; private set; }
        public int ID { get; private set; } = -1;

        public override string ToString()
        {
            return $"{ID},x:{Coord.x},y:{Coord.y}";
        }
    }
}
