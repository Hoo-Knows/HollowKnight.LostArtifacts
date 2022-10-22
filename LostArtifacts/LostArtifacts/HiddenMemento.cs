using ItemChanger;
using ItemChanger.Locations;
using Modding;

namespace LostArtifacts.Artifacts
{
	public class HiddenMemento : Artifact
	{
		public override int ID() => 20;
		public override string Name() => "Hidden Memento";
		public override string Description() => "In memory of Schy.";
		public override string LevelInfo() => "+10%, +20%, +30% damage after pogoing";
		public override string TraitName() => "Pogomaster";
		public override string TraitDescription() => "Pogoing makes the next attack deal increased damage";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.White_Palace_06),
				Test = new PDBool(nameof(PlayerData.killedBindingSeal)),
				falseLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.White_Palace_06),
					x = 22.8f,
					y = -100f,
					elevation = 0f
				},
				trueLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.White_Palace_06),
					x = 22.8f,
					y = 3.4f,
					elevation = 0f
				}
			};
		}

		private float multiplier;
		private bool bouncing;
		private bool buffActive;

		public override void Activate()
		{
			base.Activate();

			multiplier = 0.1f * level;
			bouncing = false;
			buffActive = false;

			On.HealthManager.Hit += HealthManagerHit;
			ModHooks.AttackHook += AttackHook;
			On.HeroController.Bounce += HeroControllerBounce;
			On.HeroController.BounceHigh += HeroControllerBounceHigh;
			On.HeroController.ShroomBounce += HeroControllerShroomBounce;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(buffActive)
			{
				hitInstance.Multiplier += multiplier;
				buffActive = false;
			}
			orig(self, hitInstance);
		}

		private void AttackHook(GlobalEnums.AttackDirection obj)
		{
			if(bouncing) buffActive = true;
			bouncing = false;
		}

		private void HeroControllerBounce(On.HeroController.orig_Bounce orig, HeroController self)
		{
			orig(self);
			bouncing = true;
		}

		private void HeroControllerBounceHigh(On.HeroController.orig_BounceHigh orig, HeroController self)
		{
			orig(self);
			bouncing = true;
		}

		private void HeroControllerShroomBounce(On.HeroController.orig_ShroomBounce orig, HeroController self)
		{
			orig(self);
			bouncing = true;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			ModHooks.AttackHook -= AttackHook;
			On.HeroController.Bounce -= HeroControllerBounce;
			On.HeroController.BounceHigh -= HeroControllerBounceHigh;
			On.HeroController.ShroomBounce -= HeroControllerShroomBounce;

			bouncing = false;
			buffActive = false;
		}
	}
}
