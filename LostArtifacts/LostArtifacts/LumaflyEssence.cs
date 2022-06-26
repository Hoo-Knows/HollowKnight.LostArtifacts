using GlobalEnums;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using Modding;
using Satchel;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class LumaflyEssence : Artifact
	{
		public override int ID() => 10;
		public override string Name() => "Lumafly Essence";
		public override string Description() => "Monomon the Teacher studied the Charged Lumaflies closely and extracted their " +
			"essence to give Uumuu electrifying powers. Applying the essence to the nail may produce a similar effect.";
		public override string LevelInfo() => "2, 3, 4 bursts";
		public override string TraitName() => "Shocking";
		public override string TraitDescription() => "Swinging the nail creates a chain of electric bursts (treated as nail damage)";
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

		public override void Activate()
		{
			base.Activate();

			zapGO = LostArtifacts.Preloads["GG_Uumuu"]["Mega Jellyfish GG"].LocateMyFSM("Mega Jellyfish").
				GetAction<SpawnObjectFromGlobalPool>("Gen", 2).gameObject.Value;

			ModHooks.AttackHook += AttackHook;
			On.DamageEnemies.DoDamage += DamageEnemiesDoDamage;
			On.DamageEnemies.FixedUpdate += DamageEnemiesFixedUpdate;
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
			for(int i = 1; i < level + 2; i++)
			{
				GameObject zap = Instantiate(zapGO, pos + i * dir, Quaternion.identity);
				zap.SetActive(false);

				zap.name = "LostArtifacts.LumaflyEssenceZap";
				zap.layer = (int)PhysLayers.HERO_ATTACK;
				Destroy(zap.GetComponent<DamageHero>());

				DamageEnemies de = zap.AddComponent<DamageEnemies>();
				de.damageDealt = PlayerData.instance.GetInt(nameof(PlayerData.nailDamage));
				de.attackType = AttackTypes.Nail;
				de.ignoreInvuln = false;

				zap.SetActive(true);

				yield return new WaitForSeconds(0.1f);
			}
			yield break;
		}

		private void DamageEnemiesDoDamage(On.DamageEnemies.orig_DoDamage orig, DamageEnemies self, GameObject target)
		{
			orig(self, target);
			if(self.gameObject.name == "LostArtifacts.LumaflyEssenceZap")
			{
				self.gameObject.GetComponent<CircleCollider2D>().enabled = false;
			}
		}

		private void DamageEnemiesFixedUpdate(On.DamageEnemies.orig_FixedUpdate orig, DamageEnemies self)
		{
			try
			{
				orig(self);
			}
			catch
			{
				//Out of sight, out of mind
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.AttackHook -= AttackHook;
			On.DamageEnemies.DoDamage -= DamageEnemiesDoDamage;
			On.DamageEnemies.FixedUpdate -= DamageEnemiesFixedUpdate;
		}
	}
}
