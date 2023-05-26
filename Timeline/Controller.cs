using UnityEngine;

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

