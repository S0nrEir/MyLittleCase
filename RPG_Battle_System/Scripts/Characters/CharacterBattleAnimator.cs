// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-11-2016
// ***********************************************************************
// <copyright file="CharacterAnimator.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class CharacterAnimator.
/// </summary>
[RequireComponent(typeof(Animator))]
public class CharacterBattleAnimator : MonoBehaviour
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
		animationStates[string.Format ("{0}", EnumBattleState.Idle)]=name + "_Idle";
		animationStates[string.Format ("{0}", EnumBattleState.Attack)]= name + "_Attack";
        animationStates[string.Format ("{0}", EnumBattleState.Magic)]= name + "_Magic";
		animationStates[string.Format ("{0}", EnumBattleState.Hit)]= name + "_Hit";
	}

    /// <summary>
    /// Animates the specified state.
    /// </summary>
    /// <param name="state">The state.</param>
    public void Animate (string state)
	{
        
		string s;
		animationStates.TryGetValue (state, out s);
		if (s != default(string))
			{_animator.Play(s.Replace("(Clone)", string.Empty) );
            Debug.Log(s.Replace("(Clone)", string.Empty) );
            }
	}

    /// <summary>
    /// Stops this instance.
    /// </summary>
    public void Stop()
    {
        _animator.enabled = false;
    }


}