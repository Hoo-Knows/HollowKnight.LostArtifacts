using ItemChanger;
using ItemChanger.Locations;
using Satchel;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using System.Collections;

namespace LostArtifacts.Artifacts
{
	public class ChargedCrystal : Artifact
	{
		public override int ID() => 8;
		public override string Name() => "Charged Crystal";
		public override string LoreDescription() => "Though all crystals from the Peaks hold some amount of energy, this crystal is " +
			"even more potent than usual. It pulses and glows with all of its might.";
		public override string LevelInfo() => string.Format("+{0}% nail art damage", 10 * level);
		public override string TraitName() => "Energized";
		public override string TraitDescription() => "Nail arts deal increased damage.";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Mines_18),
				Test = new PDBool(nameof(PlayerData.defeatedMegaBeamMiner)),
				falseLocation = new EnemyFsmLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Mines_18),
					enemyObj = "Mega Zombie Beam Miner (1)",
					enemyFsm = "Beam Miner",
					removeGeo = false
				},
				trueLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Mines_18),
					x = 26f,
					y = 11.4f,
					elevation = 0f
				}
			};
		}

		private GameObject crystalParticlesGO;
		private AudioClip crystalClip;
		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			PlayMakerFSM crystalFSM = LostArtifacts.Preloads["Mines_03"]["Crystal Crawler"].LocateMyFSM("Hit Crystals");
			crystalParticlesGO = crystalFSM.GetAction<SetParticleEmission>("Particle effect", 2).gameObject.GameObject.Value;
			crystalClip = crystalFSM.GetAction<AudioPlaySimple>("Particle effect", 4).oneShotClip.Value as AudioClip;

			multiplier = level * 0.1f;

			On.HealthManager.Hit += HealthManagerHit;
			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail)
			{
				if(hitInstance.Source.name.Contains("Great Slash") ||
					hitInstance.Source.name.Contains("Dash Slash") ||
					hitInstance.Source.name.Contains("Hit L") ||
					hitInstance.Source.name.Contains("Hit R"))
				{
					hitInstance.Multiplier += multiplier;
				}
			}
			orig(self, hitInstance);
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail)
			{
				if(hitInstance.Source.name.Contains("Great Slash") ||
					hitInstance.Source.name.Contains("Dash Slash") ||
					hitInstance.Source.name.Contains("Hit L") ||
					hitInstance.Source.name.Contains("Hit R"))
				{
					StartCoroutine(EmitParticles(self.transform.position));
					HeroController.instance.GetComponent<AudioSource>().PlayOneShot(crystalClip);
				}
			}
			orig(self, hitInstance);
		}

		private IEnumerator EmitParticles(Vector3 pos)
		{
			GameObject crystalParticles = Instantiate(crystalParticlesGO, pos, Quaternion.identity);
			crystalParticles.SetActive(true);

			ParticleSystem particles = crystalParticles.GetComponent<ParticleSystem>();
			particles.Emit(10);

			yield return new WaitForSeconds(2.5f);
			Destroy(crystalParticles);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
		}
	}
}