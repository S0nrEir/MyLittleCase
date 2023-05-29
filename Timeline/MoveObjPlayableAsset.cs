using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline
{
    //PlayableAsset��ʾ����Playable��Դ��������Դ����ȻҲ����timeline�ģ�
    public class MoveObjPlayableAsset : PlayableAsset
    {
        public override UnityEngine.Playables.Playable CreatePlayable( PlayableGraph graph, GameObject owner )
        {
            var bhv = new MoveObjPlayableBehaviour();
            bhv.go = GameObject.Find("Cube");
            bhv._startPos = _startPos;
            bhv._targetPos = _targetPos;
            bhv._duration = _duration;
            return ScriptPlayable<MoveObjPlayableBehaviour>.Create( graph, bhv );
        }

        public GameObject go;
        public Vector3 _startPos;
        public Vector3 _targetPos;
        public float _duration;
    }

}
