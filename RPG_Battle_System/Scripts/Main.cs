// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 03-27-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-17-2016
// ***********************************************************************
// <copyright file="Main.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Security.Permissions;

/// <summary>
/// Class Main.
/// </summary>
[Serializable ()]
public class Main : MonoBehaviour,ISerializable		
{
    /// <summary>
    /// The first scene to load
    /// </summary>
    public string FirstSceneToLoad;
    /// <summary>
    /// The first scene loaded
    /// </summary>
    public static string FirstSceneLoaded;

    // Static singleton property
    /// <summary>
    /// The character list
    /// </summary>
    public static  List<CharactersData>  CharacterList  =new List<CharactersData> ();
    /// <summary>
    /// The equipment list
    /// </summary>
    public static  List<ItemsData>  EquipmentList  =new List<ItemsData> ();
    /// <summary>
    /// The item list
    /// </summary>
    public static  List<ItemsData>  ItemList  =new List<ItemsData> ();
     /// <summary>
    /// The previous scene
    /// </summary>
    public static  string  PreviousScene  =string.Empty;
    /// <summary>
    /// The current scene
    /// </summary>
    public static  string  CurrentScene  =string.Empty;
    //Cannot serialize Vector3 due to unity limits
    /// <summary>
    /// The player position
    /// </summary>
    [NonSerialized]
	public static  Vector3  PlayerPosition  =default(Vector3);
    /// <summary>
    /// The player x position
    /// </summary>
    public static  int PlayerXPosition  = default(int);
    /// <summary>
    /// The player y position
    /// </summary>
    public static  int PlayerYPosition  = default(int);
    /// <summary>
    /// The player z position
    /// </summary>
    public static  int PlayerZPosition  = default(int);
    /// <summary>
    /// The controls blocked
    /// </summary>
    public static  bool  ControlsBlocked  =false;
    /// <summary>
    /// The just entered the screen
    /// </summary>
    public static  bool  JustEnteredTheScreen  =false;

    /// <summary>
    /// Gets a value indicating whether this instance is game paused.
    /// </summary>
    /// <value><c>true</c> if this instance is game paused; otherwise, <c>false</c>.</value>
    public static  bool IsGamePaused { 
		get{ return Time.timeScale ==0; } 
	}

    /// <summary>
    /// Pauses the game.
    /// </summary>
    /// <param name="pause">if set to <c>true</c> [pause].</param>
    public static void PauseGame(bool pause)
	{
		if (pause)
			Time.timeScale = 0;
		else 
			Time.timeScale = 1;
	}

    /// <summary>
    /// Awakes this instance.
    /// </summary>
    void Awake()
	{
			//Affect the variable firstSceneLoaded to be accesed from any part of the game 
		FirstSceneLoaded = this.FirstSceneToLoad;
		
		// First we check if there are any other instances conflicting
		if(CharacterList != null || EquipmentList != null || ItemList != null  )
		{
			// If that is the case, we destroy other instances
			Destroy(gameObject);
		}



		PreviousScene  =string.Empty;
		CurrentScene  =string.Empty;
		PlayerPosition  =default(Vector3);
		PlayerXPosition  = default(int);
		PlayerYPosition  = default(int);
		PlayerZPosition  = default(int);
		ControlsBlocked  =false;
		JustEnteredTheScreen  =false;



		// Here we save our singleton instance
		CharacterList = new   List<CharactersData>()  ;
		EquipmentList = new List<ItemsData> ();
		ItemList = new List<ItemsData> ();
	
		//Initialize games datas
		Datas.PopulateDatas (); 

		//initialize data for a new game
		InitializeDatas ();

		// Furthermore we make sure that we don't destroy between scenes (this is optional)
		DontDestroyOnLoad(gameObject);

       SceneManager.LoadScene(FirstSceneLoaded); 
	}

    /// <summary>
    /// Initializes the player position.
    /// </summary>
    public static void InitializePlayerPosition()
	{
		PlayerPosition  =default(Vector3);
		PlayerXPosition  = default(int);
		PlayerYPosition  = default(int);
		PlayerZPosition  = default(int);
	}

    /// <summary>
    /// Initializes the battle data datas weapons /magic /items.
    /// </summary>
    public void InitializeDatas()
	{
        
		CharacterList.Add (Datas.CharactersData [1]);
		CharacterList.Add (Datas.CharactersData [2]);

		CharacterList [0].Head =Datas.ItemsData [12] ;
        EquipmentList.Add(Datas.ItemsData[12]);
		CharacterList [0].Body =Datas.ItemsData [22] ;
        EquipmentList.Add(Datas.ItemsData[22]);
        CharacterList [0].RightHand =Datas.ItemsData [1] ;
        EquipmentList.Add(Datas.ItemsData[1]);
        CharacterList [0].LeftHand =Datas.ItemsData [17] ;
        EquipmentList.Add(Datas.ItemsData[17]);
        CharacterList [0].SpellsList.AddRange(Datas.SpellsData.Where(w=>w.Value.AllowedCharacterType== EnumCharacterType.Warrior).Select(s=>s.Value));


		CharacterList [1].Head =Datas.ItemsData [15] ;
        EquipmentList.Add(Datas.ItemsData[15]);
        CharacterList [1].Body =Datas.ItemsData [26] ;
        EquipmentList.Add(Datas.ItemsData[26]);
        CharacterList [1].RightHand =Datas.ItemsData [9] ;
        EquipmentList.Add(Datas.ItemsData[9]);
        CharacterList [1].SpellsList.AddRange(Datas.SpellsData.Where(w=>w.Value.AllowedCharacterType== EnumCharacterType.Wizard).Select(s=>s.Value));

        foreach (var item in EquipmentList)
        {
            item.IsEquiped = true;
        }

        ItemList.Add(Datas.ItemsData[29]);
        ItemList.Add(Datas.ItemsData[30]);
        ItemList.Add(Datas.ItemsData[31]);
        
				
	}


    #region ISerializable implementation
    /// <summary>
    /// Initializes a new instance of the <see cref="Main"/> class.
    /// </summary>
    public Main (){}
    /// <summary>
    /// Initializes a new instance of the <see cref="Main"/> class.
    /// </summary>
    /// <param name="info">The information.</param>
    /// <param name="context">The context.</param>
    public Main (SerializationInfo info, StreamingContext context)
	{
		CharacterList =(List<CharactersData>) info.GetValue ("characterList",typeof(List<CharactersData>));
		EquipmentList  =(List<ItemsData>) info.GetValue ("equipmentList",typeof(List<ItemsData>)); 
		ItemList  =(List<ItemsData>) info.GetValue ("itemList",typeof(List<ItemsData>));
		PreviousScene  =string.Empty;
		CurrentScene  =(string) info.GetValue ("currentScene",typeof(string));
		PlayerXPosition = (int) info.GetValue ("playerXPosition",typeof(int));
		PlayerYPosition = (int) info.GetValue ("playerYPosition",typeof(int));
		PlayerZPosition = (int) info.GetValue ("playerZPosition",typeof(int));
		PlayerPosition = new Vector3 (PlayerXPosition, PlayerYPosition, PlayerZPosition);
		ControlsBlocked  =false;
		JustEnteredTheScreen  =false;
	}
#pragma warning disable  // Type or member is obsolete
                              /// <summary>
                              /// Gets the object data.
                              /// </summary>
                              /// <param name="info">The information.</param>
                              /// <param name="context">The context.</param>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
#pragma warning restore  // Type or member is obsolete
    public void GetObjectData (SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("characterList", CharacterList);
		info.AddValue ("equipmentList", EquipmentList); 
		info.AddValue ("itemList", ItemList);
		info.AddValue ("currentScene", CurrentScene);
		info.AddValue ("playerXPosition", PlayerPosition.x);
		info.AddValue ("playerYPosition", PlayerPosition.y);
		info.AddValue ("playerZPosition", PlayerPosition.z);

	}

    /// <summary>
    /// Saves this instance.
    /// </summary>
    public static void Save () {

#pragma warning disable
        Main data = new Main ();
#pragma warning restore 
        Stream stream = File.Open(Settings.SavedFileName, FileMode.Create);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder(); 
		Debug.Log ("Writing Information");
		bformatter.Serialize(stream,  data);
		stream.Close();

	}

    /// <summary>
    /// Loads this instance.
    /// </summary>
    public static void Load () {


		//Main data = new Main ();
		Stream stream = File.Open(Settings.SavedFileName, FileMode.Open);
		BinaryFormatter bformatter = new BinaryFormatter();
		bformatter.Binder = new VersionDeserializationBinder(); 
		Debug.Log ("Reading Data");
		bformatter.Deserialize(stream);
		stream.Close();

		if (!string.IsNullOrEmpty (Main.CurrentScene))
			SceneManager.LoadScene (Main.CurrentScene);
		else
			SceneManager.LoadScene (Settings.MainMenuScene);

	}


    /// <summary>
    /// Class VersionDeserializationBinder. This class cannot be inherited.
    /// </summary>
    public sealed class VersionDeserializationBinder : SerializationBinder 
	{
        /// <summary>
        /// Binds to type.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>Type.</returns>
        public override Type BindToType( string assemblyName, string typeName )
		{ 
			if ( !string.IsNullOrEmpty( assemblyName ) && !string.IsNullOrEmpty( typeName ) ) 
			{ 
				Type typeToDeserialize = null; 


				assemblyName = Assembly.GetExecutingAssembly().FullName; 
				// The following line of code returns the type. 
				typeToDeserialize = Type.GetType( String.Format( "{0}, {1}", typeName, assemblyName ) ); 
				return typeToDeserialize; 
			} 
			return null; 
		} 

	}

	#endregion
}