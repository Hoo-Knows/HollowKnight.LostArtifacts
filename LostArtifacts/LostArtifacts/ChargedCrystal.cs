using System;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class ChargedCrystal : Artifact
	{
		private float multiplier;

		public override void Initialize()
		{
			id = 8;
			unlocked = true;
			name = "Charged Crystal";
			description = "Though all crystals from the Peaks hold some amount of energy, this crystal that Myla stumbled upon is even more potent than usual. It pulses and glows with all of its might.";
			traitName = "Energized";
			traitDescription = "Nail arts deal increased damage";
			active = false;
			level = 0;
		}

		public override void Activate()
		{
			LostArtifacts.Instance.Log("Activating " + traitName);

			if(level == 1) multiplier = 1.1f;
			if(level == 2) multiplier = 1.2f;
			if(level == 3) multiplier = 1.4f;

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail)
			{
				if(hitInstance.Source.name.Contains("Great Slash") ||
					hitInstance.Source.name.Contains("Dash Slash") ||
					hitInstance.Source.name.Contains("Hit L") ||
					hitInstance.Source.name.Contains("Hit R"))
				{
					hitInstance.DamageDealt = (int)(hitInstance.DamageDealt * multiplier);
				}
			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			LostArtifacts.Instance.Log("Deactivating " + traitName);

			StopAllCoroutines();
			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}