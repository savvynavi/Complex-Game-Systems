using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : MonoBehaviour {
	//add in animations/sounds later
	
	//base stats
	public float hp;
	public float mp;
	public float Speed;
	public float Str;
	public float Def;
	public float Int;
	public float Mind;
	public float Dex;
	public float Agi;
	public List<Powers> physAbilities;
	public List<Powers> magicAbilities;
	public GameObject target;

	Material material;
	public float currentHp;

	void Start() {
		material = GetComponent<Renderer>().material;
		currentHp = hp;
	}

	//testing physical power
	void Update() {
		//if alive
		if(currentHp > 0) {
			//phys attack
			if(physAbilities.Count > 0 && Input.GetKeyDown(KeyCode.C)) {
				target.GetComponent<Character>().currentHp -= physAbilities[0].damage;
				print(physAbilities[0].powName);
			}
			//magic attack
			if(magicAbilities.Count > 0 && Input.GetKeyDown(KeyCode.V) && mp >= magicAbilities[0].manaCost) {
				target.GetComponent<Character>().currentHp -= magicAbilities[0].damage;
				mp -= magicAbilities[0].manaCost;
				print(magicAbilities[0].powName);
			}

			if(target.GetComponent<Character>().currentHp <= 0) {
				target.GetComponent<Character>().currentHp = 0;
				target.GetComponent<Character>().material.color = Color.red;
			}
		}
	}
}
