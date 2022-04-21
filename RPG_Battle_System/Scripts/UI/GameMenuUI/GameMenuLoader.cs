// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-10-2016
// ***********************************************************************
// <copyright file="GameMenuLoader.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using System.Linq;

/// <summary>
/// Class GameMenuLoader.
/// </summary>
public class GameMenuLoader : MonoBehaviour {

    /// <summary>
    /// The map name
    /// </summary>
    public string MapName = string.Empty;

    // Use this for initialization
    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake () {
		
		var t =Resources.Load <GameObject> (Settings.PrefabsPath + Settings.UI);
		var o = Instantiate (t);
		var canvas = o.GetComponentsInChildren<Canvas>();
		foreach (Canvas canva in canvas) {
			canva.worldCamera= Camera.main;
		}

		o.BroadcastMessage ("ShowMapNamePopup", MapName);

		var player  = GameObject.FindGameObjectsWithTag(Settings.Player).FirstOrDefault();

		if(player)
		player.BroadcastMessage ("SetUIGameObject", o);
	}
	

}
