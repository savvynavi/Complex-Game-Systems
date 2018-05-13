using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Events;
using UnityEngine.EventSystems;

namespace RPGsys {
	public class ButtonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

		float CharacterCurrentHP;
		float CharacterMaxHP;
		float CharacterCurrentMP;
		float CharacterMaxMP;

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

		public Transform hpPosition;
		public Transform mpPosition;
		public Image hp;
		public Image mp;
		public Image bg1;
		public Image bg2;

		public Text hpTxt;
		public Text mpTxt;
		public Transform hpTextPos;
		public Transform mpTextPos;

		private void Awake() {
			powerList = GetComponent<Character>().classInfo.classPowers;
			buttons = new List<Button>();
		}

		private void Start() {

			hp.type = Image.Type.Filled;
			hp.fillMethod = Image.FillMethod.Horizontal;
			mp.type = Image.Type.Filled;
			mp.fillMethod = Image.FillMethod.Horizontal;

			CharacterCurrentHP = GetComponent<Character>().Hp;
			CharacterMaxHP = GetComponent<Character>().hpStat;
			CharacterCurrentMP = GetComponent<Character>().Mp;
			CharacterMaxMP = GetComponent<Character>().mpStat;
		}

		private void Update() {
			CharacterCurrentHP = GetComponent<Character>().Hp;
			CharacterMaxHP = GetComponent<Character>().hpStat;
			CharacterCurrentMP = GetComponent<Character>().Mp;
			CharacterMaxMP = GetComponent<Character>().mpStat;
		}

		public void Setup() {
			int count = 0;
			playerActivated = false;
			charaName = transform.name;

			//setting up menu items in correct positions

			//menu bg
			GameObject tmpPaper = Instantiate(menuBG.gameObject);
			menuBG = tmpPaper.GetComponent<Image>();
			menuBG.transform.SetParent(canvas.transform, false);
			menuBG.transform.position = paperPos.transform.position;

			//hp/mp bars/bg
			GameObject tmpbg1 = Instantiate(bg1.gameObject);
			bg1 = tmpbg1.GetComponent<Image>();
			bg1.transform.SetParent(canvas.transform, false);
			bg1.transform.position = hpPosition.transform.position;

			GameObject tmpHp = Instantiate(hp.gameObject);
			hp = tmpHp.GetComponent<Image>();
			hp.transform.SetParent(canvas.transform, false);
			hp.transform.position = hpPosition.transform.position;

			GameObject tmpbg2 = Instantiate(bg2.gameObject);
			bg2 = tmpbg2.GetComponent<Image>();
			bg2.transform.SetParent(canvas.transform, false);
			bg2.transform.position = mpPosition.transform.position;

			GameObject tmpMp = Instantiate(mp.gameObject);
			mp = tmpMp.GetComponent<Image>();
			mp.transform.SetParent(canvas.transform, false);
			mp.transform.position = mpPosition.transform.position;

			//hp/mp text
			GameObject tmpHpTxt = Instantiate(hpTxt.gameObject);
			hpTxt = tmpHpTxt.GetComponent<Text>();
			hpTxt.transform.SetParent(canvas.transform, false);
			hpTxt.transform.position = hpTextPos.transform.position;
			hpTxt.text = CharacterCurrentHP.ToString() +  "/" + CharacterMaxHP.ToString();

			GameObject tmpMpTxt = Instantiate(mpTxt.gameObject);
			mpTxt = tmpMpTxt.GetComponent<Text>();
			mpTxt.transform.SetParent(canvas.transform, false);
			mpTxt.transform.position = mpTextPos.transform.position;
			mpTxt.text = CharacterCurrentMP.ToString() + "/" + CharacterMaxHP.ToString();

			//player name
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

				//hover button stuff
				//EventTrigger.Entry entry = new EventTrigger.Entry();
				//entry.eventID = EventTriggerType.PointerEnter;
				//entry.callback.AddListener((data) => { ButtonHover((PointerEventData)data); });

				//buttons[i].OnPointerEnter.AddListener((data) => ButtonHover((PointerEventData)data));
				buttons[i].transform.position = btnPositions[i].transform.position;
			}



			HideButtons();
		}

		public void ShowButtons() {
			menuBG.gameObject.SetActive(true);
			charaNameText.gameObject.SetActive(true);
			hp.gameObject.SetActive(true);
			float hpScale = Mathf.Clamp01(CharacterCurrentHP / CharacterMaxHP);
			hp.fillAmount = hpScale;

			mp.gameObject.SetActive(true);
			float mpScale = Mathf.Clamp01(CharacterCurrentMP / CharacterMaxMP);
			mp.fillAmount = mpScale;

			bg1.gameObject.SetActive(true);
			bg2.gameObject.SetActive(true);
			hpTxt.gameObject.SetActive(true);
			hpTxt.text = CharacterCurrentHP.ToString() + "/" + CharacterMaxHP.ToString();
			mpTxt.gameObject.SetActive(true);
			mpTxt.text = CharacterCurrentMP.ToString() + "/" + CharacterMaxHP.ToString();


			foreach(Button btn in buttons) {
				playerActivated = false;
				btn.gameObject.SetActive(true);
			}
		}

		public void HideButtons() {
			menuBG.gameObject.SetActive(false);
			charaNameText.gameObject.SetActive(false);
			hp.gameObject.SetActive(false);
			mp.gameObject.SetActive(false);
			bg1.gameObject.SetActive(false);
			bg2.gameObject.SetActive(false);
			hpTxt.gameObject.SetActive(false);
			mpTxt.gameObject.SetActive(false);

			foreach(Button btn in buttons) {
				btn.gameObject.SetActive(false);
			}

		}

		//when button clicked, does this
		public void HandleClick(int capturedIndex) {
			//adds power to a list of all powers being used this round based on button pressed
			for(int i = 0; i < buttons.Count; i++) {
				if(buttons[capturedIndex].GetComponentInChildren<Text>().text == powerList[i].powName) {
					FindObjectOfType<TurnBehaviour>().TurnAddAttack(powerList[i], transform.GetComponent<Character>());
					playerActivated = true;
				}
			}
		}

		public void ButtonHover(PointerEventData data) {
			Debug.Log("Mouse over button");
		}

		//use to have button info pop up on screen/clear
		public void OnPointerEnter(PointerEventData eventData) {
			Debug.Log("Mouse over button");
		}

		public void OnPointerExit(PointerEventData eventData) {
			Debug.Log("mouse off button");
		}
	}
}
