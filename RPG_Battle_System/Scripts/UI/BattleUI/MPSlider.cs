// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="MPSlider.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Class MPSlider.
/// </summary>
[RequireComponent(typeof(Slider))]
public class MPSlider : MonoBehaviour {

    /// <summary>
    /// The mp slider
    /// </summary>
    private Slider mpSlider;

    // Use this for initialization
    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake () {
		mpSlider = GetComponent<Slider> ();
	}


    /// <summary>
    /// Sets the mp value.
    /// </summary>
    /// <param name="value">The value.</param>
    void SetMPValue(int value)
	{
		if (mpSlider != null)
			mpSlider.value = value;
	}
}
