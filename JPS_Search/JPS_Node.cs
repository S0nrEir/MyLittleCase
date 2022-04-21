using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JPS路点
/// </summary>
public class JPS_Node
{
    public JPS_Node ( int x, int y )
    {
        X = x;
        Y = y;
        IsObs = false;
        GridPosition = new Vector2( X, Y );
        ID = Node_ID_Pool.Gen();
    }

    public int ID = -1;

    public JPS_Node Parent = null;
    //障碍物
    public void SetObs ( bool isObs ) => IsObs = isObs;
    public bool IsObs { get; private set; } = false;

    public Vector2 GridPosition { get; private set; } = Vector3.zero;

    public int X { get; private set; }
    public int Y { get; private set; }

    //ID池
    internal class Node_ID_Pool
    {
        private static int id = int.MaxValue;

        public static int Gen () => id--;
    }
}
