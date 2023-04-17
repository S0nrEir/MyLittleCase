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
        /// ��������
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
        /// �ƶ����
        /// </summary>
        private void MoveMyPlayer()
        {
            _curr_scene.PlayerMove( _my_id, _my_curr_direction, _my_tile );
        }

        /// <summary>
        /// ��������
        /// </summary>
        private void CreateScene()
        {
            _curr_scene = new Scene();
        }

        /// <summary>
        /// ����AOI����
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
            //#todo��ʱ����취���治����һֱ���ֱ�������ȷ
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
        /// ��ǰ����
        /// </summary>
        private DirectionTypeEnum _my_curr_direction = DirectionTypeEnum.Invalid;

        /// <summary>
        /// ����
        /// </summary>
        private Scene _curr_scene = null;

        /// <summary>
        /// �ҵ����ID
        /// </summary>
        private int _my_id = 0;

        /// <summary>
        /// ��
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
        /// ��
        /// </summary>
        [SerializeField] private Tile _my_tile;

        /// <summary>
        /// tilemap
        /// </summary>
        [SerializeField] private Tilemap _tile_map = null;

        /// <summary>
        /// ����AOI��������
        /// </summary>
        [Range( 1, 99 )]
        [SerializeField] private int _others_count = 1;

        /// <summary>
        /// ����AOI����
        /// </summary>
        [SerializeField] private Tile _others_tile;

        /// <summary>
        /// ��ͼ����ߴ�
        /// </summary>
        [Range( 10, 100 )]
        [SerializeField] private int MAP_X_SIZE = 100;

        /// <summary>
        /// ��ͼ����ߴ�
        /// </summary>
        [Range( 10, 100 )]
        [SerializeField] private int MAP_Y_SIZE = 100;

        /// <summary>
        /// AOI���򻮷ֻ��ּ���
        /// </summary>
        [SerializeField] private int _aoi_cut_size = 10;

        /// <summary>
        /// ͨ·map
        /// </summary>
        [SerializeField] private Tile _road_tile = null;

        /// <summary>
        /// ����������ϰ��ĵ�
        /// </summary>
        private Vector2Int[] _scene_empty_node = new Vector2Int[]
        {
            new Vector2Int( 50, 50 ),
        };
    }

}