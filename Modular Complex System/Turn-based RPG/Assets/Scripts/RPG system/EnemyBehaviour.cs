using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGsys {

	[RequireComponent(typeof(Character))]
	public class EnemyBehaviour : MonoBehaviour {
		Character chara = null;
		TurnBehaviour turnBehav = null;
		int rand;

		public Button button;
		public Canvas canvas;
		public List<Transform> btnPos;

		private void Awake() {
			chara = GetComponent<Character>();
			turnBehav = FindObjectOfType<TurnBehaviour>();
		}

		void enemyButtonSetup(){
			GameObject go = Instantiate(button.gameObject);
			button = go.GetComponent<Button>();

			button.transform.SetParent(canvas.transform, false);
			button.GetComponentInChildren<Text>().text = name;

			button.onClick.AddListener(() => HandleClick());
		}

		//when the enemy button is clicked, changes the character target that clicked it
		public void HandleClick() {

		}

		// Update is called once per frame
		public void AddEnemyAttackRand(Character target) {
			chara.target = target.gameObject;
			rand = Random.Range(0, chara.classInfo.classPowers.Count);
			turnBehav.turnAddAttackEnemy(chara.classInfo.classPowers[rand], chara);
		}
	}

}