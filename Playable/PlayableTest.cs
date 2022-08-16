using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Playable
{
    /// <summary>
    ///  ����Playable
    /// </summary>
    public class PlayableTest : MonoBehaviour
    {
        private void Awake()
        {
        }

        /// <summary>
        /// Ĭ����ʾ
        ///    PlayableGraph
        ///         |
        ///   AnimationPlayableOutput
        ///         |
        /// AnimationClipPlayable
        /// </summary>
        private void Default()
        {
            //����playableGraph
            _graph = PlayableGraph.Create( "default_playable_graph" );
            _graph.SetTimeUpdateMode( DirectorUpdateMode.GameTime );
            //��Ϊ��Ҫ���Ŷ���������Ҫ��graph�����AnimationPlayableOutput
            var animation_output_playable = AnimationPlayableOutput.Create( _graph, "animation_playable_output", _animator );
            //���������Playable�ڵ㣬��Ϊ���漰����Ϻͷֲ㣬����ֻ��Ҫ���һ���򵥵�AnimationClipPlayable
            var clip_idle_playable = AnimationClipPlayable.Create( _graph, _walke_clip );
            //������playableOutput��playable�󣬽����ǹ�������
            animation_output_playable.SetSourcePlayable( clip_idle_playable );

            //��ʼ����
            _graph.Play();

            //Ҳ�����������ķ�ʽһ�����
            AnimationPlayableUtilities.PlayClip( _animator, _walke_clip, out _graph );
        }

        /// <summary>
        /// ʹ��BlendTree�Ķ�������
        /// PlayableGraph->AnimationPlayableOutput->MixerPlayable����Ҫ��ϵ�clip��ͨ��graph.Connect����
        /// </summary>
        private void BlendTransition()
        {
            _graph = PlayableGraph.Create( "default_playable_graph" );
            //ʹ��AnimationMixerPlayerʵ��BlendTree
            //�ڶ���������ʾҪ�������������л��
            _mixer_playable = AnimationMixerPlayable.Create( _graph, 2 );
            //�����ܺ��ߵ�animation
            var walk_clip_playable = AnimationClipPlayable.Create( _graph, _walke_clip );
            walk_clip_playable.SetSpeed( _walke_clip.length );

            var run_clip_playable = AnimationClipPlayable.Create( _graph, _run_clip );
            run_clip_playable.SetSpeed( _run_clip.length );

            //connect������������playableʵ����
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
        /// �����ֲ��ϣ�����ʵ�ֲ�ͬ��λ�ĸ��Ӷ�����ϣ������ϰ���ǹ���°����ƶ�
        /// </summary>
        private void LayerMixPlayable()
        {
            _graph = PlayableGraph.Create( "default_playable_graph" );
            var animation_output_playable = AnimationPlayableOutput.Create( _graph, "animation_output_playable", _animator );
            //����һ��������ϵ�playable,��������Ҫ��ϵĶ���
            var layer_mixer_playable = AnimationLayerMixerPlayable.Create( _graph, 2 );
            //������������
            var walk_clip_playable = AnimationClipPlayable.Create( _graph, _walke_clip );
            var eye_clip_playable = AnimationClipPlayable.Create( _graph, _eye_clip );

            _graph.Connect( walk_clip_playable, 0, layer_mixer_playable, 0 );
            _graph.Connect( eye_clip_playable, 0, layer_mixer_playable, 1 );

            animation_output_playable.SetSourcePlayable( layer_mixer_playable );
            //���һ������mask�����ݶ�Ӧ�Ĺ���transform��������layer�Ķ���
            var mask = new AvatarMask();
            mask.AddTransformPath( _head );
            //�����۲���������Ӧ��mask
            layer_mixer_playable.SetLayerMaskFromAvatarMask( 1, mask );
            //�ֱ������������������㣩��Ȩ��
            layer_mixer_playable.SetInputWeight( 0, 1 );
            layer_mixer_playable.SetInputWeight( 1, 1f );
            _graph.Play();
        }

        private void BlendTransitionUpdate()
        {
            //SetInputWeight�����趨ÿ���˿��ϵ�Ȩ��ֵ��Ҫע��������ж˿�Ȩ��֮�Ͳ��ܴ���1
            //0��walk
            //_graph.Connect( walk_clip_playable, 0, _mixer_playable, 0 );
            _mixer_playable.SetInputWeight( 0, 1f - _weight );
            //1��run
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

            //����������Ĺ���
            //BlendTransition();
            LayerMixPlayable();
        }



        private void Update()
        {
            //BlendTransitionUpdate();
        }

        /// <summary>
        /// ����Ȩ��
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
        /// playable graph���Կ�����animator��mananger��һ��playableGraph�������PlayableOutput������
        /// PlayableOutput����������Playable������Ч���������ܿ��ǣ�playableGraph���ǽṹ�塣
        /// </summary>
        private PlayableGraph _graph;
        [SerializeField] private Animator _animator = null;
    }
}
