using System;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class TravelersGarment : Artifact
	{
		private float[] velocityArray;

		public override void Initialize()
		{
			id = 0;
			unlocked = true;
			name = "Traveler's Garment";
			description = "A small cloth from a traveler who braved the wasteland beyond to reach Hallownest. It carries the aura of its former owner.";
			traitName = "Resilience";
			traitDescription = "Increases damage based on the player’s velocity over the past 3 seconds";
			active = false;
			level = 0;
		}

		public override void Activate()
		{
			LostArtifacts.Instance.Log("Activating " + traitName);

			velocityArray = new float[300];
			StartCoroutine(VelocityTracker());
			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail)
			{
				hitInstance.DamageDealt = (int)(hitInstance.DamageDealt * GetMultiplier());
			}
			orig(self, hitInstance);
		}

		private IEnumerator VelocityTracker()
		{
			int i = 0;
			while(active)
			{
				yield return new WaitForSeconds(0.01f);

				if(i >= 300) i = 0;
				velocityArray[i] = HeroController.instance.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
			}
			yield break;
		}

		private float GetAverageVelocity()
		{
			float sum = 0f;
			foreach(float velocity in velocityArray)
			{
				sum += velocity;
			}
			return sum / 300f;
		}

		private float GetMultiplier()
		{
			float multiplier = Mathf.Max(1f, Mathf.Min(125f * GetAverageVelocity() + 1f, 2.5f));

			//Apply level multiplier
			if(level == 1) multiplier *= 1f;
			if(level == 2) multiplier *= 1.1f;
			if(level == 3) multiplier *= 1.25f;

			return multiplier;
		}

		public override void Deactivate()
		{
			LostArtifacts.Instance.Log("Deactivating " + traitName);

			StopAllCoroutines();
			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}