﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPGsys {
	public class StateManager : MonoBehaviour {
		bool gameOver = false;
		//ButtonBehaviour buttons;
		WaitForSeconds endWait;
		List<Character> characters;
		List<Character> enemies;
		TurnBehaviour turnBehaviour;
		EnemyBehaviour[] enemyBehav;
		int rand;
		Quaternion originalRotation;

		GameObject GameOverUI;
		GameObject GameOverTextLose;
		GameObject GameOverTextWin;

		public float speed;
		public Button MainMenu;
		public Button Quit;
		public GameObject selector;
		public List<Transform> players;
		public List<Transform> publicEnemies;
		public float startDelay;
		public float endDelay;

		//////CHANGE ALL LISTS TO USE THE TURNbEHAV ONES FOR CLARITY


		// Use this for initialization
		void Start() {
			//buttons = FindObjectOfType<ButtonBehaviour>().GetComponent<ButtonBehaviour>();
			turnBehaviour = GetComponent<TurnBehaviour>();
			endWait = new WaitForSeconds(endDelay);
			characters = new List<Character>();
			enemies = new List<Character>();
			enemyBehav = FindObjectsOfType<EnemyBehaviour>();
			GameObject go = Instantiate(selector);
			selector = go;
			selector.SetActive(false);

			//game over menu stuff
			GameOverUI = GameObject.Find("GameOverMenu");
			MainMenu.onClick.AddListener(() => HandleClick(MainMenu));
			Quit.onClick.AddListener(() => HandleClick(Quit));

			GameOverTextLose = GameObject.Find("LoseText");
			GameOverTextWin = GameObject.Find("WinText");

			GameOverUI.SetActive(false);
			GameOverTextLose.SetActive(false);
			GameOverTextWin.SetActive(false);


			//filling private lists
			for(int i = 0; i < players.Count; i++) {
				characters.Add(players[i].GetComponent<Character>());
			}

			for(int i = 0; i < publicEnemies.Count; i++) {
				enemies.Add(publicEnemies[i].GetComponent<Character>());
			}

			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().Setup();
			}

			//shows enemy ui
			foreach(Character enemy in enemies) {
				enemy.GetComponent<EnemyUI>().enemyUISetup();
			}
			foreach(Character enemy in enemies) {
				enemy.GetComponent<EnemyUI>().ShowUI();
			}

			//starting game loops
			StartCoroutine(GameLoop());
		}

		//while at least 1 player is alive, will loop the gamestates starting with the player
		private IEnumerator GameLoop() {


			yield return PlayerTurn();
			yield return EnemyTurn();
			yield return ApplyMoves();

			//checking if alive to keep looping
			if(!BattleOver()) {
				StartCoroutine(GameLoop());
			}

			Debug.Log("peope are dead now");

			if(BattleOver() == true) {
				Debug.Log("menu popup");
				GameOverUI.SetActive(true);
				if(Alive() == true) {
					GameOverTextWin.SetActive(true);
					Debug.Log("you won");
				} else if(EnemyAlive() == true) {
					Debug.Log("you lost");
					GameOverTextLose.SetActive(true);
				}
			}
		}

		//Pauses while each character can choose a target + power to use
		private IEnumerator PlayerTurn() {

			yield return new WaitForEndOfFrame();

			List<Character> deadCharacters = new List<Character>();
			//if dead, remove from list
			foreach(Character chara in characters) {
				if(chara.Hp <= 0) {
					Debug.Log(chara.name + " is dead");
					chara.Hp = 0;
					deadCharacters.Add(chara);
				}
			}
			if(deadCharacters.Count > 0) {
				foreach(Character dead in deadCharacters) {
					characters.Remove(dead);
				}
			}
			
			int tmp = 0;
			foreach(Character chara in characters) {
				tmp += turnBehaviour.numOfTurns;
				chara.GetComponent<Animator>().SetBool("IdleTransition", true);
			}

			//loop through characters and wait until input to move to next one
			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().ShowButtons();


				foreach(Character chara2 in characters) {
					chara2.GetComponent<TargetSelection>().enabled = false;
				}
				chara.GetComponent<TargetSelection>().enabled = true;

				if(chara.target != null) {
					selector.transform.position = chara.target.transform.position;

				}
				while(chara.GetComponent<ButtonBehaviour>().playerActivated == false) {
					yield return null;
				}
				//sets character to animation to indicate that their move has passed
				chara.GetComponent<Animator>().SetBool("IdleTransition", false);
			}
		}

		//clear menu away, rand select move
		public IEnumerator EnemyTurn() {
			yield return new WaitForEndOfFrame();

			selector.SetActive(false);
			foreach(Character chara in characters) {
				chara.GetComponent<TargetSelection>().enabled = false;
			}

			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().HideButtons();
			}

			List<Character> deadEnemies = new List<Character>();
			foreach(Character enemy in enemies) {
				if(enemy.Hp <= 0) {
					Debug.Log(enemy.name + " is dead");
					enemy.Hp = 0;
					deadEnemies.Add(enemy);
					enemy.GetComponent<EnemyUI>().HideUI();

				}
			}
			if(deadEnemies.Count > 0) {
				foreach(Character dead in deadEnemies) {
					enemies.Remove(dead);
				}
			}

			for(int i = 0; i < enemies.Count; i++) {
				for(int j = 0; j < enemyBehav.Count(); j++) {
					if(enemies[i] == enemyBehav[j].GetChara) {
						rand = Random.Range(0, characters.Count);
						enemies[i].target = characters[rand].gameObject;
						enemyBehav[j].AddEnemyAttackRand(characters[rand]);
						break;
					}
				}
			}

			yield return new WaitForSeconds(0.5f);

		}

		//loop through moves on a delay, apply to targets
		public IEnumerator ApplyMoves() {

			//sort move list by speed
			List<TurnBehaviour.TurnInfo> sortedList = turnBehaviour.MovesThisRound.OrderByDescending(o => o.player.Speed).ToList();
			turnBehaviour.MovesThisRound = sortedList;

			foreach(TurnBehaviour.TurnInfo info in turnBehaviour.MovesThisRound) {
				originalRotation = info.player.transform.rotation;
				if(info.player.Hp > 0) {
					info.player.Timer();
					if(info.player.target == null) {
						rand = Random.Range(0, enemies.Count);
						info.player.target = enemies[rand].gameObject;
					}
					if(info.player.target.GetComponent<Character>().Hp <= 0) {
						//add check if target is dead, if so randomly selects enemy to fight
						for(int i = 0; i < enemies.Count; i++) {
							if(enemies[i].GetComponent<Character>().Hp > 0) {
								info.player.target = enemies[i].gameObject;
								break;
							} else {
								info.player.target = null;
							}
						}
					}

					if(info.player.target != null) {
						//turn player towards target

						info.player.transform.LookAt(info.player.target.transform);

						//does damage/animations
						info.ability.Apply(info.player, info.player.target.GetComponent<Character>());
						string name = info.ability.anim.ToString();
						info.player.GetComponent<Animator>().Play(name);
						info.player.target.GetComponent<Animator>().Play("TAKE_DAMAGE");
						//if player character, will allow them to go back to isle anim 
						if(info.player.tag != "Enemy") {
							info.player.GetComponent<Animator>().SetBool("IdleTransition", true);
						}
						//reset player rotation
						float step = speed * Time.deltaTime;
						//info.player.transform.rotation = Quaternion.Slerp(info.player.transform.rotation, originalRotation, speed);
					}
				}

				info.player.transform.rotation = Quaternion.Slerp(info.player.transform.rotation, originalRotation, speed);


				yield return new WaitForSeconds(info.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).length + 1.5f);
				if(info.player.target != null) {
					Death(info);
				}
			}

			//reset player lookat
			//foreach(TurnBehaviour.TurnInfo info in turnBehaviour.MovesThisRound) {
			//	//reset player rotation(find better spot for this, currently overrides earlier lookat)
			//	//info.player.transform.LookAt(new Vector3(-0.5f, 0, -3.5f));
			//	float step = speed * Time.deltaTime;
			//	//info.player.transform.rotation = Quaternion.RotateTowards(info.player.transform.rotation, new Quaternion(-0.5f, 0, -3.5f), step);
			//	info.player.transform.rotation = Quaternion.Slerp(info.player.transform.rotation, originalRotation, speed);
			//}

			turnBehaviour.MovesThisRound.Clear();
			turnBehaviour.numOfTurns = turnBehaviour.AvailablePlayers.Count;
			turnBehaviour.numOfEnemyTurns = turnBehaviour.AvailableEnemies.Count;

			yield return new WaitForSeconds(0.5f);
		}

		public void Death(TurnBehaviour.TurnInfo attackerInfo) {
			Character attackerTarget = attackerInfo.player.target.GetComponent<Character>();

			if(attackerTarget != null) {
				if(attackerTarget.Hp <= 0) {
					attackerTarget.Hp = 0;
					attackerTarget.GetComponent<Animator>().Play("DEAD");

					//remove buff effects on death
					foreach(Buff buff in attackerTarget.currentEffects) {
						buff.UpdateEffect(attackerTarget);
					}

					if(attackerTarget.gameObject.tag == "Enemy") {
						attackerTarget.GetComponent<EnemyUI>().HideUI();
						attackerTarget.GetComponent<Collider>().enabled = false;
					}
				}
			}
		}

		//if player is alive returns true, otherwise false
		public bool Alive() {
			foreach(Character chara in characters) {
				if(chara.Hp > 0) {
					return true;
				}
			}
			return false;
		}

		public bool EnemyAlive() {
			foreach(Character enemy in enemies) {
				if(enemy.Hp > 0) {
					return true;
				}
			}
			return false;
		}

		public bool BattleOver() {
			if((Alive() == true && EnemyAlive() == false) || (Alive() == false && EnemyAlive() == true)) {
				return true;
			}
			return false;
		}

		public void HandleClick(Button btn) {
			if(btn.GetComponentInChildren<Text>().text == "Main Menu") {
				SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
			}else if(btn.GetComponentInChildren<Text>().text == "Quit") {
				Application.Quit();
			}
		}
	}
}