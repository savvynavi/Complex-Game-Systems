using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "RPG/StatusEffect", order = 2)]
public class Buff : Status {
	public StatusEffect StatusEffects;

	public override void Apply(Character target) {
		if(target != null) {
			Buff tmp = Instantiate(this);
			//should be adding instance of buff to the charaEffects list
			target.currentEffects.Add(tmp);
			
			//does effect here, fix later(not sustainable)
			switch(tmp.StatusEffects.effect) {
			case StatusEffectType.DmgBuff:
				target.Str += tmp.StatusEffects.amount;
				Debug.Log("strBuff active");
				break;
			case StatusEffectType.DmgDebuff:
				target.Str -= tmp.StatusEffects.amount;
				Debug.Log("strDebuff active");
				break;
			default:
				Debug.Log("error");
				break;
			}
		}
	}

	public override void Remove(Character target) {
		Debug.Log("Removing component");

	}
}