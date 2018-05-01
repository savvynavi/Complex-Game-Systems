using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys {
	public class TurnBehaviour : MonoBehaviour {
		ButtonBehaviour button = null;

		public List<Powers> abilitiesThisRound;
		public List<Transform> AvailablePlayers;
		public List<TurnInfo> MovesThisRound;
		public int numOfTurns;

		[System.Serializable]
		public struct TurnInfo{
			public Powers ability;
			public Character player;
		}

		// Use this for initialization
		void Start() {
			//find better solution won't work w/ mult button
			button = FindObjectOfType<ButtonBehaviour>().GetComponent<ButtonBehaviour>();
			numOfTurns = AvailablePlayers.Count;
			
		}

		//creates temp struct to hold passed in values then adds this to the list of moves
		public void TurnAddAttack(Powers pow, Character chara) {
			if(numOfTurns > 0) {
				TurnInfo tmp;
				tmp.ability = pow;
				tmp.player = chara;
				numOfTurns--;
				MovesThisRound.Add(tmp);
			}
		}

		//goes through power list, applies to target
		public void TurnApplyAttack() {
			foreach(TurnInfo info in MovesThisRound) {
				info.ability.Apply(info.player, info.player.target.GetComponent<Character>());
			}

			//clears the list after each round
			MovesThisRound.Clear();
			numOfTurns = AvailablePlayers.Count;
		}
	}
}

