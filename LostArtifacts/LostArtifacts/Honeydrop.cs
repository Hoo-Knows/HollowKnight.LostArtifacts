using ItemChanger;
using ItemChanger.Locations;
using Modding;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class Honeydrop : Artifact
	{
		public override int ID() => 15;
		public override string Name() => "Honeydrop";
		public override string Description() => "This honeydrop was made through the bees’ hard work. And you took it without " +
			"permission. Unbeelievable.";
		public override string LevelInfo() => (40 * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1)) + ", " +
			(30 * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1)) + ", " +
			(20 * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1)) + " damage";
		public override string TraitName() => "Honey Coating";
		public override string TraitDescription() => "Dealing enough damage gives a honey coating that blocks one instance " +
			"of non-hazard damage (cannot stack)";
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

		private bool coated;
		private int damageDealt;
		private int damageNeeded;

		public override void Activate()
		{
			base.Activate();

			coated = false;
			damageDealt = 0;

			if(level == 1) damageNeeded = 50 * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1);
			if(level == 2) damageNeeded = 40 * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1);
			if(level == 3) damageNeeded = 30 * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1);
			if(level == 4) damageNeeded = 20 * (PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) - 1);

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
						damageDealt = 0;
					}
				}
			}
			orig(self, hitInstance);
		}

		private int AfterTakeDamageHook(int hazardType, int damageAmount)
		{
			if(!coated || hazardType != 1) return damageAmount;
			HeroController.instance.gameObject.GetComponent<AudioSource>().PlayOneShot(HeroController.instance.blockerImpact, 2f);
			coated = false;
			return 0;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
			ModHooks.AfterTakeDamageHook -= AfterTakeDamageHook;
		}
	}
}
