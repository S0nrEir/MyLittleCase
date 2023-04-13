using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static AOI.AOI_Entrance;

namespace AOI
{
    /// <summary>
    /// 真实场景
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// 随机生成障碍物
        /// </summary>
        /// <param name="empty_nodes_">避免生成障碍物的点</param>
        /// <param name="objs_count">生成障碍物的数量</param>
        private void RandomGenerateObs( int obs_count = 30 )
        {
            var gen_count = 0;
            while ( gen_count < obs_count )
            {
                Vector2Int gen_coord = new Vector2Int
                    ( 
                        Random.Range( GlobalConfig.Ins.MAP_X_SIZE - 1, GlobalConfig.Ins.MAP_Y_SIZE - 1 ),
                        Random.Range( GlobalConfig.Ins.MAP_X_SIZE - 1, GlobalConfig.Ins.MAP_Y_SIZE - 1 ) 
                    );

                _scene_data[gen_coord.x, gen_coord.y]._block = true;
            }
        }

        /// <summary>
        /// 移动指定player
        /// </summary>
        public void PlayerMove( int id, DirectionTypeEnum type )
        {
            if ( !_player_dic.TryGetValue( id, out var player ) )
                return;

            var dir = ConvertDirectionTypeEnum( type );
        }

        /// <summary>
        /// 向场景内添加一个玩家
        /// </summary>
        public bool AddPlayer( int id_, Vector2Int coord_, Tile tile_ )
        {
            //#todo越界检查
            ref var data = ref _scene_data[coord_.x, coord_.y];
            //有其他玩家或不可通行
            if ( data._has_player || data._block)
                return false;

            var player = new Player( id_, coord_, tile_ );
            if ( _curr_player_count >= _player_dic.Count )
                _player_dic.EnsureCapacity( _curr_player_count << 1 );

            _player_dic.Add( player.ID, player );
            _curr_player_count++;

            data._has_player = true;
            _tile_map.SetTile( new Vector3Int( coord_.x, coord_.y, 0 ), tile_ );

            Debug.Log( $"----------------------{_scene_data[coord_.x, coord_.y]._has_player}" );

            return true;
        }

        /// <summary>
        /// 从场景内移除一个玩家
        /// </summary>
        public bool RemovePlayer( Player player_ )
        {
            if ( _player_dic is null )
            {
                Debug.LogError( "_player_dic is null" );
                return false;
            }
            var succ = _player_dic.Remove( player_.ID );
            if ( succ )
                _curr_player_count--;

            return succ;
        }

        public void ConnectAOIZone( AOI_Zone aoi_zone_, int aoi_cut_size_ )
        {

        }

        public Scene()
        {
            _scene_data = new Scene_Node[GlobalConfig.Ins.MAP_X_SIZE, GlobalConfig.Ins.MAP_Y_SIZE];
            _tile_map = GlobalConfig.Ins._tile_map;
            for ( var i = 0; i < GlobalConfig.Ins.MAP_X_SIZE; i++ )
            {
                for ( var j = 0; j < GlobalConfig.Ins.MAP_Y_SIZE; j++)
                    _tile_map.SetTile( new Vector3Int( i, j ,0 ), GlobalConfig.Ins._road_tile );
            }

            _player_dic = new Dictionary<int, Player>( 128 );
            _curr_player_count = 0;
            //RandomGenerateObs();
        }

        private Tilemap _tile_map = null;

        /// <summary>
        /// 玩家缓存
        /// </summary>
        private Dictionary<int, Player> _player_dic = null;

        /// <summary>
        /// 玩家数量
        /// </summary>
        private int _curr_player_count = 0;

        /// <summary>
        /// 地图节点
        /// </summary>
        private Scene_Node[,] _scene_data = null;

        /// <summary>
        /// 方向枚举转换
        /// </summary>
        public (int x, int y) ConvertDirectionTypeEnum( DirectionTypeEnum type )
        {
            switch ( type )
            {
                case DirectionTypeEnum.Up:
                    return (0, 1);

                case DirectionTypeEnum.Down:
                    return (0, -1);

                case DirectionTypeEnum.Left:
                    return (-1, 0);

                case DirectionTypeEnum.Right:
                    return (1, 0);
            }
            return (0, 0);
        }

        public static int GenID()
        {
            return _id_pool--;
        }
        private static int _id_pool = int.MaxValue;
    }

    /// <summary>
    /// 方向枚举
    /// </summary>
    public enum DirectionTypeEnum
    {
        Invalid = -1,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
    }
}

