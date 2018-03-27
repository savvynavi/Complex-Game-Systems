using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Magic", menuName = "RPG/Magic", order = 2)]
public class MagicPower : Powers {

	void Awake() {
		dmgType = RPGStats.DmgType.Elemental;
		damage = 50;
		target = Target.Single;
		manaCost = 5;
		powName = "Spells";
		duration = 1;
	}
}
