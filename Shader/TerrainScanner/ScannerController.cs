using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScannerController : MonoBehaviour
{
    private void Init()
    {
        var volumComp = GetComponent<Volume>();
        if (volumComp == null)
        {
            Debug.LogError( "<color=red>failt to get volum component</color>" );
            return;
        }

        if (!volumComp.sharedProfile.TryGet( out _scannerComp))
        {
            Debug.LogError( "<color=red>failt to get scanner component from volumComp</color>" );
            return;
        }

        //_scannerComp = VolumeManager.instance.stack.GetComponent<TerrainScan_VolumComponent>();
        if (_scannerComp == null)
        {
            Debug.LogError( "<color=red>failt to get scannerComp</color>" );
        }
        _distance = 0f;
        
    }
    private void OnEnable ()
    {
        Init ();
    }

    private void Update ()
    {
        if (!_openFlag)
            return;

        _scannerComp._color.value = _color;
        time += Time.deltaTime;
        if (time >= _timerLmt)
        {
            //_openFlag = false;
            _distance = 0f;
            time = 0f;
            //return;
        }

        _distance += _speed * Time.deltaTime;
        _scannerComp._distance.value = _distance;
    }

    private float _distance = 0f;

    private TerrainScan_VolumComponent _scannerComp = null;

    private float time = 0f;
    [SerializeField] private float _timerLmt = 1f;
    [SerializeField] private Color _color = Color.white;
    [Range(0.01f, 0.1f)] [SerializeField] private float _speed = 0.4f;
    [SerializeField] private bool _openFlag = false;
}
