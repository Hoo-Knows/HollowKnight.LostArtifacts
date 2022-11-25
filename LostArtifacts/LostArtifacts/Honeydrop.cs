using ItemChanger;
using ItemChanger.Locations;
using Modding;
using System.Collections;
using UnityEngine;
using Satchel;
using HutongGames.PlayMaker.Actions;

namespace LostArtifacts.Artifacts
{
	public class Honeydrop : Artifact
	{
		public override int ID() => 15;
		public override string Name() => "Honeydrop";
		public override string LoreDescription() => "This honeydrop was made through the bees’ hard work. And you took it without " +
			"permission. Unbeelievable.";
		public override string LevelInfo() => 10 * (6 - level) * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1) + " damage " +
			"needed per honey coating";
		public override string TraitName() => "Honey Coating";
		public override string TraitDescription() => "Dealing enough damage gives a honey coating that blocks one instance " +
			"of non-hazard damage (cannot stack).";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Hive_01),
				x = 47.4f,
				y = 8.4f,
				elevation = 0f
			};
		}

		private AudioClip honeyBreak;
		private GameObject honeyGO;

		private bool coated;
		private int damageDealt;
		private int damageNeeded;

		public override void Activate()
		{
			base.Activate();

			PlayMakerFSM honeyBench = LostArtifacts.Preloads["Hive_01"]["Hive Bench"].LocateMyFSM("Control");
			honeyBreak = honeyBench.GetAction<AudioPlaySimple>("Break 2", 0).oneShotClip.Value as AudioClip;
			honeyGO = honeyBench.GetAction<FlingObjectsFromGlobalPool>("Break 2", 8).gameObject.Value;

			coated = false;
			damageDealt = 0;
			damageNeeded = 10 * (6 - level) * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1);
			
			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
			ModHooks.AfterTakeDamageHook += AfterTakeDamageHook;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				if(!coated)
				{
					damageDealt += (int)(hitInstance.DamageDealt * hitInstance.Multiplier);
					if(damageDealt > damageNeeded)
					{
						coated = true;
						StartCoroutine(FlashYellow());
						damageDealt = 0;
					}
				}
			}
			orig(self, hitInstance);
		}

		private int AfterTakeDamageHook(int hazardType, int damageAmount)
		{
			if(!coated || hazardType != 1) return damageAmount;
			HeroController.instance.gameObject.GetComponent<AudioSource>().PlayOneShot(honeyBreak, 1f);
			FlingHoney();
			coated = false;
			return 0;
		}

		private IEnumerator FlashYellow()
		{
			while(coated)
			{
				HeroController.instance.GetComponent<SpriteFlash>().flash(new Color(0.99f, 0.82f, 0.09f), 0.7f, 0.25f, 0.5f, 0.25f);
				yield return new WaitForSeconds(1f);
			}
			yield break;
		}

		private void FlingHoney()
		{
			for(int i = 0; i < 50; i++)
			{
				GameObject honey = Instantiate(honeyGO, HeroController.instance.transform.position, Quaternion.identity);
				honey.transform.position = new Vector3(honey.transform.position.x + Random.Range(-2f, 2f),
					honey.transform.position.y + Random.Range(-2f, 2f),
					honey.transform.position.z);

				float speed = Random.Range(5f, 15f);
				float angle = Random.Range(0f, 360f);
				Vector2 vel = new Vector2(speed * Mathf.Cos(angle * Mathf.Deg2Rad), speed * Mathf.Sin(angle * Mathf.Deg2Rad));
				honey.GetComponent<Rigidbody2D>().velocity = vel;
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
			ModHooks.AfterTakeDamageHook -= AfterTakeDamageHook;

			StopAllCoroutines();
		}
	}
}
