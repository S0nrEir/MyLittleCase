// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="ItemsUI.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Class ItemsUI.
/// </summary>
public class ItemsUI : MonoBehaviour {
    /// <summary>
    /// The name
    /// </summary>
    public  Text Name;
    /// <summary>
    /// The price
    /// </summary>
    public Text Price;
    /// <summary>
    /// The icon
    /// </summary>
    public Image Icon;
    /// <summary>
    /// The toggle
    /// </summary>
    public Toggle Toggle;
    /// <summary>
    /// The description
    /// </summary>
    public string Description;
    /// <summary>
    /// The item data
    /// </summary>
    public ItemsData ItemData;
}
