// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="Settings.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using UnityEngine;
/// <summary>
/// Class Settings.
/// </summary>
public class Settings
	{

    //The path to sprites folder
    /// <summary>
    /// The sprites paths
    /// </summary>
    public const string SpritesPaths = "Sprites/";
    //The path to icon folder
    /// <summary>
    /// The icons paths
    /// </summary>
    public const string IconsPaths = "Sprites/Icons/";
    //The path to prefab folder
    /// <summary>
    /// The prefabs paths
    /// </summary>
    public const string PrefabsPath = "Prefabs/";
    //The path to Sounds folder
    /// <summary>
    /// The Sounds paths
    /// </summary>
    public const string SoundPath = "Sounds/";
    //The path to battle begins music
    /// <summary>
    /// The  battle begins music name
    /// </summary>
    public const string BattleBeginsMusic = "Bumblebee loop";
    //The path to game over music
    /// <summary>
    /// The path to game over music
    /// </summary>
    public const string GameOverMusic = "softpiano20";
    // The path to winning music
    /// <summary>
    /// The path to winning music
    /// </summary>
    public const string WinningMusic = "Bouncy";
    //The  chat sound name
    /// <summary>
    /// The  chat sound name
    /// </summary>
    public const string ChatSound = "metalLatch";
    // The  Weapon sound name
    /// <summary>
    /// The  Weapon sound name
    /// </summary>
    public const string WeaponSound = "swish-10";
    // The  Item sound name
    /// <summary>
    /// The  Item sound name
    /// </summary>
    public const string ItemSound = "spell3";
    //The path to battle begins music
    /// <summary>
    /// The  battle begins music name
    /// </summary>
    public const string UIClickSound = "swish-10";
    //The path to magic aura effect
    /// <summary>
    /// The magic aura effect
    /// </summary>
    public const string MagicAuraEffect = "Magic_Aura";
    //The amount of time that the controls are locked whenthe menus are displayed
    /// <summary>
    /// The inputlocking time
    /// </summary>
    public const float InputlockingTime = 0.2f;
    //The amount of time the text dialog paused when typing leters, reduce the value to make dialogs faster
    /// <summary>
    /// The letter pause
    /// </summary>
    public const float LetterPause = 0.01f;
    //Number of characters by dialog page
    /// <summary>
    /// The dialog characters number
    /// </summary>
    public const int DialogCharactersNumber = 100;

    /*
    / This is the Input configuration
	*/
    /// <summary>
    /// The move to right input
    /// </summary>
    public const KeyCode MoveToRightInput = KeyCode.RightArrow;
    /// <summary>
    /// The move to left input
    /// </summary>
    public const KeyCode MoveToLeftInput = KeyCode.LeftArrow;
    /// <summary>
    /// The move to up input
    /// </summary>
    public const KeyCode MoveToUpInput = KeyCode.UpArrow;
    /// <summary>
    /// The move to down input
    /// </summary>
    public const KeyCode MoveToDownInput = KeyCode.DownArrow;
    /// <summary>
    /// The menu input
    /// </summary>
    public const KeyCode MenuInput = KeyCode.Escape;
    /// <summary>
    /// The interract input
    /// </summary>
    public const KeyCode InterractInput = KeyCode.Space;



    /*
    /  TAGS
	*/
    /// <summary>
    /// The player
    /// </summary>
    public const string Player = "Player";
    /// <summary>
    /// The NPC
    /// </summary>
    public const string NPC = "NPC";
    /// <summary>
    /// The UI
    /// </summary>
    public const string UI = "UI";
    /// <summary>
    /// The UI for Battle
    /// </summary>
    public const string UIBattle = "UIBattle";
    /// <summary>
    /// The logic
    /// </summary>
    public const string Logic = "Logic";
    /// <summary>
    /// The Music 
    /// </summary>
    public const string Music = "Music";

    /*
    /  Misc
	*/
    /// <summary>
    /// The clone name
    /// </summary>
    public const string CloneName = "(Clone)";

    /*
	 / Filename to save
	 */

    /// <summary>
    /// The saved file name
    /// </summary>
    public const string SavedFileName = "SavedGame.game";

    /*
	 * MainMenuScene
	 */
    /// <summary>
    /// The main menu scene
    /// </summary>
    public const string MainMenuScene = "Main";
    /// <summary>
    /// The loader scene
    /// </summary>
    public const string LoaderScene = "Loader";



	}
