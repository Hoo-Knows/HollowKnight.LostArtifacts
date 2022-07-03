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
		public override string Description() => "A small cloth from a traveler who braved the wasteland beyond to " +
			"reach Hallownest. It carries the aura of its former owner.";
		public override string LevelInfo() => "0%, 50%, 100% better scaling";
		public override string TraitName() => "Resilience";
		public override string TraitDescription() => "Scales damage with the player’s horizontal speed over the past 3 seconds";
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

			velocityArray = new float[300];
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
			float multiplier = 21f / (1f + 330f * Mathf.Exp(-0.46f * vel));
			if(vel >= 16.607f) multiplier = -41.4f + 9.5f * Mathf.Log(vel + 2.4f, 1.6f);

			//Apply level multiplier
			multiplier *= 0.5f + level * 0.5f;

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