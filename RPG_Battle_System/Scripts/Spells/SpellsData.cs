// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 03-25-2016
// ***********************************************************************
// <copyright file="SpellsData.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
/// <summary>
/// Main class that describe items datas
/// </summary>
[System.Serializable]
public class SpellsData
{ 	public  string PicturesName = string.Empty;
    /// <summary>
    /// The name
    /// </summary>
    public string Name = string.Empty;
    /// <summary>
    /// The description
    /// </summary>
    public string Description = string.Empty;
    /// <summary>
    /// The attack
    /// </summary>
    public int Attack= default(int);
    /// <summary>
    /// The mana amount
    /// </summary>
    public int ManaAmount= default(int);
	/// <summary>
	/// The particle effect to display when launched
	/// </summary>
	public string ParticleEffect= string.Empty;
    /// <summary>
	/// The sound effect
	/// </summary>
	public string SoundEffect = string.Empty;
    /// <summary>
    /// The allowed character type
    /// </summary>
    public EnumCharacterType AllowedCharacterType = EnumCharacterType.None;

}
