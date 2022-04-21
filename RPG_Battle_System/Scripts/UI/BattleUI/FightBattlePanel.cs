// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="FightBattlePanel.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;

/// <summary>
/// Class FightBattlePanel.
/// </summary>
public class FightBattlePanel : MonoBehaviour {

    /// <summary>
    /// The actual battle action
    /// </summary>
    public EnumFightMenuAction ActualBattleAction   ;

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
    public void ChangeEnumCharacterAction (EnumFightMenuAction action)
	{ 
		if (ActualBattleAction != action) {
			ActualBattleAction = action;
			SendMessage (string.Format ("{0}", ActualBattleAction));
		}
	}


    /// <summary>
    /// Swords action.
    /// </summary>
    void Sword ()
	{	SendMessageUpwards("DisplayPanel",EnumFightMenuAction.Sword);
	}
	

}
