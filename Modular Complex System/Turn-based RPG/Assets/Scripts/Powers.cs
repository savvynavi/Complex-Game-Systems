using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers : ScriptableObject {
	//add in animation/sounds for moves later when they can be tested

	//abilities can either target a group 1 person, no limits on friendly fire
	public enum Target {
		Group,
		Single
	}

	public float manaCost;
	public float damage;
	public RPGStats.DmgType dmgType;
	public Target target;
	public string powName;
	public int duration;
}
