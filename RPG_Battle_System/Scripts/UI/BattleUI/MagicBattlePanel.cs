// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="MagicBattlePanel.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;

/// <summary>
/// Class MagicBattlePanel.
/// </summary>
public class MagicBattlePanel : MonoBehaviour {

    /// <summary>
    /// The actual battle action
    /// </summary>
    public EnumMagicMenuAction ActualBattleAction   ;

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
    public void ChangeEnumCharacterAction (EnumMagicMenuAction action)
	{ 
		if (ActualBattleAction != action) {
			ActualBattleAction = action;
			SendMessage (string.Format ("{0}", ActualBattleAction));
		}
	}


    /// <summary>
    /// Fireballs this instance.
    /// </summary>
    void Fireball ()
	{
		Debug.Log ("Fireball");
		SendMessageUpwards("DisplayPanel",EnumMagicMenuAction.Fireball);
	}
    /// <summary>
    /// Ices this instance.
    /// </summary>
    void Ice ()
	{
		Debug.Log ("Ice");
		SendMessageUpwards("DisplayPanel",EnumMagicMenuAction.Ice);
	}
	
}
