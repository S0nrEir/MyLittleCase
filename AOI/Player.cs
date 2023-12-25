using UnityEngine;

namespace AOI
{
    public class Player : IAOI_Agent
    {
        //--------------impl--------------
        public virtual void Enter( IAOI_Agent other_ )
        {
            Debug.Log( $"id = {ID} ---> enter,enter id = {( other_ as Player ).ID}" );
        }

        public virtual void Move( IAOI_Agent other_ )
        {
            Debug.Log( $"id = {ID} ---> Move,move id = {( other_ as Player ).ID}" );
        }

        public virtual void Exit( IAOI_Agent other_ )
        {
            Debug.Log( $"id = {ID} ---> exit,exit id = {( other_ as Player ).ID}" );
        }

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
