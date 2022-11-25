using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class Tumbleweed : Artifact
	{
		public override int ID() => 7;
		public override string Name() => "Tumbleweed";
		public override string LoreDescription() => "These little weeds have been tumbling about the Howling Cliffs for as " +
			"long as anyone can remember. Anything imbued with its power will become as swift as the wind itself.";
		public override string LevelInfo() => string.Format("+{0}% movement speed after striking an enemy", 15 * level);
		public override string TraitName() => "Windswept";
		public override string TraitDescription() => "Striking an enemy increases movement speed for 5 seconds.";
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
		private int buffActive;

		public override void Activate()
		{
			base.Activate();

			multiplier = 0.15f * level + 1f;
			buffActive = 0;

			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
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
			buffActive++;
			if(buffActive == 1)
			{
				HeroController.instance.RUN_SPEED *= multiplier;
				HeroController.instance.RUN_SPEED_CH *= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO *= multiplier;
				HeroController.instance.WALK_SPEED *= multiplier;
				HeroController.instance.UNDERWATER_SPEED *= multiplier;
			}
			yield return new WaitForSeconds(10f);
			buffActive--;
			if(buffActive == 0)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
			StopAllCoroutines();

			if(buffActive > 0)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;

				buffActive = 0;
			}
		}
	}
}