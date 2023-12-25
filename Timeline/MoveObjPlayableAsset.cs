using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline
{
    //PlayableAsset表示所有Playable资源的数据来源（当然也包括timeline的）
    public class MoveObjPlayableAsset : PlayableAsset
    {
        public override UnityEngine.Playables.Playable CreatePlayable( PlayableGraph graph, GameObject owner )
        {
            var bhv = new MoveObjPlayableBehaviour();
            var go = GameObject.Find("Cube");

            bhv.go = go;
            bhv._startPos  = _startPos;
            bhv._targetPos = _targetPos;
            bhv._duration  = _duration;
            
            return ScriptPlayable<MoveObjPlayableBehaviour>.Create( graph, bhv );
        }

        public GameObject go;
        public Vector3 _startPos;
        public Vector3 _targetPos;
        public float _duration;
    }

}
