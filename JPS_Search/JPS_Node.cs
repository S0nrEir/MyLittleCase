using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JPS
{
    /// <summary>
    /// JPS路点
    /// </summary>
    public class JPS_Node
    {
        public JPS_Node ( int x, int y )
        {
            X = x;
            Y = y;
            GridPosition = new Vector2( X, Y );
            ID = Node_ID_Pool.Gen();
            SetObs( false );
            SetForceNeib( false );
            SetJumpPoint( false );
        }

        public int ID = -1;

        public JPS_Node Parent = null;
        //障碍物
        public void SetObs ( bool isObs ) => IsObs = isObs;
        public bool IsObs { get; private set; } = false;
        public void SetForceNeib ( bool isForceNeib ) => IsForceNeib = isForceNeib;
        public bool IsForceNeib { get; private set; } = false;
        public void SetJumpPoint ( bool isJumpPoint ) => IsJumpPoint = isJumpPoint;
        public bool IsJumpPoint { get; private set; } = false;

        public Vector2 GridPosition { get; private set; } = Vector3.zero;

        public int X { get; private set; }
        public int Y { get; private set; }

        //ID池
        public class Node_ID_Pool
        {
            private static int id = int.MaxValue;

            public static int Gen () => id--;
        }
    }

}