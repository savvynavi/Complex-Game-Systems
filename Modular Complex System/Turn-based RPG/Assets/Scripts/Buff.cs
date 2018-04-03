using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatusEffect", menuName = "RPG/StatusEffect", order = 2)]
public class Buff : Status {
	public StatusEffect StatusEffects;

	//Buff() {

	//}

	public override void Apply(GameObject target) {
		if(target != null && target.GetComponent<Character>() != null) {
			//target.GetComponent<Character>().currentEffects.Add(StatusEffects);
		}
	}
}
