using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Timeline
{
    /// <summary>
    /// 
    /// </summary>
    public class MoveObjPlayableBehaviour : PlayableBehaviour
    {
        public override void OnGraphStart( UnityEngine.Playables.Playable playable )
        {
            base.OnGraphStart( playable );
            Debug.Log( "<color=white>MoveObjPlayableBehaviour.OnGraphStart()</color>" );
        }

        public override void OnGraphStop( UnityEngine.Playables.Playable playable )
        {
            base.OnGraphStop( playable );
            Debug.Log( "<color=white>MoveObjPlayableBehaviour.OnGraphStop()</color>" );
        }

        public override void OnBehaviourPlay( UnityEngine.Playables.Playable playable, FrameData info )
        {
            //base.OnBehaviourPlay( playable, info );
            //Debug.Log( "<color=white>MoveObjPlayableBehaviour.OnBehaviourPlay</color>" );
            //if ( go != null )
            //    go.transform.position = pos;

            go.transform.position = _startPos;
        }

        public override void OnBehaviourPause( UnityEngine.Playables.Playable playable, FrameData info )
        {
            base.OnBehaviourPause( playable, info );
            Debug.Log( "<color=white>MoveObjPlayableBehaviour.OnBehaviourPause</color>" );
            if ( go != null )
                go.transform.position = new Vector3( -19.0100002f, 7.48000813f, 13.75f );
        }

        //public override void OnBehaviourDelay( UnityEngine.Playables.Playable playable, FrameData info )
        //{
        //    Debug.Log( "<color=white>MoveObjPlayableBehaviour.OnBehaviourDelay()</color>" );
        //}

        public override void ProcessFrame( UnityEngine.Playables.Playable playable, FrameData info, object playerData )
        {
            base.ProcessFrame( playable, info, playerData );
            _timePassed += info.deltaTime;
            var percent = _timePassed / _duration;
            go.transform.position = Vector3.Lerp( _startPos, _targetPos, _timePassed / _duration );

        }

        public override void OnPlayableCreate( UnityEngine.Playables.Playable playable )
        {
            base.OnPlayableCreate( playable );
            _timePassed = 0f;
        }

        private float _timePassed;

        public float _duration;
        public GameObject go;
        public Vector3 _startPos;
        public Vector3 _targetPos;
    }

}
