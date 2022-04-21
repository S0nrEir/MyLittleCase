// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-20-2016
// ***********************************************************************
// <copyright file="BattleAnimator.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Class BattleAnimator.
/// </summary>
[RequireComponent(typeof(Animator), typeof(CharacterController))]
public class BattleAnimator : MonoBehaviour
{
    /// <summary>
    /// The _animator
    /// </summary>
    Animator _animator;
    /// <summary>
    /// The animation states
    /// </summary>
    Dictionary<string, String> animationStates = new Dictionary<string, string> ();

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake ()
	{
		_animator = GetComponent<Animator> ();
		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Idle, EnumSide.Down), "Player_Idle_Down");
		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Idle, EnumSide.Left), "Player_Idle_Right");
		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Idle, EnumSide.Right), "Player_Idle_Right");
		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Idle, EnumSide.Up), "Player_Idle_Up");

		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Walking, EnumSide.Down), "Player_Walking_Down");
		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Walking, EnumSide.Right), "Player_Walking_Right");
		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Walking, EnumSide.Left), "Player_Walking_Right");
		animationStates.Add (string.Format ("{0}{1}", EnumCharacterState.Walking, EnumSide.Up), "Player_Walking_Up");
	}


    /// <summary>
    /// Animates the specified state.
    /// </summary>
    /// <param name="state">The state.</param>
    void Animate (string state)
	{
		Debug.Log (state);
		string s;
		animationStates.TryGetValue (state, out s);
		if (s != default(string))
			_animator.Play (s);
	}


}