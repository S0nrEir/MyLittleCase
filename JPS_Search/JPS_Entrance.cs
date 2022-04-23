using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace JPS
{
    public class JPS_Entrance : MonoBehaviour
    {
        private void Awake ()
        {
            _i = this;
        }

        void Start ()
        {
            CreateMap();
            _drawedPathSet = new HashSet<JPS_Node>();

            //var pathList = JPS_Search_Mgr.I.GetPath( NodesArr[14,9], NodesArr[80, 94] );
            //DrawPath( pathList );
        }

        private void Update ()
        {
            OnClearInput();
            OnStartInput();
            OnStartFindInput();
            //OnDrawInputWay();
        }

        public void SetJPTile_Test (JPS_Node node)
        {
            _tileMap.SetTile( new Vector3Int( node.X, node.Y ,0), _jpTile );
        }

        /// <summary>
        /// 从arr中获取一个点，拿不到返回null
        /// </summary>
        public JPS_Node Get ( int x, int y ,bool includeObs = false)
        {
            if (x < 0 || y < 0)
            {
                //Debug.Log($"<color=orange>{(x,y)}</color>");
                return null;
            }

            if (x >= MAX_ROW || y >= MAX_COL)
            {
                //Debug.Log( $"<color=orange>{(x, y)}</color>" );
                return null;
            }

            var node = NodesArr[x, y];
            if (node.IsObs)
            {
                if (includeObs)
                    return node;

                return null;
            }

            return node;
        }

        /// <summary>
        /// 开始寻路
        /// </summary>
        private void OnStartFindInput ()
        {
            if (!Input.GetKeyDown( KeyCode.S ))
                return;

            if (_start is null || _target is null)
                return;

            foreach (var p in NodesArr)
                p.Reset();

            Debug.Log($"<color=green>start:{_start}</color>");
            Debug.Log( $"<color=green>target:{_target}</color>" );

            var pathList = _pathFindingType == PathFindingTypeEnum.AStar ? JPS_Search_Mgr.I.AStar( _start, _target ) : JPS_Search_Mgr.I.JPS(_start,_target);
            DrawPath( pathList );
        }

        private void OnClearInput ()
        {
            if (!Input.GetKeyDown( KeyCode.Space ))
                return;

            if (_start is null || _target is null)
                return;

            _tileMap.SetTile( new Vector3Int( _start.X, _start.Y, 0 ), _roadTile );
            _tileMap.SetTile( new Vector3Int( _target.X, _target.Y, 0 ), _roadTile );

            _start = null;
            _target = null;

            foreach (var p in _drawedPathSet)
                _tileMap.SetTile( new Vector3Int( p.X, p.Y, 0 ), _roadTile );

            _drawedPathSet.Clear();
        }

        /// <summary>
        /// 处理开始输入
        /// </summary>
        private void OnStartInput ()
        {
            //左键确定起点
            if (Input.GetMouseButtonUp( 0 ))
            {
                var worldPos = Camera.main.ScreenToWorldPoint( Input.mousePosition );
                var x = (int)worldPos.x;
                var y = (int)worldPos.y;
                if (_start == null)
                {
                    _start = Get( x, y );

                    if (_start != null)
                    {
                        _tileMap.SetTile( new Vector3Int( _start.X, _start.Y, 0 ), _startTile );
                        return;
                    }
                }
                else if (_target == null)
                {
                    _target = Get( x, y );

                    if (_target != null)
                        _tileMap.SetTile( new Vector3Int( _target.X, _target.Y, 0 ), _targetTile );
                }
            }
        }

        private void DrawPath ( IList<JPS_Node> pathList )
        {
            if (pathList is null || pathList.Count == 0)
                return;

            Debug.Log( $"pathList count : {pathList.Count}" );
            for (var i = 1; i < pathList.Count - 1; i++)
            {
                _tileMap.SetTile( new Vector3Int( pathList[i].X, pathList[i].Y, 0 ), _pathTile );
                _drawedPathSet.Add( pathList[i] );
            }
        }

        private void DrawJPSPath (IList<JPS_Node> pathList )
        {
            if (pathList is null || pathList.Count == 0)
                return;

            Debug.Log( $"<color=green>pathList count : {pathList.Count}</color>" );
            for (var i = 1; i < pathList.Count - 1; i++)
            {
                _tileMap.SetTile( new Vector3Int( pathList[i].X, pathList[i].Y, 0 ), _jpTile );
                _drawedPathSet.Add( pathList[i] );
            }
        }

        /// <summary>
        /// 创建地图
        /// </summary>
        private void CreateMap ()
        {
            NodesArr = new JPS_Node[MAX_ROW, MAX_COL];
            JPS_Node node = null;
            var isObs = false;
            Vector3Int pos = Vector3Int.zero;
            for (int i = 0; i < MAX_ROW; i++)
            {
                for (int j = 0; j < MAX_COL; j++)
                {
                    node = new JPS_Node( i, j );
                    isObs = Random.value <= _obsPercent;
                    node.SetObs( isObs );
                    pos.x = i;
                    pos.y = j;
                    _tileMap.SetTile( pos, isObs ? _obsTile : _roadTile );
                    //CreateTextAtXY( pos );
                    NodesArr[i, j] = node;
                }
            }//end for
        }

        private void CreateTextAtXY (Vector3Int worldPos)
        {
            if (_canvas == null)
                return;

            var obj = new GameObject();
            obj.transform.SetParent( _canvas );

            obj.name = $"{worldPos.x},{worldPos.y}";
            var txt = obj.AddComponent<Text>();
            txt.text = $"{worldPos.x},{worldPos.y}";
            obj.transform.position = new Vector3(worldPos.x,worldPos.y,0);
        }

        private JPS_Node _start = null;
        private JPS_Node _target = null;

        [SerializeField] private Tile _jpTile = null;
        [SerializeField] private Tile _pathTile = null;
        [SerializeField] private Tilemap _tileMap = null;
        [SerializeField] private Tile _targetTile = null;
        [SerializeField] private Tile _startTile = null;
        [SerializeField] private Tile _roadTile = null;
        [SerializeField] private Tile _obsTile = null;
        [SerializeField] private Tile _inputTile = null;
        [SerializeField] [Range( 0, 1f )] private float _obsPercent = .1f;
        [SerializeField] private PathFindingTypeEnum _pathFindingType = PathFindingTypeEnum.JPS;
        [SerializeField] private Transform _canvas = null;
        private HashSet<JPS_Node> _drawedPathSet = null;
        //[SerializeField] private Vector2 _inputWay;

        //最大行列0~99
        private const int MAX_ROW = 100;
        private const int MAX_COL = 100;

        public JPS_Node[,] NodesArr = null;

        public static JPS_Entrance _i = null;
        public static JPS_Entrance I
        {
            get => _i;
        }


    }

    internal enum PathFindingTypeEnum
    {
        AStar,
        JPS,
        JPS_Optimize
    }

}
