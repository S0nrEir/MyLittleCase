// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="HPSlider.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Class HPSlider.
/// </summary>
[RequireComponent(typeof(Slider))]
public class HPSlider : MonoBehaviour {

    /// <summary>
    /// The hp slider
    /// </summary>
    private Slider hpSlider;

    // Use this for initialization
    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake () {
		hpSlider = GetComponent<Slider> ();
	}



    /// <summary>
    /// Sets the hp value.
    /// </summary>
    /// <param name="value">The value.</param>
    void SetHPValue(int value)
	{
		if (hpSlider != null)
			hpSlider.value = value;
		
	}
}
