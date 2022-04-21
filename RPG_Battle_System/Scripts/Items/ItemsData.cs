// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-06-2016
// ***********************************************************************
// <copyright file="ItemsData.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// Main class that describe items datas
/// </summary>
[System.Serializable]
public class ItemsData
{
    /// <summary>
    /// The pictures name
    /// </summary>
    public string PicturesName = string.Empty;
    /// <summary>
    /// The name
    /// </summary>
    public string Name = string.Empty;
    /// <summary>
    /// The description
    /// </summary>
    public string Description = string.Empty;

    /// <summary>
    /// The health point
    /// </summary>
    public int HealthPoint= default(int);
    /// <summary>
    /// The mana
    /// </summary>
    public int Mana= default(int);
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
    /// The price
    /// </summary>
    public int Price= default(int);

    /// <summary>
    /// The allowed character type
    /// </summary>
    public EnumCharacterType AllowedCharacterType = EnumCharacterType.None;
    /// <summary>
    /// The equipement type
    /// </summary>
    public EnumEquipmentType EquipementType = EnumEquipmentType.None;
    /// <summary>
    /// The is equiped
    /// </summary>
    public bool IsEquiped = false;
}
