using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class HiddenMemento : Artifact
	{
		public override int ID() => 20;
		public override string Name() => "Hidden Memento";
		public override string Description() => "In memory of Schy.";
		public override string LevelInfo() => "+10%, +20%, +30% speed";
		public override string TraitName() => "Pogomaster";
		public override string TraitDescription() => "Pogoing grants increased movement speed for 5 seconds";
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
		private bool buffActive;

		public override void Activate()
		{
			base.Activate();

			multiplier = 0.1f * level + 1f;

			On.HeroController.Bounce += HeroControllerBounce;
			On.HeroController.BounceHigh += HeroControllerBounceHigh;
			On.HeroController.ShroomBounce += HeroControllerShroomBounce;
		}

		private void HeroControllerBounce(On.HeroController.orig_Bounce orig, HeroController self)
		{
			orig(self);

			StopAllCoroutines();
			StartCoroutine(SpeedControl());
		}

		private void HeroControllerBounceHigh(On.HeroController.orig_BounceHigh orig, HeroController self)
		{
			orig(self);

			StopAllCoroutines();
			StartCoroutine(SpeedControl());
		}

		private void HeroControllerShroomBounce(On.HeroController.orig_ShroomBounce orig, HeroController self)
		{
			orig(self);

			StopAllCoroutines();
			StartCoroutine(SpeedControl());
		}

		private IEnumerator SpeedControl()
		{
			if(!buffActive)
			{
				HeroController.instance.RUN_SPEED *= multiplier;
				HeroController.instance.RUN_SPEED_CH *= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO *= multiplier;
				HeroController.instance.WALK_SPEED *= multiplier;
				HeroController.instance.UNDERWATER_SPEED *= multiplier;
				buffActive = true;
			}
			yield return new WaitForSeconds(10f);
			if(buffActive)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;
				buffActive = false;
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HeroController.BounceHigh -= HeroControllerBounceHigh;
			On.HeroController.ShroomBounce -= HeroControllerShroomBounce;

			StopAllCoroutines();

			if(buffActive)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;
				buffActive = false;
			}
		}
	}
}
