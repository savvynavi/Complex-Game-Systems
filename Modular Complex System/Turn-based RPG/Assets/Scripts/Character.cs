using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {

	enum stats {
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

	public List<Powers> abilities;

}
