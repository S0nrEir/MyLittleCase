using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        _cachedTran = GetComponent<Transform>();
        _cachedX = _cachedTran.position.x;
        _cachedY = _cachedTran.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        var z = Mathf.Sin( Time.fixedTime * 3f ) + _cachedTran.position.z;
        _cachedTran.position = new Vector3( _cachedX, _cachedY, z );
    }

    private float _cachedX;
    private float _cachedY;

    private Transform _cachedTran;
}
