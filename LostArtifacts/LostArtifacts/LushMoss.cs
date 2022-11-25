using ItemChanger;
using ItemChanger.Locations;
using UnityEngine;
using Satchel;
using HutongGames.PlayMaker.Actions;

namespace LostArtifacts.Artifacts
{
	public class LushMoss : Artifact
	{
		public override int ID() => 2;
		public override string Name() => "Lush Moss";
		public override string LoreDescription() => "This piece of moss came from the Massive Moss Charger. It has a mysterious " +
			"healing power that allows the moss chargers to recover their moss no matter how many times it is destroyed.";
		public override string LevelInfo() => 30 - level * 5 + " hits to heal";
		public override string TraitName() => "Regeneration";
		public override string TraitDescription() => "Heal a mask after a certain amount of nail hits.";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Fungus1_29),
				Test = new PDBool(nameof(PlayerData.megaMossChargerDefeated)),
				falseLocation = new EnemyLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Fungus1_29),
					objectName = "Mega Moss Charger",
					removeGeo = false
				},
				trueLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Fungus1_29),
					x = 50.7f,
					y = 7.4f,
					elevation = 0f
				}
			};
		}

		private GameObject flashGO;
		private AudioClip healClip;
		private int counter;
		private int hitsNeeded;

		public override void Activate()
		{
			base.Activate();

			PlayMakerFSM spellFSM = HeroController.instance.gameObject.LocateMyFSM("Spell Control");
			flashGO = spellFSM.GetAction<SpawnObjectFromGlobalPool>("Focus Heal", 6).gameObject.Value;
			healClip = (AudioClip)spellFSM.GetAction<AudioPlayerOneShotSingle>("Focus Heal", 3).audioClip.Value;
			counter = 0;
			hitsNeeded = 30 - level * 5;

			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				counter++;
				if(counter >= hitsNeeded && 
					PlayerData.instance.GetInt(nameof(PlayerData.health)) < PlayerData.instance.GetInt(nameof(PlayerData.maxHealth)))
				{
					HeroController.instance.AddHealth(1);
					HeroController.instance.GetComponent<SpriteFlash>().flashFocusHeal();
					Instantiate(flashGO, HeroController.instance.transform.position - Vector3.back * 0.21f, Quaternion.identity);
					HeroController.instance.GetComponent<AudioSource>().PlayOneShot(healClip);
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
