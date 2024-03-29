﻿using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class TravelersGarment : Artifact
	{
		public override int ID() => 0;
		public override string Name() => "Traveler's Garment";
		public override string LoreDescription() => "A cloak from a traveler who braved the wasteland beyond to " +
			"reach Hallownest. It carries the aura of its former owner.";
		public override string LevelInfo() => string.Format("Final damage buff is increased by {0}%", 25 * (level - 1));
		public override string TraitName() => "Resilience";
		public override string TraitDescription() => "Damage scales with the player’s highest velocity over the past second.";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Town),
				x = 189f,
				y = 8.4f,
				elevation = 0f
			};
		}

		private float[] velocityArray;
		private Rigidbody2D rb = null;
		private Rigidbody2D RB => rb ??= HeroController.instance.gameObject.GetComponent<Rigidbody2D>();

		public override void Activate()
		{
			base.Activate();

			velocityArray = new float[100];
			StartCoroutine(VelocityTracker());
			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				hitInstance.Multiplier += GetMultiplier();
			}
			orig(self, hitInstance);
		}

		private IEnumerator VelocityTracker()
		{
			int i = 0;
			while(active)
			{
				yield return new WaitForSeconds(0.01f);

				if(i >= 100) i = 0;
				velocityArray[i] = Mathf.Abs(RB.velocity.magnitude);
				i++;
			}
			yield break;
		}

		private float GetHighestVelocity()
		{
			float max = 0f;
			foreach(float velocity in velocityArray)
			{
				if(velocity > max) max = velocity;
			}
			return max;
		}

		private float GetMultiplier()
		{
			float multiplier = 1.4f * GetHighestVelocity();

			//Apply level multiplier
			multiplier *= (level + 3) * 0.25f;
			return multiplier / 100f;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();
		}
	}
}