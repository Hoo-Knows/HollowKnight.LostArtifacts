using System;
using System.Collections.Generic;
using UnityEngine;
using Modding;

namespace LostArtifacts
{
	public class Buzzsaw : Artifact
	{
		public override int ID() => 17;
		public override string Name() => "Buzzsaw";
		public override string Description() => "Shaw";
		public override string Levels() => "0%, 50%, 100% better scaling";
		public override string TraitName() => "Secluded";
		public override string TraitDescription() => "Nail damage increases with distance from nearest enemy";

		public override void Activate()
		{
			base.Activate();

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

		private float GetMultiplier()
		{
			float distance = FindDistanceToEnemy();
			float multiplier = 20f / (1f + 50f * Mathf.Exp(-0.6f * distance));

			//Apply level multiplier
			multiplier *= 0.5f + level * 0.5f;

			return multiplier / 100f;
		}

		private float FindDistanceToEnemy()
		{
			List<HealthManager> hms = new List<HealthManager>(GameObject.FindObjectsOfType<HealthManager>());
			if(hms.Count == 0) return 0f;

			HealthManager next = hms[0];
			Vector3 pos = HeroController.instance.transform.position;
			foreach(HealthManager hm in hms)
			{
				if(Vector3.Distance(hm.transform.position, pos) < Vector3.Distance(next.transform.position, pos) &&
					Vector3.Distance(hm.transform.position, pos) < 10f)
				{
					next = hm;
				}
			}
			return Vector3.Distance(next.transform.position, pos);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}
