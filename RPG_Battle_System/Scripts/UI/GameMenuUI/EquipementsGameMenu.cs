// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="EquipementsGameMenu.cs" company="">

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
/// Class EquipementsGameMenu.
/// </summary>
public class EquipementsGameMenu : MonoBehaviour {
    //TODO : Add the number of items in the list  Ex :  sword x2 

    /// <summary>
    /// The toggle to duplicate
    /// </summary>
    public GameObject ToggleToDuplicate;
    /// <summary>
    /// The content panel
    /// </summary>
    public Transform ContentPanel;
    /// <summary>
    /// The equip action toggle
    /// </summary>
    public Toggle EquipActionToggle;
    /// <summary>
    /// The equipment description
    /// </summary>
    public Text EquipmentDescription;
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

		foreach (var item in Main.EquipmentList) {
			GameObject newToggle = Instantiate (ToggleToDuplicate) as GameObject;
			ItemsUI toggle = newToggle.GetComponent <ItemsUI> ();
			toggle.Name.text = item.Name;
			toggle.ItemData = item;
			toggle.Icon.sprite =Resources.Load <Sprite> (Settings.IconsPaths + item.PicturesName); ;
			toggle.Toggle.isOn = false;
			newToggle.SetActive(true);
			newToggle.transform.SetParent( ContentPanel);
			newToggle.transform.localScale= Vector3.one;
			newToggle.transform.position= Vector3.one;
			if(GameMenu.SelectedCharacter.Type != item.AllowedCharacterType 
			   || item.IsEquiped)
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
		Contract.Requires<MissingComponentException> (EquipActionToggle != null);
        SoundManager.UISound();
        if (toggle.isOn) {
			EquipActionToggle.Select ();
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.cyan;
			cb.highlightedColor = Color.cyan;
			toggle.colors = cb;
			selectedToggle = toggle;
			ItemsUI toggleItem = selectedToggle.GetComponent <ItemsUI> ();
			var itemDatas=Main.EquipmentList.Where(w =>w.Name == toggleItem.Name.text).FirstOrDefault();
			EquipmentDescription.text =itemDatas.EquipementType.ToString() + " : " + itemDatas.Description;
			SendMessage("CompareEquipementsAbilities",itemDatas);

		}
		else if (!toggle.isOn) {
			//equipActionToggle.Select ();
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.white;
			cb.highlightedColor = Color.yellow;
			toggle.colors = cb;
			SendMessage("CompareEquipementsAbilities",new ItemsData());

		}
	}


    /// <summary>
    /// This procedure equip the selected item
    /// <param name="toggle">The gameobject that sent the action</param>
    /// </summary>
    /// <param name="toggle">The toggle.</param>
    public void ToggleEquipAction(Toggle toggle)
	{
		Contract.Requires<MissingComponentException> (toggle != null);
		Contract.Requires<UnassignedReferenceException> (selectedToggle != null);
		Contract.Requires<UnassignedReferenceException> (GameMenu.SelectedCharacter != null);
        SoundManager.UISound();
        if (toggle.isOn) {
			ItemsUI toggleItem = selectedToggle.GetComponent <ItemsUI> ();

			var itemDatas = Main.EquipmentList.Where(w =>w.Name == toggleItem.Name.text).FirstOrDefault();

			switch (itemDatas.EquipementType)
			{
			case EnumEquipmentType.Head : 
				if (GameMenu.SelectedCharacter.Head != default(ItemsData))
                    {
                        Main.EquipmentList.Where(w => w.Name == GameMenu.SelectedCharacter.Head.Name).FirstOrDefault().IsEquiped = false;
                    }

                    GameMenu.SelectedCharacter.Head = itemDatas;
				itemDatas.IsEquiped = true;
				break;
			case EnumEquipmentType.Body : 
				if (GameMenu.SelectedCharacter.Body != default(ItemsData))
                    {
                        GameMenu.SelectedCharacter.Body.IsEquiped = false;
                    }

                    GameMenu.SelectedCharacter.Body = itemDatas;
				itemDatas.IsEquiped = true;
				break;
			case EnumEquipmentType.LeftHand :
				if(GameMenu.SelectedCharacter.RightHand != default(ItemsData)
				   && GameMenu.SelectedCharacter.RightHand.EquipementType == EnumEquipmentType.TwoHands) 
				{
					Main.EquipmentList.Where(w =>w.Name == GameMenu.SelectedCharacter.RightHand.Name).FirstOrDefault().IsEquiped = false;
					GameMenu.SelectedCharacter.RightHand = default(ItemsData);
				}

				if (GameMenu.SelectedCharacter.LeftHand != default(ItemsData))
                    {
                        Main.EquipmentList.Where(w => w.Name == GameMenu.SelectedCharacter.LeftHand.Name).FirstOrDefault().IsEquiped = false;
                    }

                    GameMenu.SelectedCharacter.LeftHand = itemDatas;
				itemDatas.IsEquiped = true;

				break;
			case EnumEquipmentType.RightHand : 
				if (GameMenu.SelectedCharacter.RightHand != default(ItemsData))
                    {
                        Main.EquipmentList.Where(w => w.Name == GameMenu.SelectedCharacter.RightHand.Name).FirstOrDefault().IsEquiped = false;
                    }

                    GameMenu.SelectedCharacter.RightHand = itemDatas;
				itemDatas.IsEquiped = true;
				break;
			case EnumEquipmentType.TwoHands : 
				if(GameMenu.SelectedCharacter.RightHand != default(ItemsData) ) 
				{
					Main.EquipmentList.Where(w =>w.Name == GameMenu.SelectedCharacter.RightHand.Name).FirstOrDefault().IsEquiped = false;
						
				}
				if(GameMenu.SelectedCharacter.LeftHand != default(ItemsData) ) 
				{
					Main.EquipmentList.Where(w =>w.Name == GameMenu.SelectedCharacter.LeftHand.Name).FirstOrDefault().IsEquiped = false;
					GameMenu.SelectedCharacter.LeftHand = default(ItemsData);
				
				}
				GameMenu.SelectedCharacter.RightHand = itemDatas;
				itemDatas.IsEquiped = true;

				break;
                default:
                    // do the default action
                    break;
            }

            SendMessage("LoadEquipements");
			SendMessage("LoadEquipementsAbilities");
			ClearItemList();
			PopulateList ();
		}
	}



    /// <summary>
    /// This procedure remove the selected item
    /// <param name="toggle">The gameobject that sent the action</param>
    /// </summary>
    /// <param name="toggle">The toggle.</param>
    public void ToggleRemoveAction(Toggle toggle)
	{
		Contract.Requires<MissingComponentException> (toggle != null);
		Contract.Requires<UnassignedReferenceException> (selectedToggle != null);
        SoundManager.UISound();
        if (toggle.isOn) {
			ItemsUI toggleItem = selectedToggle.GetComponent <ItemsUI> ();
			
			Main.EquipmentList.Remove(Main.EquipmentList.Where(w =>w.Name == toggleItem.Name.text && w.IsEquiped ==false).FirstOrDefault());
			ClearItemList();
			PopulateList ();
		}
	}

    /// <summary>
    /// This procedure remove all the selected items
    /// <param name="toggle">The gameobject that sent the action</param>
    /// </summary>
    /// <param name="toggle">The toggle.</param>
    public void ToggleRemoveAllAction(Toggle toggle)
	{
		Contract.Requires<MissingComponentException> (toggle != null);
        SoundManager.UISound();
        if (toggle.isOn) {

			Main.EquipmentList.RemoveAll(r=>r.IsEquiped ==false);
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

