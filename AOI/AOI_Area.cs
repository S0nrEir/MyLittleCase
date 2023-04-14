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
        /// �Ӹ������Ƴ�һ��agent
        /// </summary>
        public void RemoveAgent( int id_ )
        {
            _observe.Remove( id_ );
            _observing.Remove( id_ );
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

        /// <summary>
        /// ��ȡָ������Ĺ۲���
        /// </summary>
        public HashSet<int> GetObserve( int id_ )
        {
            _observe.TryGetValue( id_, out var set );
            return set;
        }

        /// <summary>
        /// ��ȡָ������ı��۲���
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
        /// ��ӦID����Ĺ۲���
        /// </summary>
        private Dictionary<int, HashSet<int>> _observe;

        /// <summary>
        /// ��ӦID����ı��۲���
        /// </summary>
        private Dictionary<int, HashSet<int>> _observing;
    }
}
