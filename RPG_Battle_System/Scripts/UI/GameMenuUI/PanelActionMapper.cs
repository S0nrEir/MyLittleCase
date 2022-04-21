// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-12-2016
// ***********************************************************************
// <copyright file="PanelActionMapper.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;

//Without serialisation this item will not be displayed
/// <summary>
/// Struct PanelActionMapper
/// </summary>
[System.Serializable]
public struct PanelActionMapper
{
    /// <summary>
    /// The menu action
    /// </summary>
    public EnumWorldMenudAction MenuAction;
    /// <summary>
    /// The panel
    /// </summary>
    public GameObject Panel;
}