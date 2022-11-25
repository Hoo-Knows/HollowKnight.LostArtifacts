using ItemChanger;
using ItemChanger.Locations;
using Modding;

namespace LostArtifacts.Artifacts
{
	public class InfectedRock : Artifact
	{
		public override int ID() => 16;
		public override string Name() => "Infected Rock";
		public override string LoreDescription() => "The Broken Vessel had to make friends where it could.";
		public override string LevelInfo() => 3 * level + 1 + " extra SOUL per hit";
		public override string TraitName() => "Forgotten";
		public override string TraitDescription() => "Increases SOUL gain from striking enemies with the nail.";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Abyss_19),
				Test = new PDBool(nameof(PlayerData.killedInfectedKnight)),
				falseLocation = new EnemyLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Abyss_19),
					objectName = "Infected Knight",
					removeGeo = false
				},
				trueLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Abyss_19),
					x = 26.8f,
					y = 28.4f,
					elevation = 0f
				}
			};
		}

		public override void Activate()
		{
			base.Activate();

			ModHooks.SoulGainHook += SoulGainHook;
		}

		private int SoulGainHook(int soul)
		{
			return soul + 3 * level + 1;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.SoulGainHook -= SoulGainHook;
		}
	}
}
