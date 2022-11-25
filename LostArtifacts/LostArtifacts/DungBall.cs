using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class DungBall : Artifact
	{
		public override int ID() => 6;
		public override string Name() => "Dung Ball";
		public override string LoreDescription() => "A questionable gift from the Dung Defender.";
		public override string LevelInfo() => GetLevelInfo();
		public override string TraitName() => "Stinky";
		public override string TraitDescription() => "Makes you odorous and generates lag.";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Waterways_15),
				x = 18f,
				y = 4.4f,
				elevation = 0f
			};
		}

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

			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				StartCoroutine(SpawnClouds(self.transform.position));
			}
			orig(self, hitInstance);
		}

		private IEnumerator SpawnClouds(Vector3 pos)
		{
			int numToSpawn = 1;
			if(level == 2) numToSpawn = 10;
			if(level == 3) numToSpawn = 50;
			if(level == 4) numToSpawn = 500;

			for(int i = 0; i < numToSpawn; i++)
			{
				GameObject stink = Instantiate(stinkGO, pos, Quaternion.identity);

				Destroy(stink.GetComponent<DamageEnemies>());
				Destroy(stink.GetComponent<DamageEffectTicker>());

				if(level == 2) stink.transform.localScale *= 5f;
				if(level == 3) stink.transform.localScale *= 10f;
				if(level == 4) stink.transform.localScale *= 30f;

				stink.SetActive(true);

				if(i % 10 == 0) yield return new WaitForEndOfFrame();
			}
		}

		private string GetLevelInfo()
		{
			if(level == 1) return "Stink";
			if(level == 2) return "Stinkier";
			if(level == 3) return "Stinkiest";
			if(level == 4) return "Stinkiestest";
			return "";
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
		}
	}
}
