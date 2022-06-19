using System;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using Satchel;

namespace LostArtifacts
{
	public class WyrmAsh : Artifact
	{
		public override int ID() => 13;
		public override string Name() => "Wyrm Ash";
		public override string Description() => "So great was the Wyrm's power, that even a small amount of its corpse's ashes " +
			"carry noticeable power. Imbuing the nail with these ashes allows the wielder to harness the Wyrm's control over life.";
		public override string Levels() => "9, 6, 3 hits per minion";
		public override string TraitName() => "Rebirth";
		public override string TraitDescription() => "Spawn a minion after a certain amount of hits";

		private int counter;
		private int hitsNeeded;
		private GameObject hatchlingGO;

		public override void Activate()
		{
			base.Activate();

			counter = 0;
			hitsNeeded = 12 - 3 * level;
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
