using System;
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
		public override string Levels() => "+10%, +20%, +30% speed";
		public override string TraitName() => "Windswept";
		public override string TraitDescription() => "Striking an enemy increases movement speed for 5 seconds";

		private float RUN_SPEED;
		private float RUN_SPEED_CH;
		private float RUN_SPEED_CH_COMBO;
		private float WALK_SPEED;
		private float UNDERWATER_SPEED;

		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			RUN_SPEED = HeroController.instance.RUN_SPEED;
			RUN_SPEED_CH = HeroController.instance.RUN_SPEED_CH;
			RUN_SPEED_CH_COMBO = HeroController.instance.RUN_SPEED_CH_COMBO;
			WALK_SPEED = HeroController.instance.WALK_SPEED;
			UNDERWATER_SPEED = HeroController.instance.UNDERWATER_SPEED;
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
			HeroController.instance.RUN_SPEED = RUN_SPEED * multiplier;
			HeroController.instance.RUN_SPEED_CH = RUN_SPEED_CH * multiplier;
			HeroController.instance.RUN_SPEED_CH_COMBO = RUN_SPEED_CH_COMBO * multiplier;
			HeroController.instance.WALK_SPEED = WALK_SPEED * multiplier;
			HeroController.instance.UNDERWATER_SPEED = UNDERWATER_SPEED * multiplier;
			yield return new WaitForSeconds(5f);
			HeroController.instance.RUN_SPEED = RUN_SPEED;
			HeroController.instance.RUN_SPEED_CH = RUN_SPEED_CH;
			HeroController.instance.RUN_SPEED_CH_COMBO = RUN_SPEED_CH_COMBO;
			HeroController.instance.WALK_SPEED = WALK_SPEED;
			HeroController.instance.UNDERWATER_SPEED = UNDERWATER_SPEED;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();

			HeroController.instance.RUN_SPEED = RUN_SPEED;
			HeroController.instance.RUN_SPEED_CH = RUN_SPEED_CH;
			HeroController.instance.RUN_SPEED_CH_COMBO = RUN_SPEED_CH_COMBO;
			HeroController.instance.WALK_SPEED = WALK_SPEED;
			HeroController.instance.UNDERWATER_SPEED = UNDERWATER_SPEED;
		}
	}
}