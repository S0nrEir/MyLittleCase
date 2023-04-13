using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AOI
{
    /// <summary>
    /// ��ʾ��ʵ������һ������ڵ�
    /// </summary>
    public struct Scene_Node
    {

        /// <summary>
        /// ����
        /// </summary>
        public void Setup(Vector2Int coord_,Vector2 worldPos_)
        {
            Coord = coord_;
            WorldPos = worldPos_;
            _block = false;
            _has_player = false;
        }

        /// <summary>
        /// ���ռλ���
        /// </summary>
        public bool _has_player;

        /// <summary>
        /// ����ͨ��
        /// </summary>
        public bool _block;

        /// <summary>
        /// ���񻯵�ͼ����
        /// </summary>
        public Vector2Int Coord { get; private set; }

        /// <summary>
        /// ����λ��
        /// </summary>
        public Vector2 WorldPos { get; private set; }
    }
}
