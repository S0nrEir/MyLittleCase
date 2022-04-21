// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-10-2016
// ***********************************************************************
// <copyright file="FadeOut.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using Holoville.HOTween;
using UnityEngine.UI;

/// <summary>
/// Class FadeOut.
/// </summary>
public class FadeOut : MonoBehaviour {


    /// <summary>
    /// The fade out time
    /// </summary>
    public float FadeOutTime=1f;//The fadeIn animation time

    // Update is called once per frame
    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start ()
	{
		StartCoroutine (Init ());
	}



    // Animate the Logos with fadeIn and fadeOut effect
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    /// <returns>IEnumerator.</returns>
    IEnumerator Init ()
	{    	
		// Set the texture so that it is the the size of the screen and covers it.
		transform.localScale= new Vector3( Screen.width, Screen.height,1);

		Sequence mySequence = new Sequence (new SequenceParms ());

		Color oldColor = GetComponent<Image>().color;

		GetComponent<Image>().color = new Color (oldColor.r, oldColor.b, oldColor.g, 1f);

		//TweenParms parms = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 1f)).Ease (EaseType.EaseInQuart);
		TweenParms parms2 = new TweenParms ().Prop ("color", new Color (oldColor.r, oldColor.b, oldColor.g, 0f)).Ease (EaseType.EaseInQuart);

		//	mySequence.Append (HOTween.To (GetComponent<Image>(), fadeOutTime, parms));
		mySequence.Append (HOTween.To (GetComponent<Image>(), FadeOutTime, parms2));


		mySequence.Play ();

		yield return new WaitForSeconds (FadeOutTime );


	}
}
