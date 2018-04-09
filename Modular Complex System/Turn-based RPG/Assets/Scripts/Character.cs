using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RPGsys{
	public class Character : MonoBehaviour{
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

		public float Speed{
			get { return CharaStats[RPGStats.Stats.Speed]; }
			set { CharaStats[RPGStats.Stats.Speed] = value; }
		}

		public float Str{
			get { return CharaStats[RPGStats.Stats.Str]; }
			set { CharaStats[RPGStats.Stats.Str] = value; }
		}
		public float Def{
			get { return CharaStats[RPGStats.Stats.Def]; }
			set { CharaStats[RPGStats.Stats.Def] = value; }
		}
		public float Int{
			get { return CharaStats[RPGStats.Stats.Int]; }
			set { CharaStats[RPGStats.Stats.Int] = value; }
		}
		public float Mind{
			get { return CharaStats[RPGStats.Stats.Mind]; }
			set { CharaStats[RPGStats.Stats.Mind] = value; }
		}
		public float Hp{
			get { return CharaStats[RPGStats.Stats.Hp]; }
			set { CharaStats[RPGStats.Stats.Hp] = value; }
		}
		public float Mp{
			get { return CharaStats[RPGStats.Stats.Mp]; }
			set { CharaStats[RPGStats.Stats.Mp] = value; }
		}
		public float Dex{
			get { return CharaStats[RPGStats.Stats.Dex]; }
			set { CharaStats[RPGStats.Stats.Dex] = value; }
		}
		public float Agi{
			get { return CharaStats[RPGStats.Stats.Agi]; }
			set { CharaStats[RPGStats.Stats.Agi] = value; }
		}
		public Powers abilities;
		public GameObject target;

		Material material;
		public List<Status> currentEffects;
		List<Status> deadEffects;

		void Start(){
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

			//instantiating powers
			abilities = Instantiate(abilities);
		}

		//testing physical power
		void Update(){
			//if alive
			if(Hp > 0){
				//attack
				if(abilities != null && Input.GetKeyDown(KeyCode.Space)){
					target.GetComponent<Character>().Hp -= abilities.damage + Str;
					//if there are buffs on the move, apply them
					if(abilities.currentEffects.Count > 0){
						ApplyStatusEffects();
					}
				}
				if(target.GetComponent<Character>().Hp <= 0){
					target.GetComponent<Character>().Hp = 0;
					target.GetComponent<Character>().material.color = Color.red;
				}
			}
			UpdateStats();
			Timer();
		}

		void UpdateStats(){
			speedStat = Speed;
			strStat = Str;
			defStat = Def;
			intStat = Int;
			mindStat = Mind;
			hpStat = Hp;
			mpStat = Mp;
			dexStat = Dex;
			agiStat = Agi;
		}

		void ApplyStatusEffects(){
			//tmp solution
			foreach(Status effect in abilities.currentEffects){
				Debug.Log("inside chara stat loop");
				effect.Apply(this);
				//Destroy(effect, abilities.duration);
			}
		}

		//if timer less than zero, remove from effect list
		void Timer(){
			foreach(Status effect in abilities.currentEffects){
				if(abilities.duration >= 0){
					abilities.duration -= Time.deltaTime;
				} else{
					Debug.Log("Timer hitting zero");
					currentEffects.Remove(currentEffects[0]);
				}
			}
		}
	}
}