using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.EventSystems;

namespace RPGsys {
	public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

		List<Powers> powerList;
		string charaName;

		public bool playerActivated;
		public List<Button> buttons;
		public Button button;
		public GameObject canvas;
		public List<Transform> btnPositions;
		public Transform paperPos;
		public Image menuBG;
		public Transform namePos;
		public Text charaNameText;

		private void Awake() {
			powerList = GetComponent<Character>().classInfo.classPowers;
			buttons = new List<Button>();
		}

		public void Setup() {
			int count = 0;
			playerActivated = false;
			charaName = transform.name;

			GameObject tmpPaper = Instantiate(menuBG.gameObject);
			menuBG = tmpPaper.GetComponent<Image>();
			menuBG.transform.SetParent(canvas.transform, false);
			menuBG.transform.position = paperPos.transform.position;


			GameObject tmpTxt = Instantiate(charaNameText.gameObject);
			charaNameText = tmpTxt.GetComponent<Text>();
			charaNameText.transform.SetParent(canvas.transform, false);
			charaNameText.transform.position = namePos.transform.position;
			charaNameText.text = transform.GetComponent<Character>().name;

			//setting up each power with a button
			foreach(Powers pow in powerList) {
				GameObject go = Instantiate(button.gameObject);
				button = go.GetComponent<Button>();

				//button.transform.position = transform.position + (32 * count) * Vector3.up;
				button.transform.SetParent(canvas.transform, false);
				button.name = pow.powName + "(" + (count + 1) + ")";
				button.GetComponentInChildren<Text>().text = pow.powName;
				buttons.Add(button);
				count++;
			}

			//adding listeners to each button/settting active state to false
			for(int i = 0; i < buttons.Count; i++) {
				int capturedIndex = i;
				buttons[i].onClick.AddListener(() => HandleClick(capturedIndex));
				buttons[i].transform.position = btnPositions[i].transform.position;
			}



			HideButtons();
		}

		public void ShowButtons() {
			menuBG.gameObject.SetActive(true);
			charaNameText.gameObject.SetActive(true);

			foreach(Button btn in buttons) {
				playerActivated = false;
				btn.gameObject.SetActive(true);
			}
		}

		public void HideButtons() {
			menuBG.gameObject.SetActive(false);
			charaNameText.gameObject.SetActive(false);

			foreach(Button btn in buttons) {
				btn.gameObject.SetActive(false);
			}

		}

		//when button clicked, does this
		public void HandleClick(int capturedIndex) {
			//adds power to a list of all powers being used this round based on button pressed
			for(int i = 0; i < buttons.Count; i++) {
				if(buttons[capturedIndex].GetComponentInChildren<Text>().text == powerList[i].powName) {
					Debug.Log("Adding in " + powerList[i].powName + " to list");
					FindObjectOfType<TurnBehaviour>().TurnAddAttack(powerList[i], transform.GetComponent<Character>());
					playerActivated = true;
				}
			}
		}


		//use to have button info pop up on screen/clear
		public void OnPointerEnter(PointerEventData eventData) {
			Debug.Log("Mouse over button");
		}

		public void OnPointerExit(PointerEventData eventData) {
			
		}
	}
}
