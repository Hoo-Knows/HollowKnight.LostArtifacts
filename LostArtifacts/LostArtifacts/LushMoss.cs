using ItemChanger;
using ItemChanger.Locations;

namespace LostArtifacts.Artifacts
{
	public class LushMoss : Artifact
	{
		public override int ID() => 2;
		public override string Name() => "Lush Moss";
		public override string Description() => "This piece of moss came from the Massive Moss Charger. It has a mysterious " +
			"healing power that allows the moss chargers to recover their moss no matter how many times it is destroyed.";
		public override string LevelInfo() => "30, 20, 10 hits to heal";
		public override string TraitName() => "Regeneration";
		public override string TraitDescription() => "Heal a mask after a certain amount of hits";
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

		private int counter;
		private int hitsNeeded;

		public override void Activate()
		{
			base.Activate();

			counter = 0;
			hitsNeeded = 40 - level * 10;

			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				counter++;
				if(counter >= hitsNeeded)
				{
					HeroController.instance.AddHealth(1);
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
