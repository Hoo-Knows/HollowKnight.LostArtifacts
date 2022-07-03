using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class WeaverSilk : Artifact
	{
		public override int ID() => 12;
		public override string Name() => "Weaver Silk";
		public override string Description() => "Before the Weavers left Hallownest, they left behind some spools of silk. " +
			"Even when not woven into a Seal of Binding, they contain great power.";
		public override string LevelInfo() => "1, 2, 3 extra damage";
		public override string TraitName() => "Sealed";
		public override string TraitDescription() => "Striking an enemy adds flat damage to all instances of damage for 3 seconds";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Deepnest_45_v02),
				x = 115.3f,
				y = 13.4f,
				elevation = 0f
			};
		}

		public override void Activate()
		{
			base.Activate();

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				StopAllCoroutines();
				On.HealthManager.Hit -= Buff1;
				On.HealthManager.ApplyExtraDamage -= Buff2;
				StartCoroutine(DamageControl());
			}
			orig(self, hitInstance);
		}

		private IEnumerator DamageControl()
		{
			On.HealthManager.Hit += Buff1;
			On.HealthManager.ApplyExtraDamage += Buff2;
			yield return new WaitForSeconds(3f);
			On.HealthManager.Hit -= Buff1;
			On.HealthManager.ApplyExtraDamage -= Buff2;
		}

		private void Buff1(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			hitInstance.DamageDealt += level;
			orig(self, hitInstance);
		}

		private void Buff2(On.HealthManager.orig_ApplyExtraDamage orig, HealthManager self, int damageAmount)
		{
			orig(self, damageAmount + level);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			On.HealthManager.Hit -= Buff1;
			On.HealthManager.ApplyExtraDamage -= Buff2;
			StopAllCoroutines();
		}
	}
}
