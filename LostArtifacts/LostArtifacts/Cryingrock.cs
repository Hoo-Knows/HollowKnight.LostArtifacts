using System;
using System.Collections.Generic;
using Modding;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class Cryingrock : Artifact
	{
		public override int ID() => 4;
		public override string Name() => "Cryingrock";
		public override string Description() => "The never-ending rain seeps into the roads and buildings of the crumbling city, " +
			"creating rocks like this. The water embedded in the rock vaguely resembles tears.";
		public override string Levels() => "+30%, +40%, +50% damage";
		public override string TraitName() => "Fallen";
		public override string TraitDescription() => "Take one extra damage, but deal more damage for 5 seconds after taking damage";

		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			multiplier = 0.2f + level * 0.1f;

			ModHooks.TakeDamageHook += TakeDamageHook;
		}

		private int TakeDamageHook(ref int hazardType, int damage)
		{
			StopAllCoroutines();
			On.HealthManager.Hit -= HealthManagerHit;
			StartCoroutine(DamageControl());
			return damage + 1;
		}

		private IEnumerator DamageControl()
		{
			On.HealthManager.Hit += HealthManagerHit;
			yield return new WaitForSeconds(5f);
			On.HealthManager.Hit -= HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				hitInstance.Multiplier += multiplier;
			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.TakeDamageHook -= TakeDamageHook;
			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();
		}
	}
}
