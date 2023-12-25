using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Playable
{
    /// <summary>
    ///  测试Playable
    /// </summary>
    public class PlayableTest : MonoBehaviour
    {
        private void Awake()
        {
        }

        /// <summary>
        /// 默认演示
        ///    PlayableGraph
        ///         |
        ///   AnimationPlayableOutput
        ///         |
        /// AnimationClipPlayable
        /// </summary>
        private void Default()
        {
            //创建playableGraph
            _graph = PlayableGraph.Create( "default_playable_graph" );
            _graph.SetTimeUpdateMode( DirectorUpdateMode.GameTime );
            //因为需要播放动画，所以要向graph中添加AnimationPlayableOutput
            var animation_output_playable = AnimationPlayableOutput.Create( _graph, "animation_playable_output", _animator );
            //接下来添加Playable节点，因为不涉及到混合和分层，所以只需要添加一个简单的AnimationClipPlayable
            var clip_idle_playable = AnimationClipPlayable.Create( _graph, _walke_clip );
            //创建了playableOutput和playable后，将他们关联起来
            animation_output_playable.SetSourcePlayable( clip_idle_playable );

            //开始播放
            _graph.Play();

            //也可以用这样的方式一行完成
            AnimationPlayableUtilities.PlayClip( _animator, _walke_clip, out _graph );
        }

        /// <summary>
        /// 使用BlendTree的动画过渡
        /// PlayableGraph->AnimationPlayableOutput->MixerPlayable持有要混合的clip，通过graph.Connect连接
        /// </summary>
        private void BlendTransition()
        {
            _graph = PlayableGraph.Create( "default_playable_graph" );
            //使用AnimationMixerPlayer实现BlendTree
            //第二个参数表示要将几个动画进行混合
            _mixer_playable = AnimationMixerPlayable.Create( _graph, 2 );
            //创建跑和走的animation
            var walk_clip_playable = AnimationClipPlayable.Create( _graph, _walke_clip );
            walk_clip_playable.SetSpeed( _walke_clip.length );

            var run_clip_playable = AnimationClipPlayable.Create( _graph, _run_clip );
            run_clip_playable.SetSpeed( _run_clip.length );

            //connect函数连接两个playable实例，
            _graph.Connect( walk_clip_playable, 0, _mixer_playable, 0 );
            _graph.Connect( run_clip_playable, 0, _mixer_playable, 1 );

            var animation_output_playable = AnimationPlayableOutput.Create( _graph, "animation_palyable_output", _animator );
            animation_output_playable.SetSourcePlayable( _mixer_playable );

            _graph.Play();
        }

        /// <summary>
        /// Playable+Animator
        /// </summary>
        private void PlayableAnimator()
        {
            
        }

        /// <summary>
        /// 动画分层混合，用于实现不同部位的复杂动画组合，比如上半身开枪，下半身移动
        /// </summary>
        private void LayerMixPlayable()
        {
            _graph = PlayableGraph.Create( "default_playable_graph" );
            var animation_output_playable = AnimationPlayableOutput.Create( _graph, "animation_output_playable", _animator );
            //创建一个动画混合的playable,传入两个要混合的动画
            var layer_mixer_playable = AnimationLayerMixerPlayable.Create( _graph, 2 );
            //创建两个动画
            var walk_clip_playable = AnimationClipPlayable.Create( _graph, _walke_clip );
            var eye_clip_playable = AnimationClipPlayable.Create( _graph, _eye_clip );

            _graph.Connect( walk_clip_playable, 0, layer_mixer_playable, 0 );
            _graph.Connect( eye_clip_playable, 0, layer_mixer_playable, 1 );

            animation_output_playable.SetSourcePlayable( layer_mixer_playable );
            //添加一个骨骼mask，根据对应的骨骼transform屏蔽其他layer的动画
            var mask = new AvatarMask();
            mask.AddTransformPath( _head );
            //设置眼部动画到对应的mask
            layer_mixer_playable.SetLayerMaskFromAvatarMask( 1, mask );
            //分别设置两个动画（两层）的权重
            layer_mixer_playable.SetInputWeight( 0, 1 );
            layer_mixer_playable.SetInputWeight( 1, 1f );
            _graph.Play();
        }

        private void BlendTransitionUpdate()
        {
            //SetInputWeight函数设定每个端口上的权重值，要注意的是所有端口权重之和不能大于1
            //0是walk
            //_graph.Connect( walk_clip_playable, 0, _mixer_playable, 0 );
            _mixer_playable.SetInputWeight( 0, 1f - _weight );
            //1是run
            //_graph.Connect( run_clip_playable, 0, _mixer_playable, 1 );
            _mixer_playable.SetInputWeight( 1, _weight );

            var totalSpeed = ( 1 - _weight ) * _walke_clip.length + _weight * _run_clip.length;
            _mixer_playable.SetSpeed( totalSpeed );
        }

        private void InitClip()
        {
            var runtime = _animator.runtimeAnimatorController;
            var clips = runtime.animationClips;
            if ( _idle_clip == null )
                _idle_clip = GetClip( "SLIDLE00", clips );

            if ( _walke_clip == null )
                _walke_clip = GetClip( "WALK00_F", clips );

            if ( _run_clip == null )
                _run_clip = GetClip( "RUN00_F", clips );

            if ( _eye_clip == null )
                _eye_clip = GetClip( "smile1@unitychan", clips );
        }

        private AnimationClip GetClip( string name, AnimationClip[] clips )
        {
            foreach ( var clip in clips )
            {
                if ( clip.name == name )
                    return clip;
            }
            return null;
        }

        private void Start()
        {
            InitClip();

            //Default();

            //动画混合树的过渡
            //BlendTransition();
            LayerMixPlayable();
        }



        private void Update()
        {
            //BlendTransitionUpdate();
        }

        /// <summary>
        /// 动画权重
        /// </summary>
        [Range( 0, 1 )]
        [SerializeField] private float _weight = 1f;

        private AnimationClip _eye_clip = null;
        private AnimationClip _walke_clip = null;
        private AnimationClip _idle_clip = null;
        private AnimationClip _run_clip = null;

        [SerializeField] Transform _head = null;

        private AnimationMixerPlayable _mixer_playable;

        /// <summary>
        /// playable graph可以看成是animator或mananger，一个playableGraph必须包含PlayableOutput，并且
        /// PlayableOutput必须连接至Playable才能生效，出于性能考虑，playableGraph都是结构体。
        /// </summary>
        private PlayableGraph _graph;
        [SerializeField] private Animator _animator = null;
    }
}
