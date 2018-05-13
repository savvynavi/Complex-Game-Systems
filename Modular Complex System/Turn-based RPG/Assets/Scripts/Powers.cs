using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys
{
	[CreateAssetMenu(fileName = "Ability", menuName = "RPG/Ability", order = 1)]
	public class Powers : ScriptableObject
	{
		//add in animation/sounds for moves later when they can be tested

		//abilities can either target a group 1 person, no limits on friendly fire
		public enum Target{
			Group,
			Single
		}

		public enum AbilityAnim {
			RIGHT_PUNCH,
			KICK,
			SPELL,
			ORC_AXE,
			DEATH
		};

		public float manaCost;
		public float damage;
		public RPGStats.DmgType dmgType;
		public RPGStats.Stats statType;
		public Target target;
		public AbilityAnim anim; 
		public string powName;
		public float duration;
		public List<Status> currentEffects;

		///public AnimationClip anim;

		public void Apply(Character obj ,Character target){

			float attMod;
			//get stat that is being affected
			switch(statType) {
			case RPGStats.Stats.Speed:
				attMod = obj.Speed;
				break;
			case RPGStats.Stats.Str:
				attMod = obj.Str;
				break;
			case RPGStats.Stats.Def:
				attMod = obj.Def;
				break;
			case RPGStats.Stats.Int:
				attMod = obj.Int;
				break;
			case RPGStats.Stats.Mind:
				attMod = obj.Mind;
				break;
			case RPGStats.Stats.Hp:
				attMod = obj.Hp;
				break;
			case RPGStats.Stats.Mp:
				attMod = obj.Mp;
				break;
			case RPGStats.Stats.Dex:
				attMod = obj.Dex;
				break;
			case RPGStats.Stats.Agi:
				attMod = obj.Agi;
				break;
			default:
				Debug.Log("no given attack mod type, adding zero to damage");
				attMod = 0;
				break;
			}

			//decrease target hp by damage amount + the chatacters given stat
			target.Hp -= (damage + attMod);
			//setAnimName(obj.transform);
			//Debug.Log(attMod);
			//Debug.Log(obj.Str);

			//loops over current effects on this power, applies them to the target
			for(int i = 0; i < currentEffects.Count; i++) {
				currentEffects[i].Apply(target, duration);
			}
		}

		void setAnimName(Transform obj) {
			obj.GetComponent<Animator>().name = anim.ToString();
		}
	}
}