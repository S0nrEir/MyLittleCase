// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="ItemsGameMenu.cs" company="">

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
/// Class ItemsGameMenu.
/// </summary>
public class ItemsGameMenu : MonoBehaviour {
    //TODO : Add the number of items in the list  Ex :  potion x2 

    /// <summary>
    /// The toggle to duplicate
    /// </summary>
    public GameObject ToggleToDuplicate;
    /// <summary>
    /// The content panel
    /// </summary>
    public Transform ContentPanel;
    /// <summary>
    /// The use action toggle
    /// </summary>
    public Toggle UseActionToggle;
    /// <summary>
    /// The item description
    /// </summary>
    public Text ItemDescription;
    /// <summary>
    /// The selected toggle
    /// </summary>
    private Toggle selectedToggle;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start () {
		ClearItemList ();
		PopulateList ();
	}

    /// <summary>
    /// Populates the list.
    /// </summary>
    void PopulateList () {
	
		Contract.Requires<UnassignedReferenceException> (GameMenu.SelectedCharacter != null);

		foreach (var item in Main.ItemList) {
			GameObject newToggle = Instantiate (ToggleToDuplicate) as GameObject;
			ItemsUI toggle = newToggle.GetComponent <ItemsUI> ();
			toggle.Name.text = item.Name;
			toggle.Icon.sprite =Resources.Load <Sprite> (Settings.IconsPaths + item.PicturesName); ;
			toggle.Toggle.isOn = false;
			newToggle.SetActive(true);
			newToggle.transform.SetParent( ContentPanel);
			newToggle.transform.localScale= Vector3.one;
			newToggle.transform.position= Vector3.one;
			if(item.EquipementType != EnumEquipmentType.Usable) 
				newToggle.GetComponent<Toggle>().interactable = false ;
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
		Contract.Requires<MissingComponentException> (UseActionToggle != null);
        SoundManager.UISound();
        if (toggle.isOn) {
			UseActionToggle.Select ();
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.cyan;
			cb.highlightedColor = Color.cyan;
			toggle.colors = cb;
			selectedToggle = toggle;
			ItemsUI toggleItem = selectedToggle.GetComponent <ItemsUI> ();
			var itemDatas=Main.ItemList.Where(w =>w.Name == toggleItem.Name.text).FirstOrDefault();
			ItemDescription.text =itemDatas.Description;
		
		}
		else if (!toggle.isOn) {
			//equipActionToggle.Select ();
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.white;
			cb.highlightedColor = Color.yellow;
			toggle.colors = cb;
		
		}
	}


    /// <summary>
    /// This procedure use the selected item
    /// <param name="toggle">The toggle that sent the action</param>
    /// </summary>
    /// <param name="toggle">The toggle.</param>
    public void ToggleUseAction(Toggle toggle)
	{
		Contract.Requires<MissingComponentException> (toggle != null);
		Contract.Requires<UnassignedReferenceException> (selectedToggle != null);
		Contract.Requires<UnassignedReferenceException> (GameMenu.SelectedCharacter != null);
        SoundManager.UISound();
        if (toggle.isOn) {
			ItemsUI toggleItem = selectedToggle.GetComponent <ItemsUI> ();
			var x = Main.ItemList.Where (w => w.Name == toggleItem.Name.text).FirstOrDefault ();
			GameMenu.SelectedCharacter.HP = Mathf.Clamp (GameMenu.SelectedCharacter.HP + x.HealthPoint, GameMenu.SelectedCharacter.HP, GameMenu.SelectedCharacter.MaxHP);
			GameMenu.SelectedCharacter.MP = Mathf.Clamp(GameMenu.SelectedCharacter.MP + x.Mana, GameMenu.SelectedCharacter.MP, GameMenu.SelectedCharacter.MaxMP) ;
			Main.ItemList.Remove(Main.ItemList.Where(w =>w.Name == toggleItem.Name.text ).FirstOrDefault());
			SendMessage("LoadCharactersAbilities");
			ClearItemList();
			PopulateList ();
		}
	}


    /// <summary>
    /// This procedure clear all the items in the list
    /// </summary>
    public void ClearItemList()
	{
		Contract.Requires<UnassignedReferenceException> (ContentPanel != null);

		foreach (Transform child in ContentPanel.transform) {
			if(child.gameObject.activeInHierarchy)
            {
                GameObject.Destroy(child.gameObject);
            }

        }
    }

}

