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
		public override string LevelInfo() => "Maximum of 1, 2, 3 bursts";
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

			GameObject prefab = LostArtifacts.Preloads["GG_Uumuu"]["Mega Jellyfish GG"].LocateMyFSM("Mega Jellyfish").
				GetAction<SpawnObjectFromGlobalPool>("Gen", 2).gameObject.Value;
			zapGO = Instantiate(prefab);
			zapGO.SetActive(false);
			DontDestroyOnLoad(zapGO);

			zapGO.name = "LostArtifacts.LumaflyEssenceZap";
			zapGO.layer = (int)PhysLayers.HERO_ATTACK;
			Destroy(zapGO.GetComponent<DamageHero>());
			LumaflyEssenceZap de = zapGO.AddComponent<LumaflyEssenceZap>();

			numToSpawn = level;

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
					float d = 1f;
					if(!HeroController.instance.cState.facingRight) d *= -1f;
					if(HeroController.instance.cState.wallSliding) d *= -1f;
					dir = new Vector3(d, 0f);
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
				LostArtifacts.Instance.Log(hitInstance.Source.layer);
				orig(self, hitInstance);
				return;
			}

			//Override the iframes on hit so that it doesn't eat nail hits
			orig(self, hitInstance);
			ReflectionHelper.SetField(self, "evasionByHitRemaining", 0f);
		}

		private void Update()
		{
			if(active && HeroController.instance.CheckTouchingGround()) numToSpawn = level;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.AttackHook -= AttackHook;
			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();
		}
	}

	internal class LumaflyEssenceZap : MonoBehaviour
	{

		private void OnTriggerEnter2D(Collider2D otherCollider)
		{
			if(otherCollider == null) return;
			if(otherCollider.gameObject.layer != (int)PhysLayers.ENEMIES) return;

			HitInstance hit = default(HitInstance);
			hit.DamageDealt = PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) / 2;
			hit.AttackType = AttackTypes.NailBeam;
			hit.IgnoreInvulnerable = true;
			hit.Source = gameObject;
			hit.Multiplier = 1f;

			HealthManager hm = otherCollider.gameObject.GetComponent<HealthManager>();
			hm.Hit(hit);
		}
	}
}
