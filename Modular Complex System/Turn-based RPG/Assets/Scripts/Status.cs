using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys{
	public class Status : ScriptableObject{

		public ParticleSystem particles;
		protected GameObject partInst;

		//public Animation anim;
		public enum StatusEffectType{
			DmgBuff,
			DefBuff,
			DmgDebuff,
			Poison
		}

		[System.Serializable]
		public struct StatusEffect{
			public StatusEffectType effect;
			public RPGStats.Stats statBuff;
			public float timer;
			public float amount;
		}

		private void Awake(){

		}

		void Update(){

		}

		//applies the effects once
		public virtual void Apply(Character target){

		}

		public virtual void Remove(Character target){

		}

		public void Clone(Character target){
			// make as copy of the particles
			partInst = Instantiate(particles.gameObject);
			partInst.transform.parent = target.transform;
			partInst.transform.localPosition = Vector3.zero;
		}
	}
}