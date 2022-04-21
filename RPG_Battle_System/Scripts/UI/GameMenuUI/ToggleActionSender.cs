// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="ToggleActionSender.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Class ToggleActionSender.
/// </summary>
[RequireComponent(typeof(Toggle))]
public class ToggleActionSender : MonoBehaviour {

    /// <summary>
    /// The function to call
    /// </summary>
    string functionToCall = "ChangeEnumCharacterAction";
    /// <summary>
    /// The action to send
    /// </summary>
    public EnumBattleAction ActionToSend = default(EnumBattleAction);
    /// <summary>
    /// The attached toggle
    /// </summary>
    Toggle attachedToggle;
    // Use this for initialization
    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start () {
		attachedToggle = gameObject.GetComponent<Toggle> ();
		attachedToggle.onValueChanged.AddListener ((value) => { 
			if(attachedToggle.isOn)ControllerMethodToCall (value);   
		});
}



    /// <summary>
    /// Controllers the method to call.
    /// </summary>
    /// <param name="value">if set to <c>true</c> [value].</param>
    void ControllerMethodToCall(bool value)
		{
		SendMessageUpwards (functionToCall,ActionToSend);

		}



}
