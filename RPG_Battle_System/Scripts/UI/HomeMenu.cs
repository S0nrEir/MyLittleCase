// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-07-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="HomeMenu.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Class HomeMenu.
/// </summary>
public class HomeMenu : MonoBehaviour {
    /// <summary>
    /// The newgame toggle
    /// </summary>
    public Toggle NewgameToggle;
    /// <summary>
    /// The scene to load for new game
    /// </summary>
    public String SceneToLoadForNewGame;
   
    // Use this for initialization
    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start () {
		Datas.PopulateDatas ();
       
    }



    /// <summary>
    /// This procedure check the no toggle control and send action to the action variable
    /// <param name="toggle">The toggle that sent the action</param>
    /// </summary>
    /// <param name="toggle">The toggle.</param>
    public void NewgameToggleChoice(Toggle toggle)
	{
		Contract.Requires<MissingComponentException> (toggle != null);
        SoundManager.UISound();
        if (toggle.isOn) {
            gameObject.SetActive (false); 
			Main.CurrentScene = SceneToLoadForNewGame;
			SceneManager.LoadScene (SceneToLoadForNewGame);
		}

	}

    /// <summary>
    /// This procedure check the no toggle control and send action to the action variable
    /// <param name="toggle">The toggle that sent the action</param>
    /// </summary>
    /// <param name="toggle">The toggle.</param>
    public void ContinueToggleChoice(Toggle toggle)
	{
		Contract.Requires<MissingComponentException> (toggle != null);
        SoundManager.UISound();
        if (toggle.isOn) {
            gameObject.SetActive (false); 
			Main.Load ();

		}

	}



}
