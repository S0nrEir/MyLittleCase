using System.Collections.Generic;
using UnityEngine;

namespace AOI
{
    public class AOI_Zone
    {
        /// <summary>
        /// Agent��AOI�������ƶ�
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

            //����۲���
            var watchers = area.GetObserve( player_id_ );
            IAOI_Agent temp_agent = null;
            var iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( player_id_, out temp_agent );
                temp_agent?.Move( agent_ );
            }

            //�����۲���
            watchers = area.GetObserved( player_id_ );
            iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( player_id_, out temp_agent );
                if ( temp_agent != null )
                    agent_.Move( temp_agent );
            }
            //��鴩ԽAOI�߽�
            if ( TryGetArea( to_.x, to_.y, out var target_area ) )
            {
                if ( target_area == area )
                    return;

                //�뿪��ǰAOI���򣬽����µ�AOI����
                //��ǰ����leave
                Leave( agent_, from_.x, from_.y, player_id_, scene_ );
                Enter( agent_, from_.x, from_.y, player_id_, scene_ );
            }
        }
        //#todo���õ����AOI�����ڵ����ж������֪������ҽ���������ʱ��Ҫ�����Щ�۲��ߺͱ��۲���
        //��Ϊ���ǵ���Ҳ�����Ҫֱ����Ӹ������������ң����ܻ���һЩɸѡ�������е����еĲ��ã�
        //����������Ҳ��������չ��������Scene��������ƶ���ʱ���ȷ��Ҫ�����Щ�۲���
        //Ŀǰ�뵽�İ취��AOI_Area�ṩһ������ӿڱ�ʾ��ǰ�����ڵ��������ID
        //Scene�����߼���ʱ������ЩID��Ȼ����ɸѡ����ΪҪ��ӵĹ۲��ߡ�
        //����ǻ��ڶ�������ľŹ���Χ�Ļ����ڽ�����뿪��ʱ���õ������ȽϽ���Ĳ����Ϊ������뿪Ҫ֪ͨ�ı��۲��ߺ͹۲���
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

            //����۲���
            var watchers = area.GetObserve( player_id_ );
            IAOI_Agent temp_agent = null;
            var iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( player_id_, out temp_agent );
                temp_agent?.Enter( agent_ );
            }

            //�����۲���
            watchers = area.GetObserved( player_id_ );
            iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( player_id_, out temp_agent );
                if ( temp_agent != null )
                    agent_.Exit( temp_agent );
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

            //����۲���
            var watchers = area.GetObserve( player_id_ );
            IAOI_Agent temp_agent = null;
            var iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( player_id_, out temp_agent );
                temp_agent?.Exit( agent_ );
            }

            //�����۲���
            watchers = area.GetObserved( player_id_ );
            iter = watchers.GetEnumerator();
            while ( iter.MoveNext() )
            {
                scene_.TryGetAOIAgent( player_id_, out temp_agent );
                if ( temp_agent != null )
                    agent_.Exit( temp_agent );
            }
            area.RemoveAgent( player_id_ );
        }

        /// <summary>
        /// ���������ȡһ��Area
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
            if ( IsOutOfRange( y_ ) )
                return -1;

            return y_ / _size;
        }

        private int X( int x_ )
        {
            if ( IsOutOfRange( x_ ) )
                return -1;

            return x_ / _size;
        }

        /// <summary>
        /// Խ��
        /// </summary>
        private bool IsOutOfRange( int x, int y )
        {
            return x >= _size || x < 0 || y >= _size || y < 0;
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
                var cnt = temp.Count;

                for ( var j = 0; j < cnt; j++ )
                    temp[j] = new AOI_Area();

            }//end for
        }

        private int _size = 0;
        private List<AOI_Area>[] _areas = null;
    }
}
