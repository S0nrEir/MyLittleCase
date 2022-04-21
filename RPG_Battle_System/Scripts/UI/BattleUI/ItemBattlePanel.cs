// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="ItemBattlePanel.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;

/// <summary>
/// Class ItemBattlePanel.
/// </summary>
public class ItemBattlePanel : MonoBehaviour {

    /// <summary>
    /// The actual battle action
    /// </summary>
    public EnumItemMenuAction ActualBattleAction   ;

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake ()
	{
		SendMessage (string.Format ("{0}", ActualBattleAction));
	}



    /// <summary>
    /// Changes the enum character action.
    /// </summary>
    /// <param name="action">The action.</param>
    public void ChangeEnumCharacterAction (EnumItemMenuAction action)
	{ 
		if (ActualBattleAction != action) {
			ActualBattleAction = action;
			SendMessage (string.Format ("{0}", ActualBattleAction));
		}
	}


    /// <summary>
    /// Potions this instance.
    /// </summary>
    void Potion ()
	{
	SendMessageUpwards("DisplayPanel",EnumItemMenuAction.Potion);
	}

    /// <summary>
    /// Manas this instance.
    /// </summary>
    void Mana ()
	{
	SendMessageUpwards("DisplayPanel",EnumItemMenuAction.Mana);
	}
}
