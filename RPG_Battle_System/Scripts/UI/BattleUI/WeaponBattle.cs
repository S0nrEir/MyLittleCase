// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="WeaponBattle.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Class WeaponBattle.
/// </summary>
public class WeaponBattle : MonoBehaviour {
    //TODO : Add the number of items in the list  Ex :  potion x2 
    //Equipments texts
    /// <summary>
    /// The character number
    /// </summary>
    public int CharacterNumber = 1;
    /// <summary>
    /// The right hand text
    /// </summary>
    public Text RightHandText ;
    /// <summary>
    /// The right hand image
    /// </summary>
    public Image RightHandImage ;
    /// <summary>
    /// The selected character infos
    /// </summary>
    public bool SelectedCharacterInfos  = false  ;
    /// <summary>
    /// The weapon description
    /// </summary>
    public Text WeaponDescription;
    /// <summary>
    /// The weapon
    /// </summary>
    public Toggle Weapon;
    /// <summary>
    /// The logic game object
    /// </summary>
    private GameObject logicGameObject ;


    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start () {
		Weapon.isOn = false;
		if (BattlePanels.SelectedCharacter  != null)
			LoadEquipements();

		logicGameObject  = GameObject.FindGameObjectsWithTag(Settings.Logic).FirstOrDefault();

		}

    /// <summary>
    /// This procedure display theequiped items
    /// </summary>
    public void LoadEquipements()
	{
		if (BattlePanels.SelectedCharacter.RightHand != null) {
			if (RightHandImage != null)
				RightHandImage.sprite = Resources.Load <Sprite> (Settings.IconsPaths + BattlePanels.SelectedCharacter.RightHand.PicturesName);
			if (RightHandText != null)
            {
                RightHandText.text = BattlePanels.SelectedCharacter.RightHand.Name;
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

    }

    /// <summary>
    /// This procedure check the resume toggle control and displays equips canvas
    /// <param name="gameObject">The gameobject that sent the action</param>
    /// </summary>
    /// <param name="toggle">The toggle.</param>
    public void ToggleSelectAction(Toggle toggle)
	{
		Contract.Requires<MissingComponentException> (toggle != null);
        SoundManager.UISound();
        if (toggle.isOn) {
			
//			useActionToggle.Select ();
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.cyan;
			cb.highlightedColor = Color.cyan;
			toggle.colors = cb;
			if (BattlePanels.SelectedCharacter.RightHand != null) {
				BattlePanels.SelectedWeapon = BattlePanels.SelectedCharacter.RightHand;
				//weaponDescription.text =PanelActionController.selectedCharacter.RightHand.Description;
				if (logicGameObject) {
					logicGameObject.BroadcastMessage("WeaponAction");	
				}
			}
		}
		else if (!toggle.isOn) {
			//equipActionToggle.Select ();
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.white;
			cb.highlightedColor = Color.yellow;
			toggle.colors = cb;
			//PanelActionController.selectedWeapon = null;
			//weaponDescription.text =string.Empty;

		}
	}

    /// <summary>
    /// Deselects the menus toggles.
    /// </summary>
    public void DeselectMenusToggles()
	{
		if(Weapon)
        {
            Weapon.isOn = false;
        }

    }





}

