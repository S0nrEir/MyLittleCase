using System;
using System.Collections.Generic;
using UnityEngine;

namespace AOI
{
    /// <summary>
    /// ��ʾһ��AOI����
    /// </summary>
    public class AOI_Area
    {
        /// <summary>
        /// ��ȡ��Ӧobj�Ĺ۲���
        /// </summary>
        public int[] GetObserveArray( int id_ )
        {
            if ( !_observe.TryGetValue( id_, out var set ) )
                return new int[0];

            return new List<int>( set ).ToArray();
        }

        /// <summary>
        /// ��ȡ����������agent
        /// </summary>
        public int[] GetAllAgents()
        {
            return new List<int>( _curr_area_objs ).ToArray();
        }

        /// <summary>
        /// ��AOI�������Ƴ�����
        /// </summary>
        public bool RemoveAgent( int id )
        {
            return _curr_area_objs.Remove( id );
        }

        /// <summary>
        /// ��Ӷ���AOI������
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
        /// ���һ��agent��������
        /// </summary>
        /// <param name="id_">Ҫ��ӵ��������agentid</param>
        /// <param name="observe_to_add_">ҪΪ��agent��ӵĹ۲���</param>
        /// <param name="observing_to_add_">ҪΪ��agent��ӵı��۲���</param>
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
        /// ��ȡָ������Ĺ۲���
        /// </summary>
        public HashSet<int> GetObserve( int id_ )
        {
            _observe.TryGetValue( id_, out var set );
            if ( set is null )
                set = new HashSet<int>();

            return set;
        }

        /// <summary>
        /// ��ȡָ������ı��۲���
        /// </summary>
        public HashSet<int> GetObserved( int id_ )
        {
            _observing.TryGetValue( id_, out var set );
            if ( set is null )
                set = new HashSet<int>();

            return set;
        }

        /// <summary>
        /// �Ƴ�һ����������й۲���
        /// </summary>
        public bool RemoveObserve( int id_ )
        {
            return _observe.Remove(id_ );
        }

        /// <summary>
        /// �Ƴ�һ����������б��۲���
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
        /// ��ӦID����Ĺ۲���
        /// </summary>
        private Dictionary<int, HashSet<int>> _observe;

        /// <summary>
        /// ��ӦID����ı��۲���
        /// </summary>
        private Dictionary<int, HashSet<int>> _observing;

        /// <summary>
        /// ��ǰ�����ڵĶ���ID����
        /// </summary>
        private HashSet<int> _curr_area_objs = null;

        /// <summary>
        /// AOI�ռ��µ���������
        /// </summary>
        public Vector2Int AOIZoneCoord { get; private set; }
    }
}
