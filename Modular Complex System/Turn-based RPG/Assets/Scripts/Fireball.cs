using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fireball", menuName = "RPG/Fireball", order = 1)]
public class Fireball : ScriptableObject {

	public float mod;
	//public RPGStats.DmgType stat = RPGStats.DmgType.Elemental;
	public RPGStats.DmgType damageType;
}
