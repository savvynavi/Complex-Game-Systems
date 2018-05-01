using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;

namespace RPGsys {
	public class ButtonBehaviour : MonoBehaviour {

		List<Powers> powerList;

		public List<Button> buttons;
		public Button button;
		public GameObject canvas;

		private void Awake() {
			powerList = GetComponent<Character>().classInfo.classPowers;
			buttons = new List<Button>();
			Setup();

		}

		public void Setup() {
			int count = 0;
			foreach(Powers pow in powerList) {
				GameObject go = Instantiate(button.gameObject);
				button = go.GetComponent<Button>();

				button.transform.position = transform.position + (32 * count) * Vector3.up;
				button.transform.SetParent(canvas.transform, false);
				button.name = pow.powName + "(" + (count + 1) + ")";
				button.GetComponentInChildren<Text>().text = pow.powName;
				buttons.Add(button);
				count++;
			}

			//adding listeners to each button
			for(int i = 0; i < buttons.Count; i++) {
				int capturedIndex = i;
				buttons[i].onClick.AddListener(() => HandleClick(capturedIndex));
			}
		}

		public void ShowButtons() {
			foreach(Button btn in buttons) {
				btn.enabled = true;
			}
		}

		public void HideButtons() {
			foreach(Button btn in buttons) {
				btn.enabled = false;
			}
		}

		//when button clicked, does this
		public void HandleClick(int capturedIndex) {
			//adds power to a list of all powers being used this round based on button pressed
			for(int i = 0; i < buttons.Count; i++) {
				if(buttons[capturedIndex].GetComponentInChildren<Text>().text == powerList[i].powName) {
					Debug.Log("Adding in " + powerList[i].powName + " to list");
					FindObjectOfType<TurnBehaviour>().TurnAddAttack(powerList[i], transform.GetComponent<Character>());
				}
			}
		}
	}
}
