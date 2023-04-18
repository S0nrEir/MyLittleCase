using System.Collections.Generic;
using UnityEngine;

namespace AOI
{
    public class AOI_Zone
    {
        /// <summary>
        /// Agent在AOI区域内移动
        /// </summary>
        public void Move( IAOI_Agent agent_, (int x, int y) from_, (int x, int y) to_, int player_id_, Scene scene_ )
        {
            if ( agent_ is null )
            {
                Debug.LogWarning( "faild to convert AOI Agent" );
                return;
            }

            if ( !TryGetArea( from_.x, from_.y, out var area ) )
            {
                Debug.LogWarning( "faild to get aoi area" );
                return;
            }

            //处理观察者
            var watchers = area.GetObserve( player_id_ );
            IAOI_Agent temp_agent = null;
            var iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( iter.Current, out temp_agent );
                temp_agent?.Move( agent_ );
            }

            //处理被观察者
            watchers = area.GetObserved( player_id_ );
            iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( iter.Current, out temp_agent );
                if ( temp_agent != null )
                    agent_.Move( temp_agent );

            }
            //检查穿越AOI边界
            if ( TryGetArea( to_.x, to_.y, out var target_area ) )
            {
                if ( target_area == area )
                    return;

                //离开当前AOI区域，进入新的AOI区域
                //当前区域leave
                Leave( agent_, from_.x, from_.y, player_id_, scene_ );
                Enter( agent_, to_.x, to_.y, player_id_, scene_ );
            }
        }
        //得拿到这个AOI区域内的所有对象才能知道当玩家进入新区域时，要添加哪些观察者和被观察者
        //因为考虑到玩家并不是要直接添加该区域的所有玩家，可能会有一些筛选条件，有的拿有的不拿，
        //另外这样做也有利于扩展，建议在Scene处理对象移动的时候就确定要添加哪些观察者
        //目前想到的办法：AOI_Area提供一个对外接口表示当前区域内的所有玩家ID
        //Scene在做逻辑的时候，拿这些ID，然后做筛选，作为要添加的观察者。
        //如果是基于对象自身的九宫格范围的话：在进入和离开的时候拿到两个比较结果的差集，作为进入和离开要通知的被观察者和观察者
        public void Enter( IAOI_Agent agent_, int x_, int y_, int player_id_, Scene scene_ )
        {
            if ( agent_ is null )
            {
                Debug.LogWarning( "faild to convert AOI Agent" );
                return;
            }

            if ( !TryGetArea( x_, y_, out var area ) )
            {
                Debug.LogWarning( "faild to get aoi area" );
                return;
            }

            //处理观察者和被观察者
            //暂时处理：添加该区域内所有的对象
            area.AddAgent( player_id_ );
            var agents = area.GetAllAgents();
            foreach ( var id in agents )
            {
                //不添加自己
                if ( id == player_id_ )
                    continue;

                area.AddObserve( player_id_, id );
                area.AddObserving( player_id_, id );
            }
            var watchers = area.GetObserve( player_id_ );
            IAOI_Agent temp_agent = null;
            var iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( iter.Current, out temp_agent );
                temp_agent?.Enter( agent_ );
            }

            foreach ( var id in area.GetObserveArray( player_id_ ) )
                //让我的观察者观察我
                area.AddObserving( id, player_id_ );

            //处理被观察者
            watchers = area.GetObserved( player_id_ );
            iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( iter.Current, out temp_agent );
                if ( temp_agent != null )
                    agent_.Enter( temp_agent );
            }
        }

        public void Leave( IAOI_Agent agent_, int x_, int y_, int player_id_, Scene scene_ )
        {
            if ( agent_ is null )
            {
                Debug.LogWarning( "faild to convert AOI Agent" );
                return;
            }

            if ( !TryGetArea( x_, y_, out var area ) )
            {
                Debug.LogWarning( "faild to get aoi area" );
                return;
            }

            //处理观察者
            var watchers = area.GetObserve( player_id_ );
            IAOI_Agent temp_agent = null;
            var iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( iter.Current , out temp_agent );
                temp_agent?.Exit( agent_ );
            }

            //处理被观察者
            watchers = area.GetObserved( player_id_ );
            iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( iter.Current , out temp_agent );
                if ( temp_agent != null )
                    agent_.Exit( temp_agent );
            }

            area.RemoveObserve( player_id_ );
            area.RemoveObserving( player_id_ );
            area.RemoveAgent( player_id_ );
        }

        /// <summary>
        /// 根据坐标获取一个Area
        /// </summary>
        public bool TryGetArea( int x_, int y_, out AOI_Area area_ )
        {
            area_ = null;
            (int x, int y) area_pos = (X( x_ ), Y( y_ ));
            if ( area_pos.x < 0 || area_pos.y < 0 )
            {
                Debug.LogWarning( "area_pos.x < 0 || area_pos.y < 0" );
                return false;
            }

            area_ = _areas[area_pos.x][area_pos.y];
            return true;
        }

        private int Y( int y_ )
        {
            var y = y_ / _size;
            if ( IsOutOfRange( y ) )
                return -1;

            return y;
        }

        private int X( int x_ )
        {
            var x = x_ / _size;
            if ( IsOutOfRange( x ) )
                return -1;

            return x;
        }

        private bool IsOutOfRange( int val )
        {
            return val >= _size || val < 0;
        }

        public AOI_Zone( int size_ )
        {
            _size = size_;
            _areas = new List<AOI_Area>[_size];
            List<AOI_Area> temp = null;
            for ( var i = 0; i < _size; i++ )
            {
                temp = new List<AOI_Area>( _size );
                _areas[i] = temp;
                var cnt = temp.Capacity;

                for ( var j = 0; j < cnt; j++ )
                    temp.Add( new AOI_Area(i,j) );
            }//end for
        }

        private int _size = 0;
        private List<AOI_Area>[] _areas = null;
    }

    /// <summary>
    /// 通知类型
    /// </summary>
    internal enum NotifyTypeEnum
    {
        Invalid = -1,
        Move = 0,
        Enter,
        Leave
    }
}
