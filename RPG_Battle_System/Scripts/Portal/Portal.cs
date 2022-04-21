// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-17-2016
// ***********************************************************************
// <copyright file="Portal.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Class Portal.
/// </summary>
public class Portal : MonoBehaviour {

    /// <summary>
    /// The scene to load
    /// </summary>
    public string SceneToLoad;




    /// <summary>
    /// Called when [trigger enter2 d].
    /// </summary>
    /// <param name="collider">The collider.</param>
    void OnTriggerEnter2D (Collider2D collider)
	{
        if (collider.tag == Settings.Player && !Main.JustEnteredTheScreen)
        {
            Main.PreviousScene = SceneManager.GetActiveScene().name;
            Main.CurrentScene = SceneToLoad;
            SceneManager.LoadScene(SceneToLoad, LoadSceneMode.Single);
            Main.JustEnteredTheScreen = true;
            Main.PlayerPosition = default(Vector3);

        }
      
	}

    /// <summary>
    /// Called when [trigger exit2 d].
    /// </summary>
    /// <param name="collider">The collider.</param>
    void OnTriggerExit2D (Collider2D collider)
	{
		if (collider.tag == Settings.Player) {
			Main.JustEnteredTheScreen = false;
		}
	}
}
