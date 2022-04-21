// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-07-2016
// ***********************************************************************
// <copyright file="SpellsBattle.cs" company="">

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
/// Class SpellsBattle.
/// </summary>
public class SpellsBattle : MonoBehaviour {

    /// <summary>
    /// The toggle to duplicate
    /// </summary>
    public GameObject ToggleToDuplicate;
    /// <summary>
    /// The content panel
    /// </summary>
    public Transform ContentPanel;
    /// <summary>
    /// The spell description
    /// </summary>
    public Text SpellDescription;
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

		foreach (var spell in BattlePanels.SelectedCharacter.SpellsList) {
			GameObject newToggle = Instantiate (ToggleToDuplicate) as GameObject;
			ItemsUI toggle = newToggle.GetComponent <ItemsUI> ();
			toggle.Name.text = spell.Name;
			toggle.Icon.sprite =Resources.Load <Sprite> (Settings.IconsPaths + spell.PicturesName); ;
			toggle.Toggle.isOn = false;
			newToggle.SetActive(true);
			newToggle.transform.SetParent(ContentPanel.transform);
			newToggle.transform.localScale= Vector3.one;
			newToggle.transform.position= Vector3.one;
			if (BattlePanels.SelectedCharacter.MP < spell.ManaAmount)
				toggle.Toggle.interactable = false;
			else 
				toggle.Toggle.interactable = true;
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
			//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.cyan;
			cb.highlightedColor = Color.cyan;
			toggle.colors = cb;
			selectedToggle = toggle;
			ItemsUI toggleItem = selectedToggle.GetComponent <ItemsUI> ();
			var itemDatas=BattlePanels.SelectedCharacter.SpellsList.Where(w =>w.Name == toggleItem.Name.text).FirstOrDefault();
			BattlePanels.SelectedSpell = itemDatas;
			//spellDescription.text = itemDatas.Description;
			if (logicGameObject) {
				logicGameObject.BroadcastMessage("MagicAction");	
			}
		}
		else if (!toggle.isOn) {
				//equipActionToggle.isOn = true;
			ColorBlock cb = toggle.colors;
			cb.normalColor = Color.white;
			cb.highlightedColor = Color.yellow;
			toggle.colors = cb;
			//PanelActionController.selectedSpell = null;
			//spellDescription.text =string.Empty;

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

