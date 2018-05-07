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
		public float startDelay;
		public float endDelay;
		//public int numOfTurns;

		// Use this for initialization
		void Start() {
			buttons = FindObjectOfType<ButtonBehaviour>().GetComponent<ButtonBehaviour>();
			turnBehaviour = GetComponent<TurnBehaviour>();
			endWait = new WaitForSeconds(endDelay);
			characters = new List<Character>();
			enemyBehav = FindObjectOfType<EnemyBehaviour>();

			for(int i = 0; i < players.Count; i++) {
				characters.Add(players[i].GetComponent<Character>());
			}

			//mmove out of start

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
			int tmp = 0;
			foreach(Character chara in characters) {
				tmp += turnBehaviour.numOfTurns;
			}


			//if(tmp > 0) {
			//	//StartCoroutine(PlayerTurn()); crashes unity every time don't use

			//}

			//loop through characters and wait until input to move to next one
			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().ShowButtons();
				StartCoroutine(LoopCharacterButtons(chara));
				//chara.GetComponent<ButtonBehaviour>().HideButtons();
			}

			yield return endWait;
		}

		private IEnumerator LoopCharacterButtons(Character chara) {
			while(chara.GetComponent<ButtonBehaviour>().playerActivated != true) {
				yield return null;
			}
		}

		//clear menu away, rand select move
		public IEnumerator EnemyTurn() {
			Debug.Log("Enemy Turn State");
			foreach(Character chara in characters) {
				chara.GetComponent<ButtonBehaviour>().HideButtons();
			}

			//enemy move selection
			int rand = Random.Range(0, characters.Count);
			enemyBehav.AddEnemyAttackRand(characters[rand]);

			yield return new WaitForSeconds(0.5f);

		}

		//loop through moves on a delay, apply to targets
		public IEnumerator ApplyMoves() {
			Debug.Log("Applying Moves");

			foreach(TurnBehaviour.TurnInfo info in turnBehaviour.MovesThisRound) {
				info.ability.Apply(info.player, info.player.target.GetComponent<Character>());
				string name = info.ability.anim.ToString();
				info.player.GetComponent<Animator>().Play(name);
				yield return new WaitForSeconds(info.player.GetComponent<Animator>().GetCurrentAnimatorStateInfo(1).length + 1.5f);

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