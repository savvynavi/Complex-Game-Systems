using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys{
	public class Status : ScriptableObject{

		public ParticleSystem particles;
		protected GameObject partInst;
		bool particleRunning;

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
			public float amount;
		}
		public float timer;

		public StatusEffect statusEffect;

		private void Awake(){

		}

		virtual public void UpdateEffect(Character chara){
			timer -= Time.deltaTime;
			if(timer < particles.main.startLifetime.constant) {
				partInst.GetComponent<ParticleSystem>().Stop();
			}
		}

		//applies the effects once
		public virtual void Apply(Character target, float duration){

		}

		public virtual void Remove(Character target){
			Destroy(partInst);
		}

		public void Clone(Character target){
			// make as copy of the particles
			partInst = Instantiate(particles.gameObject);
			partInst.transform.parent = target.transform;
			partInst.transform.localPosition = Vector3.zero;
		}
	}
}