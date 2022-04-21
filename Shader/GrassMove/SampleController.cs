using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleController : MonoBehaviour
{
    // Start is called before the first frame update
    //void Start()
    //{

    //}

    private void OnEnable ()
    {
        //_characterController = GetComponent<CharacterController>();
        //if (_characterController == null)
        //{
        //    Debug.LogError("CharacterController is null!!!");
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey( KeyCode.W ))
            _moveDirection = Vector3.forward;
        else if (Input.GetKey( KeyCode.A ))
            _moveDirection = Vector3.left;
        else if (Input.GetKey( KeyCode.S ))
            _moveDirection = Vector3.back;
        else if (Input.GetKey( KeyCode.D ))
            _moveDirection = Vector3.right;

        Move( _moveDirection );
        _moveDirection = Vector3.zero;

        SetPlayerWorldPos2Shader();
    }

    private void Move (Vector3 direction)
    {
        _characterController.Move( direction * Time.deltaTime * _speed);
    }

    /// <summary>
    /// 将角色位置传递给GrassShader
    /// </summary>
    private void SetPlayerWorldPos2Shader ()
    {
        var pos = transform.position;
        Debug.Log( $"<color=white>player world position:{pos}</color>" );
        Shader.SetGlobalVector( "_PlayerPos", pos );
    }

    /// <summary>
    /// 角色控制器
    /// </summary>
    [SerializeField] private CharacterController _characterController;

    /// <summary>
    /// 速度
    /// </summary>
    [Range(0.5f,5f)][SerializeField] private float _speed = .5f;

    /// <summary>
    /// 移动方向
    /// </summary>
    private Vector3 _moveDirection = Vector3.zero;
}
