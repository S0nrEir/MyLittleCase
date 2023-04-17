using System;
using System.Collections.Generic;
using UnityEngine;

namespace AOI
{
    /// <summary>
    /// 表示一个AOI区域
    /// </summary>
    public class AOI_Area
    {
        /// <summary>
        /// 获取对应obj的观察者
        /// </summary>
        public int[] GetObserveArray( int id_ )
        {
            if ( !_observe.TryGetValue( id_, out var set ) )
                return new int[0];

            return new List<int>( set ).ToArray();
        }

        /// <summary>
        /// 获取区域内所有agent
        /// </summary>
        public int[] GetAllAgents()
        {
            return new List<int>( _curr_area_objs ).ToArray();
        }

        /// <summary>
        /// 从AOI区域内移除对象
        /// </summary>
        public bool RemoveAgent( int id )
        {
            return _curr_area_objs.Remove( id );
        }

        /// <summary>
        /// 添加对象到AOI区域内
        /// </summary>
        public bool AddAgent( int id )
        {
            if ( _curr_area_objs.Contains( id ) )
            {
                Debug.Log( $"<color=yellow>_curr_area_objs.Contains( id ),id = {id}</color>" );
                return false;
            }

            return _curr_area_objs.Add( id );
        }

        /// <summary>
        /// 添加一个agent到该区域
        /// </summary>
        /// <param name="id_">要添加到该区域的agentid</param>
        /// <param name="observe_to_add_">要为该agent添加的观察者</param>
        /// <param name="observing_to_add_">要为该agent添加的被观察者</param>
        public bool AddAgent( int id_, IEnumerable<int> observe_to_add_, IEnumerable<int> observing_to_add_ ) 
        {
            if ( _observing.ContainsKey( id_ ) || _observe.ContainsKey( id_ ) )
            {
                Debug.LogWarning( $"_observing.ContainsKey( id_ ) || _observe.ContainsKey( id_ ),id={id_}" );
                return false;
            }

            _observing.Add( id_, new HashSet<int>( observing_to_add_ ) );
            _observe.Add( id_, new HashSet<int>( observe_to_add_ ) );
            return true;
        }

        public bool AddObserve( int id_, int to_add_ )
        {
            if ( !_curr_area_objs.Contains( id_ ) )
            {
                Debug.Log( $"!_curr_area_objs.Contains( id_ ),id = {id_}" );
                return false;
            }

            if ( !_observe.ContainsKey( id_ ) )
                _observe.Add( id_, new HashSet<int>() {  to_add_} );
            else
                return _observe[id_].Add( to_add_ );

            return true;
        }

        public bool AddObserving( int id_, int to_add_ )
        {
            if ( !_curr_area_objs.Contains( id_ ) )
            {
                Debug.Log( $"!_curr_area_objs.Contains( id_ ),id = {id_}" );
                return false;
            }

            if ( !_observing.ContainsKey( id_ ) )
                _observing.Add( id_, new HashSet<int>() { to_add_ } );
            else
                return _observing[id_].Add( to_add_ );

            return true;
        }

        /// <summary>
        /// 获取指定对象的观察者
        /// </summary>
        public HashSet<int> GetObserve( int id_ )
        {
            _observe.TryGetValue( id_, out var set );
            if ( set is null )
                set = new HashSet<int>();

            return set;
        }

        /// <summary>
        /// 获取指定对象的被观察者
        /// </summary>
        public HashSet<int> GetObserved( int id_ )
        {
            _observing.TryGetValue( id_, out var set );
            if ( set is null )
                set = new HashSet<int>();

            return set;
        }

        /// <summary>
        /// 移除一个对象的所有观察者
        /// </summary>
        public bool RemoveObserve( int id_ )
        {
            return _observe.Remove(id_ );
        }

        /// <summary>
        /// 移除一个对象的所有被观察者
        /// </summary>
        public bool RemoveObserving( int id_ )
        {
            return _observing.Remove(id_ );
        }

        public AOI_Area(int x,int y)
        {
            _observe        = new Dictionary<int, HashSet<int>>();
            _observing      = new Dictionary<int, HashSet<int>>();
            _curr_area_objs = new HashSet<int>();
            AOIZoneCoord    = new Vector2Int( x, y );
        }

        /// <summary>
        /// 对应ID对象的观察者
        /// </summary>
        private Dictionary<int, HashSet<int>> _observe;

        /// <summary>
        /// 对应ID对象的被观察者
        /// </summary>
        private Dictionary<int, HashSet<int>> _observing;

        /// <summary>
        /// 当前场景内的对象ID集合
        /// </summary>
        private HashSet<int> _curr_area_objs = null;

        /// <summary>
        /// AOI空间下的世界坐标
        /// </summary>
        public Vector2Int AOIZoneCoord { get; private set; }
    }
}
