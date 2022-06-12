using System;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class TravelersGarment : Artifact
	{
		public override int ID() => 0;
		public override string Name() => "Traveler's Garment";
		public override string Description() => "A small cloth from a traveler who braved the wasteland beyond to " +
			"reach Hallownest. It carries the aura of its former owner.";
		public override string Levels() => "0%, 10%, 20% better scaling";
		public override string TraitName() => "Resilience";
		public override string TraitDescription() => "Scales damage based on the player’s average horizontal velocity";

		private float[] velocityArray;

		public override void Activate()
		{
			base.Activate();

			velocityArray = new float[300];
			StartCoroutine(VelocityTracker());
			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
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
				velocityArray[i] = Mathf.Abs(HeroController.instance.gameObject.GetComponent<Rigidbody2D>().velocity.x);
				i++;
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
			float vel = GetAverageVelocity();
			float multiplier = 40f / (1f + 416f * Mathf.Exp(-0.47f * vel));
			if(vel >= 18.149f) multiplier = -16.15f + 10.2f * Mathf.Log(vel - 2.3f, 1.7f);

			//Apply level multiplier
			if(level == 1) multiplier *= 1f;
			if(level == 2) multiplier *= 1.1f;
			if(level == 3) multiplier *= 1.2f;

			return multiplier / 100f + 1f;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();
		}
	}
}