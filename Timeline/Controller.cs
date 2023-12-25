using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Timeline
{
    public class Controller : MonoBehaviour
    {
        //Track Group 将不同的轨道进行分类，相当于文件夹功能
        //Activation Track 控制物体的显示和隐藏
        //Animation Track 为物体加入动画，可以在场景中方便地录制动画，也可以是已经制作好的Animation Clip
        //Audio Track 为动画添加音效，并可对音效进行简单的裁剪和操作
        //Control Track 在该轨道上可以添加粒子效果，同时也可以添加子Timeline进行嵌套
        //Signal Track 信号轨道，可以发送信号，触发响应信号的函数调用
        //Playable Track 在该轨道中用户可以添加自定义的播放功能

        // Start is called before the first frame update
        void Start()
        {
            if ( _director.playableAsset == null || _director.playableAsset is not TimelineAsset)
                return;

            //自己创建timeline,track,clip
            var timeline = _director.playableAsset as TimelineAsset;
            //timeline.durationMode = TimelineAsset.DurationMode.FixedLength;
            //timeline.fixedDuration = 10f;
            ////动态创建一个timeline的track
            //var track = timeline.CreateTrack<PlayableTrack>( "CustomTrack" );
            ////在为一个track创建Clip时，可以直接调用CreateClip函数，它的参数接受ScriptableObject，也就是PlayableAsset类型
            ////并且在调用时，调用参数的CreatePlayable函数并返回一个对应的clip表示该片段
            ////在调用CreatePlayable时，返回一个PlayableBehaviour表示clip的行为处理逻辑，
            //var clip = track.CreateClip<MoveObjPlayableAsset>();
            //clip.duration = 5f;
            ////clip.asset表示该片段持有的playable数据实例
            ////设置该实例的信息数据
            //var asset = clip.asset as MoveObjPlayableAsset;
            //asset.go = _director_cube;
            //asset._startPos = new Vector3( -19.0100002f, 7.48000813f, 13.75f );
            //asset._targetPos = new Vector3( 50f, 50f, 0 );
            //asset._duration = (float)clip.duration;

            //使用现有的clip
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
            //    //找到相应的轨道
            //    var trackName = binding.streamName;
            //    if ( trackName.Equals( "CustomTrack" ) )
            //    {
            //        //找到轨道上所有的片段，并且将对应的片段转成想要的
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

