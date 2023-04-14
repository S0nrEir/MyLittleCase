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
        /// 从该区域移除一个agent
        /// </summary>
        public void RemoveAgent( int id_ )
        {
            _observe.Remove( id_ );
            _observing.Remove( id_ );
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

        /// <summary>
        /// 获取指定对象的观察者
        /// </summary>
        public HashSet<int> GetObserve( int id_ )
        {
            _observe.TryGetValue( id_, out var set );
            return set;
        }

        /// <summary>
        /// 获取指定对象的被观察者
        /// </summary>
        public HashSet<int> GetObserved( int id_ )
        {
            _observing.TryGetValue( id_, out var set );
            return set;
        }

        public AOI_Area()
        {
            _observe = new Dictionary<int, HashSet<int>>();
            _observing = new Dictionary<int, HashSet<int>>();
        }

        /// <summary>
        /// 对应ID对象的观察者
        /// </summary>
        private Dictionary<int, HashSet<int>> _observe;

        /// <summary>
        /// 对应ID对象的被观察者
        /// </summary>
        private Dictionary<int, HashSet<int>> _observing;
    }
}
