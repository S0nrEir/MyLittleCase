// ***********************************************************************
// Assembly         : Assembly-CSharp
// Author           : Pondomaniac
// Created          : 07-06-2016
//
// Last Modified By : Pondomaniac
// Last Modified On : 07-20-2016
// ***********************************************************************
// <copyright file="BattleController.cs" company="">

// </copyright>
// <summary></summary>
// ***********************************************************************
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Holoville.HOTween;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;




/// <summary>
/// Class BattleController.
/// </summary>
public class BattleController : MonoBehaviour
{
    /// <summary>
    /// Gets or sets the state of the character.
    /// </summary>
    /// <value>The state of the character.</value>
    EnumCharacterState CharacterState { get; set; }
    /// <summary>
    /// Gets or sets the character side.
    /// </summary>
    /// <value>The character side.</value>
    EnumSide CharacterSide { get; set; }
    /// <summary>
    /// The possible ennemies
    /// </summary>
    public List<GameObject> PossibleEnnemies = new List<GameObject>();
    /// <summary>
    /// The number of enemy to generate
    /// </summary>
    public int NumberOfEnemyToGenerate = 1;
    /// <summary>
    /// The space between characters
    /// </summary>
    public int SpaceBetweenCharacters = 5;
    /// <summary>
    /// The space between character and enemy
    /// </summary>
    public int SpaceBetweenCharacterAndEnemy = 3;
    /// <summary>
    /// The players x position
    /// </summary>
    public int PlayersXPosition = -17;
    /// <summary>
    /// The ennemy x position
    /// </summary>
    public int EnnemyXPosition = 17;
    /// <summary>
    /// The players y position
    /// </summary>
    public int PlayersYPosition = 16;
    /// <summary>
    /// The ennemy y position
    /// </summary>
    public int EnnemyYPosition = 16;
    /// <summary>
    /// The selector
    /// </summary>
    public GameObject Selector;
    /// <summary>
    /// The target selector
    /// </summary>
    public GameObject TargetSelector;
    /// <summary>
    /// The weapon particle effect
    /// </summary>
    public GameObject WeaponParticleEffect;
    /// <summary>
    /// The magic particle effect
    /// </summary>
    public GameObject MagicParticleEffect;
    /// <summary>
    /// The current state
    /// </summary>
    private EnumBattleState currentState = EnumBattleState.Beginning;
    /// <summary>
    /// The battl action
    /// </summary>
    private EnumBattleAction battlAction = EnumBattleAction.None;
    /// <summary>
    /// The generated enemy list
    /// </summary>
    private List<GameObject> generatedEnemyList = new List<GameObject>();
    /// <summary>
    /// The instantiated character list
    /// </summary>
    private List<GameObject> instantiatedCharacterList = new List<GameObject>();
    /// <summary>
    /// The turn by turn sequence list
    /// </summary>
    private List<Tuple<EnumPlayerOrEnemy, GameObject>> turnByTurnSequenceList = new List<Tuple<EnumPlayerOrEnemy, GameObject>>();
    /// <summary>
    /// The sequence enumerator
    /// </summary>
    List<Tuple<EnumPlayerOrEnemy, GameObject>>.Enumerator sequenceEnumerator;

    /// <summary>
    /// The instantiated selector
    /// </summary>
    private GameObject instantiatedSelector;
    /// <summary>
    /// The instantiated target selector
    /// </summary>
    private GameObject instantiatedTargetSelector;
    /// <summary>
    /// The selected enemy
    /// </summary>
    private GameObject selectedEnemy;
    /// <summary>
    /// The selected player
    /// </summary>
    private GameObject selectedPlayer;
    /// <summary>
    /// The selected player datas
    /// </summary>
    private CharactersData selectedPlayerDatas;
    /// <summary>
    /// The UI game object
    /// </summary>
    private GameObject uiGameObject ;
    /// <summary>
    /// Awakes this instance.main
    /// </summary>
    void Awake()
	{
		
		  
        CharacterState = EnumCharacterState.Idle;
		CharacterSide = EnumSide.Down;
		instantiatedSelector = GameObject.Instantiate(Selector);
		instantiatedTargetSelector = GameObject.Instantiate(TargetSelector);
		var t =Resources.Load <GameObject> (Settings.PrefabsPath + Settings.UIBattle);
		var o = Instantiate (t);
		var canvas = o.GetComponentsInChildren<Canvas>();
		foreach (Canvas canva in canvas) {
			canva.worldCamera= Camera.main;
		}
		GenerateEnnemies();
		PositionPlayers();
		GenerateTurnByTurnSequence();
		sequenceEnumerator = turnByTurnSequenceList.GetEnumerator();
		NextBattleSequence();
		uiGameObject  = GameObject.FindGameObjectsWithTag(Settings.UI).FirstOrDefault();
		HideDecision();
	


	}




    /// <summary>
    /// This is the main loop and where the system detect the presed keys and send them to the controller.
    /// </summary>
    void Update()
	{ 

		if (HOTween.GetAllPlayingTweens().Any())
		return;
	else if (currentState == EnumBattleState.SelectingTarget) {
		//Detecting if the player clicked on the left mouse button and also if there is no animation playing
		if (Input.GetButtonDown ("Fire1")) {
			//The 3 following lines is to get the clicked GameObject and getting the RaycastHit2D that will help us know the clicked object
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			if (hit.transform != null) {

				//TODO: Add tweening condition
				bool foundEnemy = false;
				foreach (var x in generatedEnemyList) {
					if (x.GetInstanceID () == hit.transform.gameObject.GetInstanceID ()) {
						foundEnemy = true;
						selectedEnemy = hit.transform.gameObject;
						PositionTargetSelector (selectedEnemy);
						break;
					}
				}
				if (!foundEnemy)
					return;


			}

			
		}
	}
	else if (currentState == EnumBattleState.EnemyTurn)
	{Log (GameTexts.EnemyTurn);
		var z = turnByTurnSequenceList.Where(w=> w.First == EnumPlayerOrEnemy.Player);
		var playerTargetedByEnemy = z.ElementAt(UnityEngine.Random.Range(0, z.Count() - 1));
		var playerTargetedByEnemyDatas =  GetCharacterDatas (playerTargetedByEnemy.Second.name);

		PositionTargetSelector(playerTargetedByEnemy.Second);
		EnemyAttack(playerTargetedByEnemy.Second,playerTargetedByEnemyDatas);
		NextBattleSequence ();                                                             
	}
	else if (currentState == EnumBattleState.PlayerTurn)
	{Log (GameTexts.PlayerTurn);
		HideTargetSelector();
		ShowMenu();
			currentState = EnumBattleState.None;             
	}
	else if (currentState == EnumBattleState.PlayerWon)
	{Log (GameTexts.PlayerWon);
		HideTargetSelector();
		HideMenu ();
			int totalXP = 0;
			foreach (var x in generatedEnemyList) {
				totalXP += x.GetComponent<EnemyCharacterDatas> ().XP;
			}

			foreach (var x in turnByTurnSequenceList)
		{
				var characterdatas = GetCharacterDatas (x.Second.name);
				characterdatas.XP += totalXP;
				var calculatedXP = Math.Floor ( Math.Sqrt(625+100* characterdatas.XP)-25)/ 50;
				characterdatas.Level =(int) calculatedXP;
		}
		var textTodisplay = GameTexts.EndOfTheBattle + "\n\n" + GameTexts.PlayerXP + totalXP;
			ShowDropMenu(textTodisplay);
			currentState = EnumBattleState.EndBattle;
            var go = GameObject.FindGameObjectsWithTag(Settings.Music).FirstOrDefault();
            if (go) go.GetComponent<AudioSource>().Stop(); 
            SoundManager.WinningMusic();         
	}
	else if (currentState == EnumBattleState.EnemyWon)
	{   Log (GameTexts.EnemyWon);
		HideTargetSelector();
		HideMenu ();

		var textTodisplay = GameTexts.EndOfTheBattle + "\n\n" +GameTexts.YouLost ;
		ShowDropMenu(textTodisplay);

		currentState = EnumBattleState.None;
            var go = GameObject.FindGameObjectsWithTag(Settings.Music).FirstOrDefault();
            if (go) go.GetComponent<AudioSource>().Stop();
            SoundManager.GameOverMusic();
        }
}

    /// <summary>
    /// Gets the character datas.
    /// </summary>
    /// <param name="s">The s.</param>
    /// <returns>CharactersData.</returns>
    public CharactersData GetCharacterDatas(string s )
	{

		return Main.CharacterList.Where (w => w.Name == s.Replace(Settings.CloneName,"" ) ).FirstOrDefault ();

	}


    /// <summary>
    /// Changes the state of the enum character.
    /// </summary>
    /// <param name="state">The state.</param>
    public void ChangeEnumCharacterState(EnumCharacterState state)
	{
		CharacterState = state;
		SendMessage("Animate", string.Format("{0}{1}", CharacterState, CharacterSide));
	}

    /// <summary>
    /// Changes the enum character side.
    /// </summary>
    /// <param name="state">The state.</param>
    public void ChangeEnumCharacterSide(EnumSide state)
	{
		CharacterSide = state;
		SendMessage("Animate", string.Format("{0}{1}", CharacterState, CharacterSide));
	}

    /// <summary>
    /// Flips the specified to the left.
    /// </summary>
    /// <param name="toTheLeft">if set to <c>true</c> [to the left].</param>
    void Flip(bool toTheLeft)
	{
		Vector3 theScale = transform.localScale;
		if (toTheLeft)
			theScale.x = -Mathf.Abs(theScale.x);
		else
			theScale.x = Mathf.Abs(theScale.x);
		transform.localScale = theScale;
	}

    /// <summary>
    /// Generates the ennemies.
    /// </summary>
    void GenerateEnnemies()
	{
		int y = EnnemyYPosition;
		int calculatedPosition = y;
		for (int i = 0; i <= NumberOfEnemyToGenerate-1; i++)
		{
			y--;
			calculatedPosition = calculatedPosition - SpaceBetweenCharacters;
		    GameObject go = GameObject.Instantiate (PossibleEnnemies [UnityEngine.Random.Range ((int)0, (int)PossibleEnnemies.Count )],
				         new Vector3 (EnnemyXPosition, calculatedPosition, 1), Quaternion.identity) as GameObject;
			
			 
			generatedEnemyList.Add(go);

		}

	}

    /// <summary>
    /// Positions the players.
    /// </summary>
    void PositionPlayers()
	{
		try
		{


			int y = PlayersYPosition;
			int calculatedPosition = y;
			foreach (var character in Main.CharacterList)
			{
				y--;
				calculatedPosition = calculatedPosition - SpaceBetweenCharacters;

				GameObject go =  GameObject.Instantiate(Resources.Load(Settings.PrefabsPath + character.Name)
										, new Vector3(PlayersXPosition, calculatedPosition, 1), Quaternion.identity) as GameObject;
				var datas = GetCharacterDatas(go.name);

				instantiatedCharacterList.Add(go);

				go.BroadcastMessage ("SetHPValue",datas.MaxHP<=0?0 :  datas.HP*100/datas.MaxHP);
				go.BroadcastMessage ("SetMPValue",datas.MaxMP <= 0 ? 0 : datas.MP*100/datas.MaxMP);

			}



		}
		catch (System.Exception ex)
		{
			Debug.Log(ex.Message);
		}

	}

    /// <summary>
    /// Generates the turn by turn sequence.
    /// </summary>
    void GenerateTurnByTurnSequence()
	{
		var x = 0;
		var y = 0;
		var z = 0;
		var indexInRange = true;
		while (indexInRange)
		{
			if (instantiatedCharacterList.Count - 1 < y && generatedEnemyList.Count - 1 < z) { indexInRange = false;
                break;
            }

            if (x % 2 == 0 && instantiatedCharacterList.Count - 1 >= y) { turnByTurnSequenceList.Add(new Tuple<EnumPlayerOrEnemy, GameObject>(EnumPlayerOrEnemy.Player, instantiatedCharacterList[y]));
                y++;
            }
            else if (x % 2 != 0 && generatedEnemyList.Count - 1 >= z) { turnByTurnSequenceList.Add(new Tuple<EnumPlayerOrEnemy, GameObject>(EnumPlayerOrEnemy.Enemy, generatedEnemyList[z]));
                z++;
            }

            x++;
		}


	}

    /// <summary>
    /// Nexts the battle sequence.
    /// </summary>
    public void NextBattleSequence()
	{
		var x =turnByTurnSequenceList.Where (w => w.First == EnumPlayerOrEnemy.Enemy).Count ();
		var y = turnByTurnSequenceList.Where (w => w.First == EnumPlayerOrEnemy.Player).Count ();
		if(x<=0) {currentState=EnumBattleState.PlayerWon;
            return;
        }
        else if(y<=0){ currentState=EnumBattleState.EnemyWon;
            return;
        }


        if (sequenceEnumerator.MoveNext ()) {
			PositionSelector (sequenceEnumerator.Current.Second);
			if (sequenceEnumerator.Current.First == EnumPlayerOrEnemy.Player) {

				currentState = EnumBattleState.PlayerTurn;

				selectedPlayer = sequenceEnumerator.Current.Second;
				selectedPlayerDatas =GetCharacterDatas(selectedPlayer.name);
				BattlePanels.SelectedCharacter = selectedPlayerDatas;
			
			} else if (sequenceEnumerator.Current.First == EnumPlayerOrEnemy.Enemy) {
				currentState = EnumBattleState.EnemyTurn;

			}

		} else {

			sequenceEnumerator = turnByTurnSequenceList.GetEnumerator();
			NextBattleSequence();

		}
	}



    /// <summary>
    /// Positions the selector.
    /// </summary>
    /// <param name="go">The go.</param>
    public void PositionSelector(GameObject go)
	{
		instantiatedSelector.transform.position = go.transform.position + new Vector3(-4, 0, 0);
	}

    /// <summary>
    /// Weapons the action.
    /// </summary>
    public void WeaponAction()
	{
		currentState = EnumBattleState.SelectingTarget;
		battlAction = EnumBattleAction.Weapon;
		SelectTheFirstEnemy();
		HideMenu();
		ShowDecision();

	}


    /// <summary>
    /// Magics the action.
    /// </summary>
    public void MagicAction()
	{
		currentState = EnumBattleState.SelectingTarget;
		battlAction = EnumBattleAction.Magic;
		SelectTheFirstEnemy();
		HideMenu();
		ShowDecision();

	}


    /// <summary>
    /// Items the action.
    /// </summary>
    public void ItemAction()
	{
		currentState = EnumBattleState.SelectingTarget;
		battlAction = EnumBattleAction.Item;
		SelectTheFirstEnemy();
		PassAction ();

	}

    /// <summary>
    /// Passes the action.
    /// </summary>
    public void PassAction()
	{
		battlAction = EnumBattleAction.Pass;
		selectedPlayer.BroadcastMessage ("SetHPValue",selectedPlayerDatas.MaxHP <= 0 ? 0 : selectedPlayerDatas.HP*100/selectedPlayerDatas.MaxHP);
		selectedPlayer.BroadcastMessage ("SetMPValue",selectedPlayerDatas.MaxMP <= 0 ? 0 : selectedPlayerDatas.MP*100/selectedPlayerDatas.MaxMP);
		NextBattleSequence ();
		HideMenu();


	}

    /// <summary>
    /// Selects the first enemy.
    /// </summary>
    public void SelectTheFirstEnemy()
	{
		selectedEnemy = generatedEnemyList.Where(w=>w.activeSelf).FirstOrDefault();
		if (selectedEnemy != null)
			PositionTargetSelector(selectedEnemy);

	}
    /// <summary>
    /// Positions the target selector.
    /// </summary>
    /// <param name="target">The target.</param>
    public void PositionTargetSelector(GameObject target)
	{
		instantiatedTargetSelector.SetActive(true);
		instantiatedTargetSelector.transform.position = target.transform.position + new Vector3(-4, 0, 0);
	}

    /// <summary>
    /// Hides the target selector.
    /// </summary>
    public void HideTargetSelector()
	{
		instantiatedTargetSelector.SetActive(false);
	}

    /// <summary>
    /// Hides the menu.
    /// </summary>
    public void HideMenu()
	{
		if(battlAction!= EnumBattleAction.Pass && currentState != EnumBattleState.EnemyWon && currentState != EnumBattleState.PlayerWon)
			DeselectMenusToggles ();

		if (uiGameObject ) 
			uiGameObject.BroadcastMessage("HideActionMenu");	

	}

    /// <summary>
    /// Deselects the menus toggles.
    /// </summary>
    public void DeselectMenusToggles()
	{
		if ( uiGameObject) 
			uiGameObject.BroadcastMessage("DeselectMenusToggles");	
	}


    /// <summary>
    /// Shows the menu.
    /// </summary>
    public void ShowMenu()
	{	if (uiGameObject) {
			uiGameObject.BroadcastMessage("ShowActionMenu");	
			uiGameObject.BroadcastMessage ("Start");
		}
	

	}

    /// <summary>
    /// Hides the decision.
    /// </summary>
    public void HideDecision()
	{
		if ( uiGameObject ) 
			uiGameObject.BroadcastMessage ("HideDecision");
	}

    /// <summary>
    /// Shows the decision.
    /// </summary>
    public void ShowDecision()
	{
		if (uiGameObject ) 
			uiGameObject.BroadcastMessage ("ShowDecision");
	}

    /// <summary>
    /// Declines the decision.
    /// </summary>
    public void DeclineDecision()
	{
        SoundManager.UISound();
        currentState = EnumBattleState.PlayerTurn;
		ShowMenu();
		HideTargetSelector();
		HideDecision();
	}

    /// <summary>
    /// Accepts the decision.
    /// </summary>
    public void AcceptDecision()
	{
        SoundManager.UISound();
        currentState = EnumBattleState.SelectedTarget;
		HideTargetSelector();
		HideDecision();
		PlayerAction();
	}

    /// <summary>
    /// Ends the battle.
    /// </summary>
    public void EndBattle()
	{
		Main.ControlsBlocked = false;
       SceneManager.LoadScene (Settings.MainMenuScene);
	}

    /// <summary>
    /// Players the action.
    /// </summary>
    public void PlayerAction()
	{
	


		var enemyCharacterdatas = selectedEnemy.GetComponent<EnemyCharacterDatas> ();
		int calculatedDamage = 0;
		if (enemyCharacterdatas != null && selectedPlayerDatas != null) {
			switch (battlAction) {
			case EnumBattleAction.Weapon:
                    Sequence actions = new Sequence(new SequenceParms());
                    TweenParms parms = new TweenParms().Prop("position", selectedEnemy.transform.position - new Vector3(SpaceBetweenCharacterAndEnemy, 0, 0)).Ease(EaseType.EaseOutQuart);
                    TweenParms parmsResetPlayerPosition = new TweenParms().Prop("position", selectedPlayer.transform.position).Ease(EaseType.EaseOutQuart);
                    actions.Append(HOTween.To(selectedPlayer.transform, 0.5f, parms));
                    actions.Append(HOTween.To(selectedPlayer.transform, 0.5f, parmsResetPlayerPosition));
                    actions.Play();
                    calculatedDamage = BattlePanels.SelectedWeapon.Attack + selectedPlayerDatas.GetAttack () - enemyCharacterdatas.Defense; 
				calculatedDamage = Mathf.Clamp (calculatedDamage, 0, calculatedDamage);
				enemyCharacterdatas.HP =Mathf.Clamp ( enemyCharacterdatas.HP - calculatedDamage, 0 , enemyCharacterdatas.HP - calculatedDamage);
				ShowPopup ("-"+calculatedDamage.ToString (), selectedEnemy.transform.position);
				selectedEnemy.BroadcastMessage ("SetHPValue",enemyCharacterdatas.MaxHP<=0?0 :  enemyCharacterdatas.HP*100/enemyCharacterdatas.MaxHP);
				Destroy( Instantiate (WeaponParticleEffect, selectedEnemy.transform.localPosition, Quaternion.identity),1.5f);
                    SoundManager.WeaponSound();
					selectedPlayer.SendMessage("Animate",EnumBattleState.Attack.ToString());
					selectedEnemy.SendMessage("Animate",EnumBattleState.Hit.ToString());
					
					 break;
			case EnumBattleAction.Magic:
				calculatedDamage = BattlePanels.SelectedSpell.Attack + selectedPlayerDatas.GetMagic () - enemyCharacterdatas.MagicDefense; 
				calculatedDamage = Mathf.Clamp (calculatedDamage, 0, calculatedDamage);
				enemyCharacterdatas.HP =Mathf.Clamp ( enemyCharacterdatas.HP - calculatedDamage, 0 , enemyCharacterdatas.HP - calculatedDamage);
				selectedPlayerDatas.MP = Mathf.Clamp ( selectedPlayerDatas.MP - BattlePanels.SelectedSpell.ManaAmount, 0 ,selectedPlayerDatas.MP - BattlePanels.SelectedSpell.ManaAmount);
				ShowPopup (calculatedDamage.ToString (), selectedEnemy.transform.localPosition);
				ShowPopup ("-"+calculatedDamage.ToString (), selectedEnemy.transform.position);
				selectedEnemy.BroadcastMessage ("SetHPValue",enemyCharacterdatas.MaxHP<=0?0 :  enemyCharacterdatas.HP*100/enemyCharacterdatas.MaxHP);
				selectedPlayer.BroadcastMessage ("SetMPValue",selectedPlayerDatas.MaxMP <= 0 ? 0 : selectedPlayerDatas.MP*100/selectedPlayerDatas.MaxMP);

                    var ennemyEffect = Resources.Load<GameObject>(Settings.PrefabsPath + BattlePanels.SelectedSpell.ParticleEffect);
                    Destroy(Instantiate(ennemyEffect, selectedEnemy.transform.localPosition, Quaternion.identity), 0.5f);

                    var playerEffect = Resources.Load<GameObject>(Settings.PrefabsPath + Settings.MagicAuraEffect);
                    Destroy( Instantiate (playerEffect, selectedPlayer.transform.localPosition, Quaternion.identity),0.4f);
                    SoundManager.StaticPlayOneShot(BattlePanels.SelectedSpell.SoundEffect, Vector3.zero);
					selectedPlayer.SendMessage("Animate",EnumBattleState.Magic.ToString());
					selectedEnemy.SendMessage("Animate",EnumBattleState.Hit.ToString());
					
                    break;
			case EnumBattleAction.Item:
				calculatedDamage = BattlePanels.SelectedItem.Attack - enemyCharacterdatas.MagicDefense; 
				calculatedDamage = Mathf.Clamp (calculatedDamage, 0, calculatedDamage);
				enemyCharacterdatas.HP =Mathf.Clamp ( enemyCharacterdatas.HP - calculatedDamage, 0 , enemyCharacterdatas.HP - calculatedDamage);
				ShowPopup ("-"+calculatedDamage.ToString (), selectedEnemy.transform.position);
				selectedEnemy.BroadcastMessage ("SetHPValue",enemyCharacterdatas.HP*100/enemyCharacterdatas.MaxHP);
				Destroy( Instantiate (MagicParticleEffect, selectedEnemy.transform.localPosition, Quaternion.identity),1.7f);
                    SoundManager.ItemSound();
				selectedPlayer.SendMessage("Animate",EnumBattleState.Magic.ToString());
				selectedEnemy.SendMessage("Animate",EnumBattleState.Hit.ToString());
					
                    break;
			default:
				break;
			}
		}

		if (enemyCharacterdatas.HP <= 0)
			KillCharacter (selectedEnemy);
		//selectedPlayer.SendMessage ("ChangeEnumCharacterState", battlection);
		selectedEnemy = null;


		NextBattleSequence();
	}

    /// <summary>
    /// Enemies the attack.
    /// </summary>
    /// <param name="playerToAttack">The player to attack.</param>
    /// <param name="playerToAttackDatas">The player to attack datas.</param>
    public void EnemyAttack(GameObject playerToAttack,CharactersData playerToAttackDatas )
	{
		var go = sequenceEnumerator.Current.Second;
	
		Sequence actions = new Sequence(new SequenceParms());
		TweenParms parms = new TweenParms().Prop("position", playerToAttack.transform.position + new Vector3(SpaceBetweenCharacterAndEnemy, 0, 0)).Ease(EaseType.EaseOutQuart);
		TweenParms parmsResetPlayerPosition = new TweenParms().Prop("position", go.transform.position).Ease(EaseType.EaseOutQuart);
		actions.Append(HOTween.To(go.transform, 0.5f, parms));
		actions.Append(HOTween.To(go.transform, 0.5f, parmsResetPlayerPosition));

		actions.Play();

		var enemyCharacterdatas = go.GetComponent<EnemyCharacterDatas> ();
		int calculatedDamage = 0;
	if (enemyCharacterdatas != null && selectedPlayerDatas != null) {
			switch (battlAction) {
			case EnumBattleAction.Weapon:
				calculatedDamage = enemyCharacterdatas.Attack - playerToAttackDatas.Defense; 
				calculatedDamage = Mathf.Clamp (calculatedDamage, 0, calculatedDamage);
				playerToAttackDatas.HP = Mathf.Clamp (playerToAttackDatas.HP - calculatedDamage , 0 ,playerToAttackDatas.HP - calculatedDamage);
				ShowPopup ("-"+calculatedDamage.ToString (), playerToAttack.transform.position);
				playerToAttack.BroadcastMessage ("SetHPValue",playerToAttackDatas.MaxHP<=0 ?0 : playerToAttackDatas.HP*100/playerToAttackDatas.MaxHP);
				Destroy( Instantiate (WeaponParticleEffect, playerToAttack.transform.localPosition, Quaternion.identity),1.5f);
                    SoundManager.WeaponSound();
                   go.SendMessage("Animate",EnumBattleState.Attack.ToString());
				   playerToAttack.SendMessage("Animate",EnumBattleState.Hit.ToString());
				
				    break;
			
			default:
				calculatedDamage = enemyCharacterdatas.Attack - playerToAttackDatas.Defense; 
				calculatedDamage = Mathf.Clamp (calculatedDamage, 0, calculatedDamage);
				playerToAttackDatas.HP = Mathf.Clamp (playerToAttackDatas.HP - calculatedDamage , 0 ,playerToAttackDatas.HP - calculatedDamage);
				ShowPopup ("-"+calculatedDamage.ToString (), playerToAttack.transform.position);
				playerToAttack.BroadcastMessage ("SetHPValue",playerToAttackDatas.MaxHP<=0 ?0 : playerToAttackDatas.HP*100/playerToAttackDatas.MaxHP);
				Destroy( Instantiate (WeaponParticleEffect, playerToAttack.transform.localPosition, Quaternion.identity),1.5f);
                    SoundManager.WeaponSound();
                   go.SendMessage("Animate",EnumBattleState.Attack.ToString());
				   playerToAttack.SendMessage("Animate",EnumBattleState.Hit.ToString());
				
                    break;

			}
		}
		if (playerToAttackDatas.HP <= 0)
			KillCharacter (playerToAttack);
		//selectedPlayer.SendMessage ("ChangeEnumCharacterState", battlection);
		selectedEnemy = null;

	}

    /// <summary>
    /// Kills the character.
    /// </summary>
    /// <param name="go">The go.</param>
    public void KillCharacter(GameObject go)
	{
		float time = 0.75f;
		Sequence actions = new Sequence(new SequenceParms());
		TweenParms parms = new TweenParms().Prop("color", new Color(1.0f, 1.0f, 1.0f, 0.0f)).Ease(EaseType.EaseOutQuart);
		actions.Append(HOTween.To(go.GetComponent<SpriteRenderer>(), time, parms));
		actions.Play();
		go.SetActive (false);
		turnByTurnSequenceList.RemoveAll (r => r.Second.GetInstanceID () == go.GetInstanceID ());
		var id = sequenceEnumerator.Current.Second.GetInstanceID ();
		sequenceEnumerator = turnByTurnSequenceList.GetEnumerator();
		sequenceEnumerator.MoveNext();
		while (sequenceEnumerator.Current.Second.GetInstanceID () != id)
			sequenceEnumerator.MoveNext();
		

			

	}

    /// <summary>
    /// Logs the specified text.
    /// </summary>
    /// <param name="text">The text.</param>
    public void Log(string text)
	{
		if (uiGameObject) 
			uiGameObject.BroadcastMessage ("LogText",text);
		}

    /// <summary>
    /// Shows the popup.
    /// </summary>
    /// <param name="text">The text.</param>
    /// <param name="position">The position.</param>
    public void ShowPopup(string text, Vector3 position)
	{
		if (uiGameObject)
			uiGameObject.BroadcastMessage ("ShowPopup", new object[]{ text, position });
	}

    /// <summary>
    /// Hides the popup.
    /// </summary>
    public void HidePopup()
	{if (uiGameObject) 
		uiGameObject.BroadcastMessage ("HidePopup");
	}


    /// <summary>
    /// Shows the drop menu.
    /// </summary>
    /// <param name="text">The text.</param>
    public void ShowDropMenu(string text)
	{
		if (uiGameObject) {
			uiGameObject.BroadcastMessage ("ShowDropMenu", text);
			}

	}

   
}