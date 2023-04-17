using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AOI
{

    public class MyPlayer : Player
    {
        public MyPlayer( int id_, Vector2Int coord_ ) : base( id_, coord_ )
        {
        }

        public override void Enter( IAOI_Agent other_ )
        {
            //base.Enter( other_ );
            Debug.Log( $"<color=white>my player ---> enter,other id = {(other_ as Player).ID}</color>" );
        }

        public override void Exit( IAOI_Agent other_ )
        {
            //base.Exit( other_ );
            Debug.Log( $"<color=white>my player ---> exit,other id = {(other_ as Player).ID}</color>" );
        }

        public override void Move( IAOI_Agent other_ )
        {
            //base.Exit( other_ );
            Debug.Log( $"<color=white>my player ---> move,other id = {(other_ as Player).ID}</color>" );
        }
    }

}