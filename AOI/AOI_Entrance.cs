using UnityEngine;
using UnityEngine.Tilemaps;
namespace AOI
{

    public class AOI_Entrance : MonoBehaviour
    {
        private void OnEnable()
        {
            if ( ( MAP_X_SIZE * MAP_Y_SIZE % _aoi_cut_size ) != 0 )
            {
                Debug.Log( "<color=red>( MAP_X_SIZE * MAP_Y_SIZE % _aoi_cut_size )</color>" );
                return;
            }

            GlobalConfig.LoadConfig
                (
                    MAP_X_SIZE,
                    MAP_Y_SIZE,
                    _aoi_cut_size,
                    _scene_empty_node,
                    _tile_map,
                    _my_tile,
                    _others_tile,
                    _others_count,
                    _road_tile,
                    _aoi_cut_size
                );
        }

        private void Start()
        {
            CreateScene();
            CreatePlayer();
        }

        private void Update()
        {
            Input();
            MoveMyPlayer();
        }

        /// <summary>
        /// 处理输入
        /// </summary>
        private void Input()
        {
            _my_curr_direction = DirectionTypeEnum.Invalid;

            if ( UnityEngine.Input.GetKeyDown( KeyCode.UpArrow ) )
                _my_curr_direction = DirectionTypeEnum.Up;
            else if ( UnityEngine.Input.GetKeyDown( KeyCode.DownArrow ) )
                _my_curr_direction = DirectionTypeEnum.Down;
            else if ( UnityEngine.Input.GetKeyDown( KeyCode.LeftArrow ) )
                _my_curr_direction = DirectionTypeEnum.Left;
            else if ( UnityEngine.Input.GetKeyDown( KeyCode.RightArrow ) )
                _my_curr_direction = DirectionTypeEnum.Right;
            //else
            //    _my_curr_direction = DirectionTypeEnum.Invalid;
        }

        /// <summary>
        /// 移动玩家
        /// </summary>
        private void MoveMyPlayer()
        {
            _curr_scene.PlayerMove( _my_id, _my_curr_direction, _my_tile );
        }

        /// <summary>
        /// 创建场景
        /// </summary>
        private void CreateScene()
        {
            _curr_scene = new Scene();
        }

        /// <summary>
        /// 创建AOI对象
        /// </summary>
        private void CreatePlayer()
        {
            _my_id = Scene.GenID();
            _curr_scene.AddPlayer( _my_id, new Vector2Int( 0, 0 ), _my_tile ,true);
            //for test
            _curr_scene.AddPlayer( Scene.GenID(), new Vector2Int( 11, 0 ), GlobalConfig.Ins._other_tile );
            _curr_scene.AddPlayer( Scene.GenID(), new Vector2Int( 0, 11 ), GlobalConfig.Ins._other_tile );

            return;
            var player_count = GlobalConfig.Ins._others_count;
            var added_count = 0;
            var add_succ = false;
            //#todo暂时解决办法，随不到就一直随机直到结果正确
            while ( added_count < player_count )
            {
                add_succ = false;
                add_succ = _curr_scene.AddPlayer
                    (
                        Scene.GenID(),
                        new Vector2Int( Random.Range( 0, GlobalConfig.Ins.MAP_X_SIZE - 1 ), Random.Range( 0, GlobalConfig.Ins.MAP_Y_SIZE - 1 ) ),
                        GlobalConfig.Ins._other_tile
                    );

                if ( add_succ )
                    added_count++;
            }
        }

        /// <summary>
        /// 当前方向
        /// </summary>
        private DirectionTypeEnum _my_curr_direction = DirectionTypeEnum.Invalid;

        /// <summary>
        /// 场景
        /// </summary>
        private Scene _curr_scene = null;

        /// <summary>
        /// 我的玩家ID
        /// </summary>
        private int _my_id = 0;

        /// <summary>
        /// 我
        /// </summary>
        private Player _me = null;

        public class GlobalConfig
        {
            public static void LoadConfig
                (
                    int x_size_,
                    int y_size_,
                    int aoi_size_,
                    Vector2Int[] empty_scene_node,
                    Tilemap tile_map_,
                    Tile my_tile_,
                    Tile other_tile_,
                    int others_count_,
                    Tile road_tile_,
                    int aoi_area_size_
                )
            {
                _instance                   = new GlobalConfig();
                _instance.MAP_X_SIZE        = x_size_;
                _instance.MAP_Y_SIZE        = y_size_;
                _instance.AOI_SIZE          = aoi_size_;
                _instance._scene_empty_node = empty_scene_node;
                _instance._tile_map         = tile_map_;
                _instance._my_tile          = my_tile_;
                _instance._other_tile       = other_tile_;
                _instance._others_count     = others_count_;
                _instance._road_tile        = road_tile_;
                _instance._aoi_area_size    = aoi_area_size_;
            }

            public static GlobalConfig Ins
            {
                get
                {
                    if ( _instance is null )
                        Debug.LogError( "config not load" );

                    return _instance;
                }
            }

            private static GlobalConfig _instance;

            public int MAP_X_SIZE;
            public int MAP_Y_SIZE;
            public int AOI_SIZE;
            public Vector2Int[] _scene_empty_node = null;
            public Tilemap _tile_map = null;
            public Tile _my_tile = null;
            public Tile _other_tile = null;
            public int _others_count;
            public Tile _road_tile = null;
            public int _aoi_area_size = 0;
        }

        //---------------------config---------------------

        /// <summary>
        /// 我
        /// </summary>
        [SerializeField] private Tile _my_tile;

        /// <summary>
        /// tilemap
        /// </summary>
        [SerializeField] private Tilemap _tile_map = null;

        /// <summary>
        /// 其他AOI对象数量
        /// </summary>
        [Range( 1, 99 )]
        [SerializeField] private int _others_count = 1;

        /// <summary>
        /// 其他AOI对象
        /// </summary>
        [SerializeField] private Tile _others_tile;

        /// <summary>
        /// 地图横向尺寸
        /// </summary>
        [Range( 10, 100 )]
        [SerializeField] private int MAP_X_SIZE = 100;

        /// <summary>
        /// 地图竖向尺寸
        /// </summary>
        [Range( 10, 100 )]
        [SerializeField] private int MAP_Y_SIZE = 100;

        /// <summary>
        /// AOI区域划分划分级别
        /// </summary>
        [SerializeField] private int _aoi_cut_size = 10;

        /// <summary>
        /// 通路map
        /// </summary>
        [SerializeField] private Tile _road_tile = null;

        /// <summary>
        /// 不随机生成障碍的点
        /// </summary>
        private Vector2Int[] _scene_empty_node = new Vector2Int[]
        {
            new Vector2Int( 50, 50 ),
        };
    }

}