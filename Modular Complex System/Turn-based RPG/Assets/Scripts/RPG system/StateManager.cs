﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RPGsys {
	public class StateManager : MonoBehaviour {
		public bool confirmMoves = false;
		public bool redoTurn = false;
		public float speed;
		public Button MainMenu;
		public Button Quit;
		public GameObject selector;

		int rand;
		List<Character> characters;
		List<Character> enemies;
		List<Character> deadCharactersREVIVE;
		List<Character> storeTargets;
		TurnBehaviour turnBehaviour;
		EnemyBehaviour[] enemyBehav;
		Quaternion originalRotation;
		GameObject GameOverUI;
		GameObject GameOverTextLose;
		GameObject GameOverTextWin;
		MoveConfirmMenu confirmMenu;
		CameraMovement camMovement;

		// Use this for initialization
		void Start() {
			turnBehaviour = GetComponent<TurnBehaviour>();
			confirmMenu = GetComponent<MoveConfirmMenu>();

			camMovement = GetComponent<CameraMovement>();
			storeTargets = new List<Character>();

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

			//grabbing players/enemies from the scene to fill lists
			GameObject[] tmp;
			tmp = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject chara in tmp) {
				if(chara.GetComponent<Character>() != null) {
					characters.Add(chara.GetComponent<Character>());
				}
			}

			//sort player list based on their choiceOrder number(so you can make it that the closest one to the screen picks first ect)
			List<Character> sortedList = characters.OrderBy(o => o.ChoiceOrder).ToList();
			characters = sortedList;

			tmp = GameObject.FindGameObjectsWithTag("Enemy");
			foreach(GameObject enemy in tmp) {
				if(enemy.GetComponent<Character>() != null) {
					enemies.Add(enemy.GetComponent<Character>());
				}
			}

			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().Setup();
			}

			List<Character> tmpList = new List<Character>();
			//when battle reentered, forces any dead characters to act like it
			foreach(Character chara in characters) {
				Debug.Log(chara.name + "'s HP: " + chara.Hp);
				if(chara.Hp <= 0) {
					tmpList.Add(chara);
					Death(chara, tmpList);
				}
			}

			//shows enemy ui
			foreach(Character enemy in enemies) {
				enemy.GetComponent<EnemyUI>().enemyUISetup();
			}
			foreach(Character enemy in enemies) {
				enemy.GetComponent<EnemyUI>().ShowUI();
			}


			turnBehaviour.Setup(characters, enemies);
			confirmMenu.Setup();

			//starting game loops
			StartCoroutine(GameLoop());
		}

		//while at least 1 player is alive, will loop the gamestates starting with the player
		private IEnumerator GameLoop() {


			yield return PlayerTurn();
			while(confirmMoves == false){
				yield return LockInMoves();
			}
			yield return EnemyTurn();
			yield return ApplyMoves();

			//checking if alive to keep looping
			if(!BattleOver()) {
				yield return GameLoop();
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
			redoTurn = false;
			confirmMoves = false;
			//characters.GetComponent<ButtonBehaviour>().undoMove = false;
			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().undoMove = false;
			}

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
			for(int i = 0; i < characters.Count; i++) {
				characters[i].GetComponent<ButtonBehaviour>().ShowButtons();
				foreach(Character chara2 in characters) {
					chara2.GetComponent<TargetSelection>().enabled = false;
				}
				characters[i].GetComponent<TargetSelection>().enabled = true;

				//selector only visible if the target isn't null
				if(characters[i].target != null) {
					selector.gameObject.SetActive(true);
					selector.transform.position = characters[i].target.transform.position;
				} else {
					selector.gameObject.SetActive(false);
				}

				int currentPlayer = i;
				int previousPlayer = i - 1;
				while(characters[currentPlayer].GetComponent<ButtonBehaviour>().playerActivated == false) {
					//if undo button hit, sets current player to previous, sets undo to false
					if(characters[currentPlayer].GetComponent<ButtonBehaviour>().undoMove == true) {
						characters[currentPlayer].GetComponent<ButtonBehaviour>().undoMove = false;
						characters[currentPlayer].GetComponent<ButtonBehaviour>().playerActivated = true;
						Debug.Log("Undoing Action");
						currentPlayer = previousPlayer;
					}
					yield return null;
				}

				//if undo button hit, sets previous player to idle anim, hides buttons of current, removes the last set move and sets i to be 1 less than prev(does this as on next loop will auto i++)
				if(currentPlayer == previousPlayer) {
					characters[currentPlayer].GetComponent<Animator>().SetBool("IdleTransition", true);
					characters[i].GetComponent<ButtonBehaviour>().HideButtons();
					//turnBehaviour.MovesThisRound.RemoveAt(turnBehaviour.MovesThisRound.Count - 1);
					i = previousPlayer - 1;

				} else {
					characters[i].GetComponent<Animator>().SetBool("IdleTransition", false);
					characters[i].GetComponent<ButtonBehaviour>().HideButtons();
				}

			}
		}

		public IEnumerator LockInMoves() {
			confirmMenu.ShowMenu();
			while(confirmMoves != true) {
				yield return null;
			}
			confirmMenu.HideMenu();
			if(redoTurn == true) {
				Debug.Log("in the yield now going to player turn again");
				turnBehaviour.MovesThisRound.Clear();
				turnBehaviour.ResetTurnNumber();
				yield return PlayerTurn();
			}
			yield return true;
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

			//each loop is a players turn
			foreach(TurnBehaviour.TurnInfo info in turnBehaviour.MovesThisRound) {
				originalRotation = info.player.transform.rotation;
				if(info.player.Hp > 0) {
					info.player.Timer();
					//died due to effect SET UP BETTER DEATH CHECK SYSTEM THIS IS GETTING SILLY
					if(info.player.Hp <= 0) {
						List<Character> tmp = new List<Character>();
						tmp.Add(info.player);
						Death(info.player, tmp);
					}

					if(info.player.Hp > 0) {
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
							camMovement.LookAtAttacker(info.player);
							yield return new WaitForSeconds(0.5f);


							//does damage/animations
							if(info.ability.areaOfEffect == Powers.AreaOfEffect.Group) {

								if(info.player.target.tag == "Player") {
									AddToStoredList(characters);
								} else {
									AddToStoredList(enemies);
								}
								camMovement.LookAtGroup(storeTargets);

								if(info.player.target.tag == "Player") {
									GroupAttack(info, characters, storeTargets);
								} else {
									GroupAttack(info, enemies, storeTargets);
								}


							} else if(info.ability.areaOfEffect == Powers.AreaOfEffect.Single) {

								//if same team, use facecam, else single out enemy target
								if(info.player.tag == info.player.target.tag) {
									camMovement.LookAtAttacker(info.player.target.GetComponent<Character>());
								} else {
									camMovement.LookAtTarget(info.player, info.player.target.GetComponent<Character>());
								}


								info.ability.Apply(info.player, info.player.target.GetComponent<Character>());
								string name = info.ability.anim.ToString();
								info.player.GetComponent<Animator>().Play(name);



								info.player.target.GetComponent<Animator>().Play("TAKE_DAMAGE");
								//if player character, will allow them to go back to isle anim 
								if(info.player.tag != "Enemy") {
									info.player.GetComponent<Animator>().SetBool("IdleTransition", true);
								}
								storeTargets.Add(info.player.target.GetComponent<Character>());
							} else {
								storeTargets = null;
							}

							//foreach(Character chara in characters) {
							//	if(chara != info.player && chara != info.player.target.GetComponent) {
							//		chara.gameObject.SetActive(false);
							//	}
							//}

							//foreach(Character enem in enemies) {
							//	if(enem != info.player && enem != info.player.target) {
							//		enem.gameObject.SetActive(false);
							//	}
							//}

							yield return new WaitForSeconds(1.5f);



							//reset player rotation
							float step = speed * Time.deltaTime;

							//waits for attack anim to finish before spinning character back towards front
							yield return new WaitForSeconds(info.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - info.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
							info.player.transform.rotation = Quaternion.Slerp(info.player.transform.rotation, originalRotation, speed);
							camMovement.Reset();

							//foreach(Character chara in characters) {
							//	if(chara != info.player || chara != info.player.target) {
							//		chara.gameObject.SetActive(true);
							//	}
							//}

							//foreach(Character enem in enemies) {
							//	if(enem != info.player || enem != info.player.target) {
							//		enem.gameObject.SetActive(true);
							//	}
							//}
						}

					}
				}

				yield return new WaitForSeconds(info.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).length + 1.5f);

				if(storeTargets != null) {
					Death(info.player.target.GetComponent<Character>(), storeTargets);
				}
				//if either side dead already, end fight
				if(BattleOver() == true) {
					yield return EndBattle();
					break;
				}
			}

			turnBehaviour.MovesThisRound.Clear();
			turnBehaviour.numOfTurns = turnBehaviour.AvailablePlayers.Count;
			turnBehaviour.numOfEnemyTurns = turnBehaviour.AvailableEnemies.Count;

			yield return new WaitForSeconds(0.5f);
		}

		public IEnumerator EndBattle() {
			// Cleanup button behaviours
			List<ButtonBehaviour> buttonBehaviours = new List<ButtonBehaviour>();

			turnBehaviour.MovesThisRound.Clear();

			foreach(Character chara in characters) {
				chara.GetComponent<Animator>().SetBool("IdleTransition", true);
			}

			GameOverUI.SetActive(true);
			if(Alive() == true) {
				GameOverTextWin.SetActive(true);
			} else if(EnemyAlive() == true) {
				GameOverTextLose.SetActive(true);
			}
			yield return new WaitForSeconds(0.5f);
		}

		public void Death(Character attackerTarget, List<Character> targets) {
			//if no list given, do 1 target, else loop over all targets
			if(targets != null) {
				foreach(Character target in targets) {
					if(target.Hp <= 0) {
						target.Hp = 0;
						target.GetComponent<Animator>().Play("DEAD");

						foreach(Buff buff in target.currentEffects) {
							buff.UpdateEffect(target);
						}

						if(target.gameObject.tag == "Enemy") {
							target.GetComponent<EnemyUI>().HideUI();
							target.GetComponent<Collider>().enabled = false;

						}
						//if it's a player it is added to the deadCharacter list, will be used for revives in battle
						if(target.gameObject.tag == "Player") {
							deadCharactersREVIVE.Add(target);
						}
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

		public void AddToStoredList(List<Character> targets) {
			foreach(Character chara in targets) {
				storeTargets.Add(chara);
			}
		}

		public void GroupAttack(TurnBehaviour.TurnInfo info, List<Character> targets, List<Character> storeTargets) {
			foreach(Character chara in targets) {
				info.ability.Apply(info.player, chara);
				string name = info.ability.anim.ToString();
				info.player.GetComponent<Animator>().Play(name);
				chara.GetComponent<Animator>().Play("TAKE_DAMAGE");
				//storeTargets.Add(chara);
			}

			//if player character, will allow them to go back to isle anim 
			if(info.player.tag != "Enemy") {
				info.player.GetComponent<Animator>().SetBool("IdleTransition", true);
			}
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