using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGsys{
	public class CharaParticle : MonoBehaviour{
		public ParticleSystem particle;
		public float duration;

		Character chara;

		Transform objTransform;

		private void Awake(){
			particle = Instantiate(particle);
			chara = GetComponent<Character>();
			
		}

		private void Update(){
			particle.transform.position = objTransform.position;
		}
	}
}