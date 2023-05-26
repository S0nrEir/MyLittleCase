using UnityEngine;

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
            _source.Play();
        }

        private void Update()
        {
            if ( Input.GetKeyDown( KeyCode.P ) )
                _source.PlayOneShot(_clip);
        }

        [SerializeField] private AudioSource _source = null;
        [SerializeField] private AudioClip _clip = null;
    }
}

