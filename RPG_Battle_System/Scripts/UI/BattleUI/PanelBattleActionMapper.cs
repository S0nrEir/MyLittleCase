// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-12-2016
// ***********************************************************************
// <copyright file="PanelBattleActionMapper.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;


//Without serialisation this item will not be displayed
/// <summary>
/// Struct PanelBattleActionMapper
/// </summary>
[System.Serializable]
public struct PanelBattleActionMapper
{
    /// <summary>
    /// The battle action
    /// </summary>
    public EnumBattleAction BattleAction;
    /// <summary>
    /// The panel
    /// </summary>
    public GameObject Panel;
}