using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;
using Satchel;

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
		public override string TraitDescription() => "Releasing a nail art adds flat damage to all instances of damage for 10 seconds";
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

		private int buffActive;
		private PlayMakerFSM nartFSM;

		public override void Activate()
		{
			base.Activate();

			buffActive = 0;
			nartFSM = HeroController.instance.gameObject.LocateMyFSM("Nail Arts");
			nartFSM.InsertCustomAction("Regain Control", () =>
			{
				StartCoroutine(BuffControl());
			}, 0);
		}

		private IEnumerator BuffControl()
		{
			buffActive++;
			if(buffActive == 1)
			{
				On.HealthManager.Hit += Buff1;
				On.HealthManager.ApplyExtraDamage += Buff2;
			}
			yield return new WaitForSeconds(10f);
			buffActive--;
			if(buffActive == 0)
			{
				On.HealthManager.Hit -= Buff1;
				On.HealthManager.ApplyExtraDamage -= Buff2;
			}
			yield break;
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

			nartFSM.RemoveAction("Regain Control", 0);
			StopAllCoroutines();

			if(buffActive > 0)
			{
				On.HealthManager.Hit -= Buff1;
				On.HealthManager.ApplyExtraDamage -= Buff2;
			}
		}
	}
}
