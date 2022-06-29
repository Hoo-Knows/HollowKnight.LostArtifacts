using ItemChanger;
using ItemChanger.Locations;

namespace LostArtifacts
{
	public class ChargedCrystal : Artifact
	{
		public override int ID() => 8;
		public override string Name() => "Charged Crystal";
		public override string Description() => "Though all crystals from the Peaks hold some amount of energy, this crystal is " +
			"even more potent than usual. It pulses and glows with all of its might.";
		public override string LevelInfo() => "+20%, +30%, +40% damage";
		public override string TraitName() => "Energized";
		public override string TraitDescription() => "Nail arts deal increased damage";
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

		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			multiplier = 0.1f + level * 0.1f;

			On.HealthManager.Hit += HealthManagerHit;
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

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}