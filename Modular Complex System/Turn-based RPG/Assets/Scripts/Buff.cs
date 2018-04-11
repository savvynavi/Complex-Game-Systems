using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys
{
	[CreateAssetMenu(fileName = "StatusEffect", menuName = "RPG/StatusEffect", order = 2)]
	public class Buff : Status{
		public StatusEffect StatusEffects;

		public override void Apply(Character target, float duration){
			if(target != null){
				Buff tmp = Instantiate(this);
				// make as copy of the particles
				tmp.Clone(target);
				tmp.timer = duration;
				//adding instance of buff to the charaEffects list
				target.currentEffects.Add(tmp);
				SetStats(target);
			}
		}

		public override void Remove(Character target){
			ResetStats(target);
			base.Remove(target);
			Debug.Log("Removing component");
		}

		void SetStats(Character target){
			//does effect here, fix later(not sustainable)
			switch(StatusEffects.effect) {
			case StatusEffectType.DmgBuff:
				target.Str += StatusEffects.amount;
				Debug.Log("strBuff active");
				break;
			case StatusEffectType.DmgDebuff:
				target.Str -= StatusEffects.amount;
				Debug.Log("strDebuff active");
				break;
			default:
				Debug.Log("error");
				break;
			}
		}

		void ResetStats(Character target) {
			//does effect here, fix later(not sustainable)
			switch(StatusEffects.effect) {
			case StatusEffectType.DmgBuff:
				target.Str -= StatusEffects.amount;
				Debug.Log("strBuff deactive");
				break;
			case StatusEffectType.DmgDebuff:
				target.Str += StatusEffects.amount;
				Debug.Log("strDebuff deactive");
				break;
			default:
				Debug.Log("error");
				break;
			}
		}
	}
}