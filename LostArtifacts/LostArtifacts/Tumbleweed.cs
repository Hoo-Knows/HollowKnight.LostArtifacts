using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class Tumbleweed : Artifact
	{
		public override int ID() => 7;
		public override string Name() => "Tumbleweed";
		public override string Description() => "These little weeds have been tumbling about the Howling Cliffs for as " +
			"long as anyone can remember. Anything imbued with its power will become as swift as the wind itself.";
		public override string LevelInfo() => "+10%, +20%, +30% speed";
		public override string TraitName() => "Windswept";
		public override string TraitDescription() => "Striking an enemy increases movement speed for 5 seconds";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Cliffs_01),
				x = 45.2f,
				y = 109.3f,
				elevation = 0f
			};
		}

		private float multiplier;
		private bool buffActive;

		public override void Activate()
		{
			base.Activate();

			multiplier = 0.1f * level + 1;

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				StopAllCoroutines();
				StartCoroutine(SpeedControl());
			}
			orig(self, hitInstance);
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
			yield return new WaitForSeconds(5f);
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

			On.HealthManager.Hit -= HealthManagerHit;
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