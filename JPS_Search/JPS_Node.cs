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
            SetJumpPoint( false );
            _dirList = new List<(int x, int y)>();
        }

        public void Reset ()
        {
            SetJumpPoint( false );
            Parent = null;
        }

        public int ID = -1;

        public JPS_Node Parent = null;
        //障碍物
        public void SetObs ( bool isObs ) => IsObs = isObs;
        public bool IsObs { get; private set; } = false;
        public void SetJumpPoint ( bool isJumpPoint ) => IsJumpPoint = isJumpPoint;
        public bool IsJumpPoint { get; private set; } = false;

        public Vector2 GridPosition { get; private set; } = Vector3.zero;

        public int X { get; private set; }
        public int Y { get; private set; }

        public List<(int x, int y)> _dirList { get; private set; } = null;

        public void AddDir ( Vector2Int dirToAdd )
        {
            if (_dirList is null)
                _dirList = new List<(int x, int y)>();

            var dir = (dirToAdd.x, dirToAdd.y);
            //#todo优化为位运算
            if (_dirList.Contains( dir ))
                return;

            _dirList.Add( dir );
        }

        public void AddDir ( (int x, int y) dirToAdd )
        {
            if(_dirList is null)
                _dirList = new List<(int x,int y)>();

            //#todo优化为位运算
            if (_dirList.Contains( dirToAdd ))
                return;

            _dirList.Add( dirToAdd );
        }

        //public void SetDir ( List<(int x, int y)> dirToSet )
        //{
        //    if (dirToSet is null || dirToSet.Count == 0)
        //    {
        //        Debug.Log( $"<color=red>faild to set dir id:{ID},pos:{ToString()}</color>" );
        //        return;
        //    }

        //    _dirList = dirToSet;
        //}


        //public void SetDir ( params (int x,int y)[] dirToSet )
        //{
        //    if (dirToSet is null || dirToSet.Length == 0)
        //    {
        //        Debug.Log( $"<color=red>faild to set dir id:{ID},pos:{ToString()}</color>" );
        //        return;
        //    }

        //    _dirList.Clear();
        //    _dirList.AddRange(dirToSet);
        //}

        //ID池
        public class Node_ID_Pool
        {
            private static int id = int.MaxValue;
            public static int Gen () => id--;
        }

        public override string ToString ()
        {
            return $"{X},{Y}";
        }
    }

}