using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : MonoBehaviour {
	//add in animations/sounds later
	//base stats
	public float speedStat;
	public float strStat;
	public float defStat;
	public float intStat;
	public float mindStat;
	public float hpStat;
	public float mpStat;
	public float dexStat;
	public float agiStat;

	//dictionary stuff
	public Dictionary<RPGStats.Stats, float> CharaStats = new Dictionary<RPGStats.Stats, float>();

	public float Speed {
		get { return CharaStats[RPGStats.Stats.Speed]; }
		set { CharaStats[RPGStats.Stats.Speed] = value; }
	}

	public float Str {
		get { return CharaStats[RPGStats.Stats.Str]; }
		set { CharaStats[RPGStats.Stats.Str] = value; }
	}
	public float Def {
		get { return CharaStats[RPGStats.Stats.Def]; }
		set { CharaStats[RPGStats.Stats.Def] = value; }
	}
	public float Int {
		get { return CharaStats[RPGStats.Stats.Int]; }
		set { CharaStats[RPGStats.Stats.Int] = value; }
	}
	public float Mind {
		get { return CharaStats[RPGStats.Stats.Mind]; }
		set { CharaStats[RPGStats.Stats.Mind] = value; }
	}
	public float Hp {
		get { return CharaStats[RPGStats.Stats.Hp]; }
		set { CharaStats[RPGStats.Stats.Hp] = value; }
	}
	public float Mp {
		get { return CharaStats[RPGStats.Stats.Mp]; }
		set { CharaStats[RPGStats.Stats.Mp] = value; }
	}
	public float Dex {
		get { return CharaStats[RPGStats.Stats.Dex]; }
		set { CharaStats[RPGStats.Stats.Dex] = value; }
	}
	public float Agi {
		get { return CharaStats[RPGStats.Stats.Agi]; }
		set { CharaStats[RPGStats.Stats.Agi] = value; }
	}
	public List<Powers> abilities;
	//public List<Powers> physAbilities;
	//public List<Powers> magicAbilities;
	public GameObject target;

	Material material;
	public List<Status> currentEffects;

	void Start() {
		material = GetComponent<Renderer>().material;
		Speed = speedStat;
		Str = strStat;
		Def = defStat;
		Int = intStat;
		Mind = mindStat;
		Hp = hpStat;
		Mp = mpStat;
		Dex = dexStat;
		Agi = agiStat;
	}

	//testing physical power
	void Update() {
		//if alive
		if(Hp > 0) {
			//phys attack
			if(abilities.Count > 0 && Input.GetKeyDown(KeyCode.C)) {
				target.GetComponent<Character>().Hp -= abilities[0].damage;
			}
			//magic attack
			//if(magicAbilities.Count > 0 && Input.GetKeyDown(KeyCode.V) && Mp >= magicAbilities[0].manaCost) {
			//	target.GetComponent<Character>().Hp -= magicAbilities[0].damage;
			//	Mp -= magicAbilities[0].manaCost;
			//}

			if(target.GetComponent<Character>().Hp <= 0) {
				target.GetComponent<Character>().Hp = 0;
				target.GetComponent<Character>().material.color = Color.red;
			}
		}
	}

	void ApplyStatusEffects() {
		//goes through buff list and applies them
		foreach(Status buff in currentEffects) {
			buff.Apply(GetComponent<GameObject>());
			if(buff.Sta) {

			}
		}
	}
}
