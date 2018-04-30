using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;

namespace RPGsys {
	public class ButtonBehaviour : MonoBehaviour {

		List<Powers> powerList;

		public Button button;
		public GameObject canvas;

		private void Start() {
			//UnityEditor.Events.UnityEventTools.AddPersistentListener(button.onClick, HandleClick);
			//button.onClick.AddListener(HandleClick);
			powerList = GetComponent<Character>().classInfo.classPowers;
			Setup();
		}

		public void Setup() {
			int i = 1;
			foreach(Powers pow in powerList) {
				GameObject go = Instantiate(button.gameObject);
				button = go.GetComponent<Button>();
				button.GetComponent<Button>().onClick.AddListener(HandleClick);

				button.transform.position = transform.position + (32 * i) * Vector3.up;
				button.transform.SetParent(canvas.transform, false);
				button.name = pow.powName + "(" + i + ")";
				i++;
				button.GetComponentInChildren<Text>().text = pow.powName;
			}
		}

		//when button clicked, does this
		public void HandleClick() {
			foreach(Powers pow in powerList) {
				Debug.Log("Punch Button Works!");

				GetComponent<Character>().Attack();
			}
		}

		//private void OnApplicationQuit() {
		//	UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick, HandleClick);
		//}
	}
}
