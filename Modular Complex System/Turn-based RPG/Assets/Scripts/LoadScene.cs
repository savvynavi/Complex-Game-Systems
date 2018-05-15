using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {
	public Button start;
	public Button controls;
	public Image controlImage;
	public Button back;

	// Use this for initialization
	void Start () {
		//Button go = Instantiate(back);
		//back = go;

		start.onClick.AddListener(() => HandleClick(start));
		controls.onClick.AddListener(() => HandleClick(controls));
		back.onClick.AddListener(() => HandleClick(back));
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}
	}

	public void HandleClick(Button btn) {
		if(btn.GetComponentInChildren<Text>().text == "Start") {
			SceneManager.LoadScene("Testing", LoadSceneMode.Single);
		} else if(btn.GetComponentInChildren<Text>().text == "Controls") {
			Debug.Log(btn.GetComponentInChildren<Text>().text);
		}
	}
}
