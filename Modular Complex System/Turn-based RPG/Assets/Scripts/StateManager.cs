using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys {
	public class StateManager : MonoBehaviour {
		bool gameOver = false;
		ButtonBehaviour buttons;
		WaitForSeconds endWait;
		List<Character> characters;
		List<Character> enemies;
		TurnBehaviour turnBehaviour;
		EnemyBehaviour enemyBehav;
		int rand;

		public List<Transform> players;
		public List<Transform> publicEnemies;
		public float startDelay;
		public float endDelay;

		//////CHANGE ALL LISTS TO USE THE TURNbEHAV ONES FOR CLARITY


		// Use this for initialization
		void Start() {
			buttons = FindObjectOfType<ButtonBehaviour>().GetComponent<ButtonBehaviour>();
			turnBehaviour = GetComponent<TurnBehaviour>();
			endWait = new WaitForSeconds(endDelay);
			characters = new List<Character>();
			enemies = new List<Character>();
			enemyBehav = FindObjectOfType<EnemyBehaviour>();

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

			//starting game loops
			StartCoroutine(GameLoop());
		}

		//while at least 1 player is alive, will loop the gamestates starting with the player
		private IEnumerator GameLoop() {

			yield return PlayerTurn();
			yield return EnemyTurn();
			yield return ApplyMoves();


			//checking if alive to keep looping
			if(Alive()) {
				StartCoroutine(GameLoop());
			}
		}

		//make it pause for user input/menu stuff
		private IEnumerator PlayerTurn() {
			Debug.Log("Player Turn State");

			yield return new WaitForEndOfFrame();

			List<Character> deadCharacters = new List<Character>();
			//if dead, remove from list
			foreach(Character chara in characters) {
				Debug.Log(chara.name);
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

			Debug.Log(characters[0].Hp);

			int tmp = 0;
			foreach(Character chara in characters) {
				tmp += turnBehaviour.numOfTurns;
			}

			//loop through characters and wait until input to move to next one
			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().ShowButtons();
				while(chara.GetComponent<ButtonBehaviour>().playerActivated == false) {
					yield return null;
				}
			}
		}

		//clear menu away, rand select move
		public IEnumerator EnemyTurn() {
			Debug.Log("Enemy Turn State");
			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().HideButtons();
			}

			//enemy move selection
			foreach(Character enemy in enemies) {
				int rand = Random.Range(0, characters.Count);
				enemy.target = characters[rand].gameObject;
				enemyBehav.AddEnemyAttackRand(characters[rand]);
			}



			yield return new WaitForSeconds(0.5f);

		}

		//loop through moves on a delay, apply to targets
		public IEnumerator ApplyMoves() {
			Debug.Log("Applying Moves");

			foreach(TurnBehaviour.TurnInfo info in turnBehaviour.MovesThisRound) {
				info.player.Timer();
				info.ability.Apply(info.player, info.player.target.GetComponent<Character>());
				string name = info.ability.anim.ToString();
				info.player.GetComponent<Animator>().Play(name);
				yield return new WaitForSeconds(info.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).length + 1.5f);
			}

			//move back to see if fixes wrong dead person
			foreach(TurnBehaviour.TurnInfo info in turnBehaviour.MovesThisRound) {
				if(info.player.Hp <= 0) {
					characters[rand].Hp = 0;
					characters[rand].GetComponent<Animator>().Play("DEAD");
				}
			}

			turnBehaviour.MovesThisRound.Clear();
			turnBehaviour.numOfTurns = turnBehaviour.AvailablePlayers.Count;
			turnBehaviour.numOfEnemyTurns = turnBehaviour.AvailableEnemies.Count;

			yield return new WaitForSeconds(0.5f);
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
	}
}