using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : MonoBehaviour {
	public GameObject destination;
	Slider healthBar;
	//public float buffer;

	private void Start() {
		healthBar = GetComponent<Slider>();
		healthBar.maxValue = destination.GetComponent<Character>().hp;
		healthBar.interactable = false;
		//healthBar.fillRect;

		//set pos and move up by a buffer
		transform.position = Camera.main.WorldToScreenPoint(destination.transform.position);
		transform.position = new Vector3(transform.position.x, transform.position.y + destination.transform.localScale.y, transform.position.z);
		//transform.localScale *= 0.1f;
	}

	private void Update() {
		healthBar.value = destination.GetComponent<Character>().hp;
	}
}
