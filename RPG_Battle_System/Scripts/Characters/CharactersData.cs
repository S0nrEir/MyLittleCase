// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-20-2016
// ***********************************************************************
// <copyright file="CharactersData.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// Class that describes characters datas
/// Dont derive from MonoBehavior
/// </summary>
[System.Serializable]
public class CharactersData:ICloneable  
{ 	public  string PicturesName = string.Empty;
    /// <summary>
    /// The name
    /// </summary>
    public string Name = string.Empty;
    /// <summary>
    /// The is the main character
    /// </summary>
    public bool IsTheMainCharacter = false ;
    /// <summary>
    /// The type
    /// </summary>
    public EnumCharacterType Type = default(EnumCharacterType);

    //Character abilities
    /// <summary>
    /// The hp
    /// </summary>
    public int HP= default(int);
    /// <summary>
    /// The mp
    /// </summary>
    public int MP= default(int);

    /// <summary>
    /// The attack
    /// </summary>
    public int Attack= default(int);
    /// <summary>
    /// The defense
    /// </summary>
    public int Defense= default(int);
    /// <summary>
    /// The magic
    /// </summary>
    public int Magic= default(int);
    /// <summary>
    /// The magic defense
    /// </summary>
    public int MagicDefense= default(int);
    /// <summary>
    /// The level
    /// </summary>
    public int Level= default(int);
    /// <summary>
    /// The xp
    /// </summary>
    public int XP= default(int);

    //The maximum amount of mana that the charater can reach
    /// <summary>
    /// The maximum hp
    /// </summary>
    public int MaxHP= default(int);
    /// <summary>
    /// The maximum mp
    /// </summary>
    public int MaxMP= default(int);



    //Equipement
    /// <summary>
    /// The left hand
    /// </summary>
    public ItemsData LeftHand =  default(ItemsData);
    /// <summary>
    /// The right hand
    /// </summary>
    public ItemsData RightHand =  default(ItemsData);
    /// <summary>
    /// The head
    /// </summary>
    public ItemsData Head =  default(ItemsData);
    /// <summary>
    /// The body
    /// </summary>
    public ItemsData Body =  default(ItemsData);

    //Spells
    /// <summary>
    /// The spells list
    /// </summary>
    public List<SpellsData>  SpellsList  =new List<SpellsData> ();


    /// <summary>
    /// Gets the attack.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public int GetAttack()
	{
		var value = this.Attack + 
			(LeftHand == default(ItemsData) ? 0 : LeftHand.Attack) +
			(RightHand == default(ItemsData) ? 0 : RightHand.Attack) +
			(Head == default(ItemsData) ? 0 : Head.Attack) +
			(Body == default(ItemsData) ? 0 : Body.Attack);
		return value;
	}

    /// <summary>
    /// Gets the defense.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public int GetDefense()
	{
		var value = this.Defense  + 
			(LeftHand == default(ItemsData) ? 0 : LeftHand.Defense) +
				(RightHand == default(ItemsData) ? 0 : RightHand.Defense) +
				(Head == default(ItemsData) ? 0 : Head.Defense) +
				(Body == default(ItemsData) ? 0 : Body.Defense);
		return value;
	}

    /// <summary>
    /// Gets the magic.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public int GetMagic()
	{
		var value = this.Magic  + 
			(LeftHand == default(ItemsData) ? 0 : LeftHand.Magic) +
				(RightHand == default(ItemsData) ? 0 : RightHand.Magic) +
				(Head == default(ItemsData) ? 0 : Head.Magic) +
				(Body == default(ItemsData) ? 0 : Body.Magic);
		return value;
	}

    /// <summary>
    /// Gets the magic defense.
    /// </summary>
    /// <returns>System.Int32.</returns>
    public int GetMagicDefense()
	{
		var value = this.MagicDefense  + 
			(LeftHand == default(ItemsData) ? 0 : LeftHand.MagicDefense) +
				(RightHand == default(ItemsData) ? 0 : RightHand.MagicDefense) +
				(Head == default(ItemsData) ? 0 : Head.MagicDefense) +
				(Body == default(ItemsData) ? 0 : Body.MagicDefense);
		return value;
	}

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}