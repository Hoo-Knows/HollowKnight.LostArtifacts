using System;
using System.Collections.Generic;
using UnityEngine;

namespace LostArtifacts
{
	public class DungBall : Artifact
	{
		public override int ID() => 6;
		public override string Name() => "Dung Ball";
		public override string Description() => "A questionable gift from the Dung Defender.";
		public override string Levels() => "Stinky, Stinkier, Stinkiest";
		public override string TraitName() => "Stinky";
		public override string TraitDescription() => "Odorous (also lag generator)";

		private GameObject stinkGO;

		public override void Activate()
		{
			base.Activate();

			foreach(var pool in ObjectPool.instance.startupPools)
			{
				if(pool.prefab.name == "Knight Dung Trail")
				{
					stinkGO = pool.prefab;
					break;
				}
			}

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				int numToSpawn = 1;
				if(level == 2) numToSpawn = 10;
				if(level == 3) numToSpawn = 75;

				for(int i = 0; i < numToSpawn; i++)
				{
					GameObject stink = Instantiate(stinkGO, self.transform.position, Quaternion.identity);

					Destroy(stink.GetComponent<DamageEnemies>());
					Destroy(stink.GetComponent<DamageEffectTicker>());

					if(level == 2) stink.transform.localScale *= 5f;
					if(level == 3) stink.transform.localScale *= 25f;

					stink.SetActive(true);
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
