using ItemChanger;
using ItemChanger.Locations;
using Modding;
using System.Collections;
using UnityEngine;
using Satchel;
using HutongGames.PlayMaker.Actions;

namespace LostArtifacts.Artifacts
{
	public class CryingStatue : Artifact
	{
		public override int ID() => 4;
		public override string Name() => "Crying Statue";
		public override string LoreDescription() => "The never-ending rain seeps into every object it can find, including this relic. " +
			"The water looks vaguely like tears.";
		public override string LevelInfo() => string.Format("+{0}% damage per mask of damage taken", 5 * (level + 1));
		public override string TraitName() => "Fallen";
		public override string TraitDescription() => "Take one extra damage, but deal more damage for 10 seconds after taking damage.";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Ruins1_27),
				x = 51.5f,
				y = 23.4f,
				elevation = 0f
			};
		}

		private AudioClip furyClip;
		private GameObject furyGO;
		private float multiplier;
		private int damageTaken;

		public override void Activate()
		{
			base.Activate();

			PlayMakerFSM furyFSM = HeroController.instance.transform.Find("Charm Effects").gameObject.LocateMyFSM("Fury");
			furyClip = (AudioClip)furyFSM.GetAction<AudioPlayerOneShotSingle>("Activate", 0).audioClip.Value;
			furyGO = Instantiate(HeroController.instance.transform.Find("Charm Effects/Fury").gameObject);
			furyGO.transform.parent = HeroController.instance.transform.Find("Charm Effects");
			furyGO.transform.localPosition = new Vector3(-0.014f, - 0.713f, 0f);
			DontDestroyOnLoad(furyGO);
			ParticleSystem.MainModule settings = furyGO.GetComponent<ParticleSystem>().main;
			settings.startColor = new Color(0f, 0.25f, 0.6f);

			multiplier = 0.05f * (level + 1);
			damageTaken = 0;

			ModHooks.TakeHealthHook += TakeHealthHook;
			ModHooks.TakeDamageHook += TakeDamageHook;
			On.HealthManager.Hit += HealthManagerHit;
		}

		private int TakeHealthHook(int damage)
		{
			StartCoroutine(DamageControl(damage));
			return damage;
		}

		private int TakeDamageHook(ref int hazardType, int damage)
		{
			return damage == 0 ? 0 : damage + 1;
		}

		private IEnumerator DamageControl(int damage)
		{
			damageTaken += damage;
			HeroController.instance.GetComponent<AudioSource>().PlayOneShot(furyClip);
			furyGO.GetComponent<ParticleSystem>().Play();

			yield return new WaitForSeconds(10f);

			damageTaken -= damage;
			if(damageTaken == 0) furyGO.GetComponent<ParticleSystem>().Stop();
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				hitInstance.Multiplier += multiplier * damageTaken;
			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.TakeHealthHook -= TakeHealthHook;
			ModHooks.TakeDamageHook -= TakeDamageHook;
			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();

			furyGO.GetComponent<ParticleSystem>().Stop();
		}
	}
}
