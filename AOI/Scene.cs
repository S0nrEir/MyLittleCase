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
        /// 获取一个AOI Agent
        /// </summary>
        public bool TryGetAOIAgent(int player_id_,out IAOI_Agent agent_)
        {
            _player_dic.TryGetValue( player_id_, out var player );
            agent_ = player as IAOI_Agent;
            return agent_ != null;
        }

        /// <summary>
        /// 移动指定player
        /// </summary>
        public void PlayerMove( int id, DirectionTypeEnum type, Tile tile_ )
        {
            if ( !_player_dic.TryGetValue( id, out var player ) )
                return;
            if ( type != DirectionTypeEnum.Invalid )
                ;

            var dir = ConvertDirectionTypeEnum( type );
            (int x, int y) target_pos = (player.Coord.x + dir.x, player.Coord.y + dir.y);
            if ( IsOutOfRange( target_pos.x, target_pos.y ) )
            {
                Debug.Log( "<color=whie>move player ---> out of range</color>" );
                return;
            }

            //if ( target_pos.x == player.Coord.x && target_pos.y == player.Coord.y )
            //    return;

            ref var target_pos_data = ref _scene_data[target_pos.x, target_pos.y];
            if ( target_pos_data._block || target_pos_data._has_player )
            {
                //Debug.Log( "<color=whie>move player ---> blocked || has player</color>" );
                return;
            }

            //aoi
            //--->这块写的不好，场景和玩家应该独立出来成为全局组件，现在只能通过传值的方式给到AOI部分<---
            //--->这块写的不好，场景和玩家应该独立出来成为全局组件，现在只能通过传值的方式给到AOI部分<---
            //--->这块写的不好，场景和玩家应该独立出来成为全局组件，现在只能通过传值的方式给到AOI部分<---
            _aoi_zone?.Move
                (
                    player as IAOI_Agent,
                    (player.Coord.x, player.Coord.y),
                    (target_pos.x, target_pos.y),
                    player.ID,
                    this
                );

            //reset curr
            var curr_pos = player.Coord;

            ref var curr_data = ref _scene_data[curr_pos.x, curr_pos.y];
            curr_data._has_player = false;
            _tile_map.SetTile( new Vector3Int( curr_pos.x, curr_pos.y, 0 ), GlobalConfig.Ins._road_tile );

            //move player
            target_pos_data._has_player = true;
            player.SetCoord( target_pos.x, target_pos.y );
            _tile_map.SetTile( new Vector3Int( target_pos.x, target_pos.y, 0 ), tile_ );
        }

        /// <summary>
        /// 向场景内添加一个玩家
        /// </summary>
        public bool AddPlayer( int id_, Vector2Int coord_, Tile tile_ ,bool is_my_player_ = false)
        {
            if ( IsOutOfRange( coord_.x, coord_.y ) )
                return false;

            ref var data = ref _scene_data[coord_.x, coord_.y];
            //有其他玩家或不可通行
            if ( data._has_player || data._block )
                return false;

            Player player = null;
            if ( is_my_player_ )
                player = new MyPlayer( id_, coord_ );
            else
                player = new Player( id_, coord_ );

            if ( _curr_player_lmt <= _player_dic.Count )
                _player_dic.EnsureCapacity( _curr_player_lmt << 1 );

            _player_dic.Add( player.ID, player );

            data._has_player = true;
            _tile_map.SetTile( new Vector3Int( coord_.x, coord_.y, 0 ), tile_ );
            Debug.Log( $"----------------------{_scene_data[coord_.x, coord_.y]._has_player}" );

            //add to aoi area
            _aoi_zone.Enter( player as IAOI_Agent, coord_.x, coord_.y, id_, this );
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
            //remove from aoi
            _aoi_zone.Leave( player_ as IAOI_Agent, player_.Coord.x, player_.Coord.y,player_.ID, this );


            var succ = _player_dic.Remove( player_.ID );
            return succ;
        }

        public Scene()
        {
            _scene_data = new Scene_Node[GlobalConfig.Ins.MAP_X_SIZE, GlobalConfig.Ins.MAP_Y_SIZE];
            _tile_map = GlobalConfig.Ins._tile_map;
            for ( var i = 0; i < GlobalConfig.Ins.MAP_X_SIZE; i++ )
            {
                for ( var j = 0; j < GlobalConfig.Ins.MAP_Y_SIZE; j++ )
                {
                    _tile_map.SetTile( new Vector3Int( i, j, 0 ), GlobalConfig.Ins._road_tile );
                    ref Scene_Node temp_node = ref _scene_data[i, j];
                    temp_node.Setup( new Vector2Int( i, j ), new Vector3( i * _tile_map.cellSize.x, j * _tile_map.cellSize.y, 0f ) );
                }
            }

            _player_dic = new Dictionary<int, Player>( _curr_player_lmt );
            _aoi_zone = new AOI_Zone( GlobalConfig.Ins._aoi_area_size );
            //RandomGenerateObs();
        }

        private bool IsOutOfRange( int x, int y )
        {
            return x >= GlobalConfig.Ins.MAP_X_SIZE || y >= GlobalConfig.Ins.MAP_Y_SIZE || x < 0 || y < 0;
        }

        private AOI_Zone _aoi_zone = null;

        private Tilemap _tile_map = null;

        /// <summary>
        /// 玩家缓存
        /// </summary>
        private Dictionary<int, Player> _player_dic = null;

        /// <summary>
        /// 玩家数量
        /// </summary>
        private int _curr_player_lmt = 128;

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
            return _id_pool++;
        }
        private static int _id_pool = 0;
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

