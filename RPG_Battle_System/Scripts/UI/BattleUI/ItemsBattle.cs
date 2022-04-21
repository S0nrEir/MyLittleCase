// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="ItemsBattle.cs" company="">

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
/// Class ItemsBattle.
/// </summary>
public class ItemsBattle : MonoBehaviour {
    //TODO : Add the number of items in the list  Ex :  potion x2 

    /// <summary>
    /// The toggle to duplicate
    /// </summary>
    public GameObject ToggleToDuplicate;
    /// <summary>
    /// The content panel
    /// </summary>
    public Transform ContentPanel;
    //public Toggle useActionToggle;
    /// <summary>
    /// The item description
    /// </summary>
    public Text ItemDescription;
    /// <summary>
    /// The selected toggle
    /// </summary>
    private Toggle selectedToggle;
    /// <summary>
    /// The logic game object
    /// </summary>
    private GameObject logicGameObject ;

    /// <summary>
    /// Starts this instance.
    /// </summary>
    void Start () {
		ClearItemList ();
		PopulateList ();
		logicGameObject  = GameObject.FindGameObjectsWithTag(Settings.Logic).FirstOrDefault();

	}

    /// <summary>
    /// Populates the list.
    /// </summary>
    void PopulateList () {
	
		Contract.Requires<UnassignedReferenceException> (BattlePanels.SelectedCharacter != null);

		foreach (var item in Main.ItemList) {
			GameObject newToggle = Instantiate (ToggleToDuplicate) as GameObject;
			ItemsUI toggle = newToggle.GetComponent <ItemsUI> ();
			toggle.Name.text = item.Name;
			toggle.Icon.sprite =Resources.Load <Sprite> (Settings.IconsPaths + item.PicturesName); ;
			toggle.Toggle.isOn = false;
			newToggle.SetActive(true);
			newToggle.transform.SetParent( ContentPanel.transform);
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
        SoundManager.UISound();
        if (toggle.isOn) {
//			useActionToggle.Select ();
			//equipActionToggle.isOn = true;
			toggle.group.NotifyToggleOn(toggle);
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.cyan;
			cb.highlightedColor = Color.cyan;
			toggle.colors = cb;
			selectedToggle = toggle;
			ItemsUI toggleItem = selectedToggle.GetComponent <ItemsUI> ();
			var itemDatas=Main.ItemList.Where(w =>w.Name == toggleItem.Name.text).FirstOrDefault();
			//itemDescription.text =itemDatas.Description;
			BattlePanels.SelectedCharacter.HP += itemDatas.HealthPoint;
			BattlePanels.SelectedCharacter.MP += itemDatas.Mana;
			BattlePanels.SelectedItem = itemDatas;
			Main.ItemList.Remove(Main.ItemList.Where(w =>w.Name == toggleItem.Name.text).FirstOrDefault());
			if (logicGameObject) {
				logicGameObject.BroadcastMessage("ItemAction");	
				}
			}
		else if (!toggle.isOn) {
			//equipActionToggle.Select ();
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.white;
			cb.highlightedColor = Color.yellow;
			toggle.colors = cb;
			//PanelActionController.selectedItem = null;

			//itemDescription.text =string.Empty;
		
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

    /// <summary>
    /// Deselects the menus toggles.
    /// </summary>
    public void DeselectMenusToggles()
	{
		if(selectedToggle)
        {
            selectedToggle.isOn = false;
        }

    }

}

