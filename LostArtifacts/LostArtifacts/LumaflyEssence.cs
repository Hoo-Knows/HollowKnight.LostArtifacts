using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using Modding;
using Satchel;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class LumaflyEssence : Artifact
	{
		public override int ID() => 10;
		public override string Name() => "Lumafly Essence";
		public override string Description() => "Monomon the Teacher studied the Charged Lumaflies closely and extracted their " +
			"essence to give Uumuu electrifying powers. Applying the essence to the nail may produce a similar effect.";
		public override string LevelInfo() => "Maximum of 2, 3, 4 bursts";
		public override string TraitName() => "Shocking";
		public override string TraitDescription() => "Swinging the nail creates a chain of electric bursts (shorter each swing, " +
			"resets when touching ground)";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Fungus3_archive_02),
				x = 26.5f,
				y = 74.4f,
				elevation = 0f
			};
		}

		private GameObject zapGO;
		private int numToSpawn;

		public override void Activate()
		{
			base.Activate();

			zapGO = LostArtifacts.Preloads["GG_Uumuu"]["Mega Jellyfish GG"].LocateMyFSM("Mega Jellyfish").
				GetAction<SpawnObjectFromGlobalPool>("Gen", 2).gameObject.Value;
			numToSpawn = level + 1;

			ModHooks.AttackHook += AttackHook;
			On.HealthManager.Hit += HealthManagerHit;
		}

		private void AttackHook(AttackDirection obj)
		{
			Vector3 dir = new Vector3();
			switch(obj)
			{
				case AttackDirection.upward:
					dir = new Vector3(0f, 1f);
					break;
				case AttackDirection.downward:
					dir = new Vector3(0f, -1f);
					break;
				case AttackDirection.normal:
					if(HeroController.instance.cState.facingRight) dir = new Vector3(1f, 0f);
					else dir = new Vector3(-1f, 0f);
					break;
			}
			dir *= 4f;
			StartCoroutine(ZapControl(dir));
		}

		private IEnumerator ZapControl(Vector3 dir)
		{
			Vector3 pos = HeroController.instance.transform.position;
			for(int i = 1; i <= numToSpawn; i++)
			{
				GameObject zap = Instantiate(zapGO, pos + i * dir, Quaternion.identity);
				zap.SetActive(false);

				zap.name = "LostArtifacts.LumaflyEssenceZap";
				zap.layer = (int)PhysLayers.HERO_ATTACK;
				Destroy(zap.GetComponent<DamageHero>());

				DamageEnemies de = zap.AddComponent<DamageEnemies>();
				de.damageDealt = PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) / 3;
				de.attackType = AttackTypes.NailBeam;
				de.ignoreInvuln = true;

				zap.SetActive(true);

				yield return new WaitForSeconds(0.1f);
			}
			numToSpawn--;
			yield break;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.Source.name != "LostArtifacts.LumaflyEssenceZap")
			{
				orig(self, hitInstance);
				return;
			}

			//Override the iframes on hit so that it doesn't eat nail hits
			GameObject.Destroy(hitInstance.Source.GetComponent<DamageEnemies>());
			hitInstance.IsExtraDamage = true;
			orig(self, hitInstance);
			ReflectionHelper.SetField(self, "evasionByHitRemaining", 0f);
		}

		private void Update()
		{
			if(active && HeroController.instance.CheckTouchingGround()) numToSpawn = level + 1;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.AttackHook -= AttackHook;
			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();
		}
	}
}
