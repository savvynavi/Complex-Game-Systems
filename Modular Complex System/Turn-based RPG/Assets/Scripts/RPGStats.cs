using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGStats : MonoBehaviour {

	public enum DmgType {
		Physical,
		Elemental
	}

	public enum Stats {
		Speed,
		Str,
		Def,
		Int,
		Mind,
		Hp,
		Mp,
		Dex,
		Agi
	}

	public enum ItemTypes {
		Head,
		Torso,
		Legs,
		Hand
	}
}
