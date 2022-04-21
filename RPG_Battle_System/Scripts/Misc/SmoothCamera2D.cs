// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="SmoothCamera2D.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using UnityEngine.UI;

/// <summary>
/// Class SmoothCamera2D.
/// </summary>
public class SmoothCamera2D : MonoBehaviour
{

    /// <summary>
    /// The damp time
    /// </summary>
    public float DampTime = 0.15f;
    /// <summary>
    /// The velocity
    /// </summary>
    Vector3 velocity = Vector3.zero;
    /// <summary>
    /// The target
    /// </summary>
    public Transform Target;
    /// <summary>
    /// The minx
    /// </summary>
    public float MINX = float.NegativeInfinity;
    /// <summary>
    /// The maxx
    /// </summary>
    public float MAXX = float.PositiveInfinity;
    /// <summary>
    /// The miny
    /// </summary>
    public float MINY = float.NegativeInfinity;
    /// <summary>
    /// The maxy
    /// </summary>
    public float MAXY = float.PositiveInfinity;


    // Update is called once per frame
    /// <summary>
    /// Updates this instance.
    /// </summary>
    void Update ()
	{
		if (Target) {
			Vector3 point = GetComponent<Camera> ().WorldToViewportPoint (Target.position);
			Vector3 delta = Target.position - GetComponent<Camera> ().ViewportToWorldPoint (new Vector3 (0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			destination = new Vector3(
				Mathf.Clamp(destination.x, MINX, MAXX),
				Mathf.Clamp(destination.y, MINY, MAXY),
				destination.z);
			
			transform.position = Vector3.SmoothDamp (transform.position, destination, ref velocity, DampTime);
		}
			
	}



}
	
