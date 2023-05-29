using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline
{
    public class Controller : MonoBehaviour
    {
        //Track Group ����ͬ�Ĺ�����з��࣬�൱���ļ��й���
        //Activation Track �����������ʾ������
        //Animation Track Ϊ������붯���������ڳ����з����¼�ƶ�����Ҳ�������Ѿ������õ�Animation Clip
        //Audio Track Ϊ���������Ч�����ɶ���Ч���м򵥵Ĳü��Ͳ���
        //Control Track �ڸù���Ͽ����������Ч����ͬʱҲ���������Timeline����Ƕ��
        //Signal Track �źŹ�������Է����źţ�������Ӧ�źŵĺ�������
        //Playable Track �ڸù�����û���������Զ���Ĳ��Ź���

        // Start is called before the first frame update
        void Start()
        {
            //_source.Play();
            if ( _director.playableAsset == null || _director.playableAsset is not TimelineAsset)
                return;

            var timeline = _director.playableAsset as TimelineAsset;
            foreach ( var track in timeline.GetOutputTracks() )
            {
                if ( !track.name.Equals( "CustomTrack" ) )
                    continue;

                foreach ( var clip in track.GetClips() )
                {
                    if ( clip.asset is not MoveObjPlayableAsset )
                        continue;

                    var asset = clip.asset as MoveObjPlayableAsset;
                    asset.go = _director_cube;
                    asset._startPos = new Vector3( -19.0100002f, 7.48000813f, 13.75f );
                    asset._targetPos = new Vector3( 50f, 50f, 0 );
                    asset._duration = 5f;
                }
            }

            #region nouse
            //foreach ( var binding in timeline.outputs )
            //{
            //    //�ҵ���Ӧ�Ĺ��
            //    var trackName = binding.streamName;
            //    if ( trackName.Equals( "CustomTrack" ) )
            //    {
            //        //�ҵ���������е�Ƭ�Σ����ҽ���Ӧ��Ƭ��ת����Ҫ��
            //        var track = binding.sourceObject as TrackAsset;
            //        var clips = track.GetClips();
            //        foreach ( var clip in clips )
            //        {
            //            if ( clip.asset is MoveObjPlayableAsset)
            //            {
            //                var asset = clip.asset as MoveObjPlayableAsset;
            //                asset.go = _director_cube;
            //                asset.pos = new Vector3( 100f, 100f, 0 );

            //                break;
            //            }
            //        }
            //    }
            //}
            #endregion
            _director.Play();
        }

        private void Update()
        {
            //if ( Input.GetKeyDown( KeyCode.P ) )
            //    _source.PlayOneShot(_clip);
        }

        [SerializeField] private AudioSource _source = null;
        [SerializeField] private AudioClip _clip = null;
        [SerializeField] private PlayableDirector _director = null;
        [SerializeField] private GameObject _director_cube = null;
    }
}

