using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Physical", menuName = "RPG/Physical", order = 1)]
public class PhysicalPower : Powers{

	private void Awake() {
		dmgType = RPGStats.DmgType.Physical;
		damage = 10;
		target = Target.Single;
		manaCost = 0;
		powName = "Punch";
		duration = 1;
	}
}
