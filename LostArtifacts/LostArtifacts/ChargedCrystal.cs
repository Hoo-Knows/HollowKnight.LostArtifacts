using System;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class ChargedCrystal : Artifact
	{
		public override int ID() => 8;
		public override string Name() => "Charged Crystal";
		public override string Description() => "Though all crystals from the Peaks hold some amount of energy, this crystal " +
			"that Myla stumbled upon is even more potent than usual. It pulses and glows with all of its might.";
		public override string Levels() => "+10%, +20%, +40% damage";
		public override string TraitName() => "Energized";
		public override string TraitDescription() => "Nail arts deal increased damage";

		private float multiplier;

		public override void Activate()
		{
			base.Activate();

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
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}