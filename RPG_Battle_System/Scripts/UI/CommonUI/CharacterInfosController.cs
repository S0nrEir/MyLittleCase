// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-20-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-11-2016
// ***********************************************************************
// <copyright file="CharacterInfosController.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class CharacterInfosController.
/// </summary>
public class CharacterInfosController : MonoBehaviour {

    /// <summary>
    /// The character number
    /// </summary>
    public int CharacterNumber = 1;

    /// <summary>
    /// The characters datas
    /// </summary>
    public CharactersData CharactersDatas;
    /// <summary>
    /// The selected character infos
    /// </summary>
    public bool SelectedCharacterInfos  = false  ;
    /// <summary>
    /// The characters image
    /// </summary>
    public Image  CharactersImage ;

    //UI public text boxes
    /// <summary>
    /// The characters name
    /// </summary>
    public Text CharactersName ;
    /// <summary>
    /// The characters class
    /// </summary>
    public Text CharactersClass;
    /// <summary>
    /// The characters level
    /// </summary>
    public Text CharactersLevel ;

    //The  ammount of HP/MP that the character can have it's inferior of the max HP/MP if the character is wounded
    /// <summary>
    /// The characters hp
    /// </summary>
    public Text CharactersHP ;
    /// <summary>
    /// The characters mp
    /// </summary>
    public Text CharactersMP ;

    //The maximum ammount of HP/MP that the character can have 
    /// <summary>
    /// The characters hp maximum
    /// </summary>
    public Text CharactersHPMax ;
    /// <summary>
    /// The characters mp maximum
    /// </summary>
    public Text CharactersMPMax ;

    //Equipments texts
    /// <summary>
    /// The right hand text
    /// </summary>
    public Text RightHandText ;
    /// <summary>
    /// The left hand text
    /// </summary>
    public Text LeftHandText;
    /// <summary>
    /// The head text
    /// </summary>
    public Text HeadText ;
    /// <summary>
    /// The body text
    /// </summary>
    public Text BodyText ;

    //Equipments images
    /// <summary>
    /// The right hand image
    /// </summary>
    public Image RightHandImage ;
    /// <summary>
    /// The left hand image
    /// </summary>
    public Image LeftHandImage;
    /// <summary>
    /// The head image
    /// </summary>
    public Image HeadImage ;
    /// <summary>
    /// The body image
    /// </summary>
    public Image BodyImage;

    //Abilities texts
    /// <summary>
    /// The attack text
    /// </summary>
    public Text AttackText ;
    /// <summary>
    /// The magic text
    /// </summary>
    public Text MagicText;
    /// <summary>
    /// The defense text
    /// </summary>
    public Text DefenseText ;
    /// <summary>
    /// The magic defense text
    /// </summary>
    public Text MagicDefenseText ;


    //Abilities texts of the new item to compare with the old one 
    /// <summary>
    /// The attack text compare
    /// </summary>
    public Text AttackTextCompare;
    /// <summary>
    /// The magic text compare
    /// </summary>
    public Text MagicTextCompare;
    /// <summary>
    /// The defense text compare
    /// </summary>
    public Text DefenseTextCompare;
    /// <summary>
    /// The magic defense text compare
    /// </summary>
    public Text MagicDefenseTextCompare;


    // Use this for initialization
    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start () {
		if (GameMenu.SelectedCharacter != default(CharactersData) && SelectedCharacterInfos)
			CharacterNumber = Main.CharacterList.IndexOf (GameMenu.SelectedCharacter)+1;
		if (Main.CharacterList.Count >= CharacterNumber) {

			//Getting the charactersDatas from the list and setting it to the ui objects
			CharactersDatas = Main.CharacterList [CharacterNumber - 1];
			Contract.Requires<MissingReferenceException>(CharactersDatas!=null);
			LoadCharactersAbilities();
			LoadEquipements();
			LoadEquipementsAbilities();
			
			
		} else {
			//If the list doesnt contains enough characters the engine hide this character infos
			if(this.gameObject)
                this.gameObject.SetActive(false);
		}
	}

    /// <summary>
    /// This procedure display the character abilities points
    /// </summary>
    public void LoadCharactersAbilities()
	{

		
		
		if(CharactersImage != null)
        {
            CharactersImage.sprite = Resources.Load<Sprite>(Settings.SpritesPaths + CharactersDatas.PicturesName);
        }



        if (CharactersName != null)
        {
            CharactersName.text = CharactersDatas.Name;
        }

        if (CharactersClass != null)
        {
            CharactersClass.text = CharactersDatas.Type.ToString();
        }

        if (CharactersLevel != null)
        {
            CharactersLevel.text = CharactersDatas.Level.ToString();
        }

        if (CharactersHP != null)
        {
            CharactersHP.text = CharactersDatas.HP.ToString();
        }

        if (CharactersMP != null)
        {
            CharactersMP.text = CharactersDatas.MP.ToString();
        }


        if (CharactersHPMax != null)
        {
            CharactersHPMax.text = CharactersDatas.MaxHP.ToString();
        }

        if (CharactersMPMax != null)
        {
            CharactersMPMax.text = CharactersDatas.MaxMP.ToString();
        }

    }

    /// <summary>
    /// This procedure display the equipment abilities points
    /// </summary>
    public void LoadEquipementsAbilities()
	{
		if (AttackText != null) {
			AttackText.text = CharactersDatas.GetAttack().ToString();
		}
		if (MagicText != null)
		{
			MagicText.text = CharactersDatas.GetMagic().ToString();
	}
	if(DefenseText != null)
		{
			DefenseText.text = CharactersDatas.GetDefense().ToString();}
if(MagicDefenseText != null)
		{
			MagicDefenseText.text = CharactersDatas.GetMagicDefense().ToString();}

	}

    /// <summary>
    /// This procedure display the equiped items
    /// </summary>
    public void LoadEquipements()
	{
		if (CharactersDatas.RightHand != null) {
			if (RightHandImage != null)
				RightHandImage.sprite = Resources.Load <Sprite> (Settings.IconsPaths + CharactersDatas.RightHand.PicturesName);
			if (RightHandText != null)
            {
                RightHandText.text = CharactersDatas.RightHand.Name;
            }

        }
        else {
			if (RightHandImage != null)
            {
                RightHandImage.sprite = null;
            }

            if (RightHandText != null)
            {
                RightHandText.text = string.Empty;
            }

        }
        if (CharactersDatas.LeftHand != null)
		{
			if(LeftHandImage != null)
            {
                LeftHandImage.sprite = Resources.Load<Sprite>(Settings.IconsPaths + CharactersDatas.LeftHand.PicturesName);
            }

            if (LeftHandText  != null)
            {
                LeftHandText.text = CharactersDatas.LeftHand.Name;
            }

        }
        else {
			if(LeftHandImage != null)
            {
                LeftHandImage.sprite = null;
            }

            if (LeftHandText  != null)
            {
                LeftHandText.text = string.Empty;
            }

        }

        if (CharactersDatas.Head != null)
		{
			if(HeadImage != null)
            {
                HeadImage.sprite = Resources.Load<Sprite>(Settings.IconsPaths + CharactersDatas.Head.PicturesName);
            }

            if (HeadText  != null)
            {
                HeadText.text = CharactersDatas.Head.Name;
            }

        }
        else {
			if(HeadImage != null)
            {
                HeadImage.sprite = null;
            }

            if (HeadText  != null)
            {
                HeadText.text = string.Empty;
            }

        }

        if (CharactersDatas.Body != null)
		{
			if(BodyImage != null)
            {
                BodyImage.sprite = Resources.Load<Sprite>(Settings.IconsPaths + CharactersDatas.Body.PicturesName);
            }

            if (BodyText != null)
            {
                BodyText.text = CharactersDatas.Body.Name;
            }

        }
        else {
			if(BodyImage != null)
            {
                BodyImage.sprite = null;
            }

            if (BodyText != null)
            {
                BodyText.text = string.Empty;
            }

        }

    }


    /// <summary>
    /// This procedure display the equiped items
    /// <param name="gameObject">The gameobject that sent the action</param>
    /// </summary>
    /// <param name="itemsData">The items data.</param>
    public void CompareEquipementsAbilities(ItemsData itemsData)
	{
        var clonedSelectedCharacter =( CharactersData)( GameMenu.SelectedCharacter.Clone());

        switch (itemsData.EquipementType)
        {
            case EnumEquipmentType.Head: clonedSelectedCharacter.Head = itemsData; break;
            case EnumEquipmentType.Body : clonedSelectedCharacter.Body = itemsData; break;
            case EnumEquipmentType.LeftHand: clonedSelectedCharacter.LeftHand = itemsData; break;
            case EnumEquipmentType.RightHand: clonedSelectedCharacter.RightHand = itemsData; break;
            case EnumEquipmentType.TwoHands: clonedSelectedCharacter.RightHand = itemsData; clonedSelectedCharacter.LeftHand = null; break;
        }
       

        if (AttackTextCompare != null)
        {
            AttackTextCompare.text = clonedSelectedCharacter.GetAttack().ToString();
        }

        if (MagicTextCompare != null)
        {
            MagicTextCompare.text = clonedSelectedCharacter.GetMagic().ToString();
        }

        if (DefenseTextCompare != null)
        {
            DefenseTextCompare.text = clonedSelectedCharacter.GetDefense().ToString();
        }

        if (MagicDefenseTextCompare != null)
        {
            MagicDefenseTextCompare.text = clonedSelectedCharacter.GetMagicDefense().ToString();
        }


    }





    /// <summary>
    /// Toggles the select character.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    public void ToggleSelectCharacter(GameObject gameObject)
	{
		Contract.Requires<UnassignedReferenceException> (gameObject != null);

		var toggle = gameObject.GetComponent<Toggle>();
		
		if (toggle.isOn)
			GameMenu.SelectedCharacter = CharactersDatas;
	}


}
