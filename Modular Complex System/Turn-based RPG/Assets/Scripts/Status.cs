using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : ScriptableObject {

	public enum StatusEffectType{
		DmgBuff,
		DefBuff,
		DmgDebuff,
		Poison
	}

	[System.Serializable]
	public struct StatusEffect {
		public StatusEffectType effect;
		public RPGStats.Stats statBuff;
		public float timer;
		public float amount;
	}

	void Update() {
		
	}

	//applies the effects once
	public virtual void Apply(GameObject target) {

	}
}
