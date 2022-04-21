// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-21-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-24-2016
// ***********************************************************************
// <copyright file="SoundManager.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;

/// <summary>
/// Class SoundManager.
/// </summary>
public class SoundManager : MonoBehaviour {


    // Use this for initialization
    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start () {
	
	}

   
    /// <summary>
    /// This variable indicate if the music should restart from the begining even if it's the same as the previous scene
    /// </summary>
    public bool IgnorePreviousSceneMusic = false;

    /// <summary>
    /// The current singleton instance
    /// </summary>
    private static SoundManager _instance;

     /// <summary>
    /// The property of the current singleton instance
    /// </summary>
    public static SoundManager instance
    {
        get
        {
           return _instance;
        }
    }

    // Awake is called in the initialization 
    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake()
    {
         if (_instance == null )
        {
            //If I am the first instance, make me the Singleton
            _instance = this;
            DontDestroyOnLoad(this);
         }
        else
        {
            var audiosource = GetComponent<AudioSource>();
            var oldAudioSource = instance.GetComponent<AudioSource>();

            //If the music change destroy the old singleton
            if (oldAudioSource.clip.name != audiosource.clip.name || IgnorePreviousSceneMusic)
            {
                Destroy(instance.gameObject);
                _instance = this;
               
            }
            //If a Singleton already exists and you find
            //another reference in scene, destroy it!
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }


    // Update is called once per frame
    /// <summary>
    /// Updates this instance.
    /// </summary>
    void Update () {
	
	}

    /// <summary>
    /// Plays the one shot.
    /// </summary>
    /// <param name="soundName">Name of the sound.</param>
    /// <param name="volume">The volume.</param>
    public void PlayOneShot(string soundName,float volume = -1)
    {
        var instantiatedSound = Resources.Load<AudioClip>( Settings.SoundPath + soundName);
        if (volume >= 0) instance.gameObject.GetComponent<AudioSource>().PlayOneShot(instantiatedSound, volume);
        else instance.gameObject.GetComponent<AudioSource>().PlayOneShot(instantiatedSound);
      
    }

    /// <summary>
    /// Plays the background music.
    /// </summary>
    /// <param name="musicTitle">The music title.</param>
    /// <param name="volume">The volume.</param>
    public void PlayBackgroundMusic(string musicTitle, float volume = -1)
    {
        var instantiatedSound = Resources.Load<AudioClip>( Settings.SoundPath + musicTitle);
        if (volume >= 0) instance.gameObject.GetComponent<AudioSource>().volume = volume;
        instance.gameObject.GetComponent<AudioSource>().clip = instantiatedSound;
        instance.gameObject.GetComponent<AudioSource>().Play();
      
    }

    /// <summary>
    /// Statics the play one shot.
    /// </summary>
    /// <param name="soundName">Name of the sound.</param>
    /// <param name="position">The position.</param>
    /// <param name="volume">The volume.</param>
    public static void StaticPlayOneShot(string soundName,Vector3 position, float volume = -1)
    {

        var instantiatedSound = Resources.Load<AudioClip>( Settings.SoundPath + soundName);
        var lastTimeScale= Time.timeScale;
        Time.timeScale = 1f;
        if (volume >= 0) AudioSource.PlayClipAtPoint(instantiatedSound, position, volume);
        else AudioSource.PlayClipAtPoint(instantiatedSound, position);
        Time.timeScale = lastTimeScale;
    }

    /// <summary>
    /// Playing UI sound.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public static void UISound(float volume = -1)
    {
        SoundManager.StaticPlayOneShot(Settings.ChatSound, Vector3.zero, volume);
    }

    /// <summary>
    /// Playing Chat sound.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public static void ChatSound(float volume = -1)
    {
        SoundManager.StaticPlayOneShot(Settings.ChatSound, Vector3.zero, volume);
    }

    /// <summary>
    /// Playing Weapon sound.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public static void WeaponSound(float volume = -1)
    {
        SoundManager.StaticPlayOneShot(Settings.WeaponSound, Vector3.zero, volume);
    }


    /// <summary>
    /// Playing Item sound.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public static void ItemSound(float volume = -1)
    {
        SoundManager.StaticPlayOneShot(Settings.ItemSound, Vector3.zero, volume);
    }

    /// <summary>
    /// Playing Gam Over sound.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public static void GameOverMusic(float volume = -1)
    {
        SoundManager.StaticPlayOneShot(Settings.GameOverMusic, Vector3.zero, volume);
    }

    /// <summary>
    /// Playing winning sound.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public static void WinningMusic(float volume = -1)
    {
        SoundManager.StaticPlayOneShot(Settings.WinningMusic, Vector3.zero, volume);
    }


    /// <summary>
    /// Battle begins sound.
    /// </summary>
    /// <param name="volume">The volume.</param>
    public static void BattleBeginsMusic(float volume = -1)
    {
        SoundManager.StaticPlayOneShot(Settings.BattleBeginsMusic, Vector3.zero, 1);
    }
    
}




