using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using Satchel;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class WyrmAsh : Artifact
	{
		public override int ID() => 13;
		public override string Name() => "Wyrm Ash";
		public override string Description() => "So great was the Wyrm's power, that even a small amount of its corpse's ashes " +
			"carry noticeable power. Imbuing the nail with these ashes allows the wielder to harness the Wyrm's control over life.";
		public override string LevelInfo() => "12, 8, 4 hits per minion";
		public override string TraitName() => "Rebirth";
		public override string TraitDescription() => "Spawn a hatchling after a certain amount of hits";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Deepnest_East_12),
				Test = new PDBool(nameof(PlayerData.hasKingsBrand)),
				falseLocation = new EnemyFsmLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Deepnest_East_Hornet),
					enemyObj = "Hornet Boss 2",
					enemyFsm = "Control",
					removeGeo = false
				},
				trueLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Deepnest_East_12),
					x = 101.8f,
					y = 11.4f,
					elevation = 0f
				}
			};
		}

		private int counter;
		private int hitsNeeded;
		private GameObject hatchlingGO;

		public override void Activate()
		{
			base.Activate();

			counter = 0;
			hitsNeeded = 16 - 4 * level;
			hatchlingGO = HeroController.instance.transform.Find("Charm Effects").gameObject.LocateMyFSM("Hatchling Spawn").
				GetAction<SpawnObjectFromGlobalPool>("Hatch", 2).gameObject.Value;

			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				counter++;
				if(counter >= hitsNeeded)
				{
					Instantiate(hatchlingGO, HeroController.instance.transform.position, Quaternion.identity);
					counter = 0;
				}
			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
		}
	}
}
