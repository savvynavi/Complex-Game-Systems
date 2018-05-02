using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys {
	public class StateManager : MonoBehaviour {
		bool gameOver = false;
		ButtonBehaviour buttons;
		WaitForSeconds endWait;
		List<Character> characters;
		TurnBehaviour turnBehaviour;

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

			for(int i = 0; i < players.Count; i++) {
				characters.Add(players[i].GetComponent<Character>());
			}

			buttons.Setup();

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
			buttons.ShowButtons();
			int tmp = 0;
			foreach(Character chara in characters) {
				tmp += turnBehaviour.numOfTurns;
			}


			if(tmp > 0) {
				//StartCoroutine(PlayerTurn()); crashes unity every time don't use

			}
			yield return endWait;
		}

		//clear menu away, rand select move
		public IEnumerator EnemyTurn() {
			Debug.Log("Enemy Turn State");
			buttons.HideButtons();
			yield return endWait;

		}

		//loop through moves on a delay, apply to targets
		public IEnumerator ApplyMoves() {
			Debug.Log("Applying Moves");
			//turnBehaviour.TurnApplyAttack();

			foreach(TurnBehaviour.TurnInfo info in turnBehaviour.MovesThisRound) {
				info.ability.Apply(info.player, info.player.target.GetComponent<Character>());
				yield return new WaitForSeconds(2);

			}

			turnBehaviour.MovesThisRound.Clear();
			turnBehaviour.numOfTurns = turnBehaviour.AvailablePlayers.Count;

			yield return endWait;
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