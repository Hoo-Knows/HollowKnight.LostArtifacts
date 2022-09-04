using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class TravelersGarment : Artifact
	{
		public override int ID() => 0;
		public override string Name() => "Traveler's Garment";
		public override string Description() => "A cloak from a traveler who braved the wasteland beyond to " +
			"reach Hallownest. It carries the aura of its former owner.";
		public override string LevelInfo() => "0%, 50%, 100% better scaling";
		public override string TraitName() => "Resilience";
		public override string TraitDescription() => "Scales damage with the player’s velocity over the past second";
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
				velocityArray[i] = Mathf.Abs(HeroController.instance.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude);
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
			return sum / 100f;
		}

		private float GetMultiplier()
		{
			float vel = GetAverageVelocity();
			LostArtifacts.Instance.Log(vel);
			float multiplier = 24.7f / (1f + 173f * Mathf.Exp(-0.46f * vel));
			if(vel > 13.73f) multiplier = -38f + 10.7f * Mathf.Log(vel - 1.6f, 1.6f);

			//Apply level multiplier
			multiplier *= (level + 1) * 0.5f;

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