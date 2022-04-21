// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 08-15-2015
// ***********************************************************************
// <copyright file="ActiveStateToggler.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;

/// <summary>
/// Class ActiveStateToggler.
/// </summary>
public class ActiveStateToggler : MonoBehaviour {

    /// <summary>
    /// Toggles the active.
    /// </summary>
    public void ToggleActive () {
		gameObject.SetActive (!gameObject.activeSelf);
	}
}
