using System;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class Tumbleweed : Artifact
	{
		private float runSpeed;
		private float runSpeedCh;
		private float runSpeedChCombo;
		private float walkSpeed;
		private float underwaterSpeed;

		private float multiplier;

		public override void Initialize()
		{
			id = 7;
			unlocked = true;
			name = "Tumbleweed";
			description = "These little weeds have been tumbling about the Howling Cliffs for as long as anyone can remember. Anything that touches one will become as swift as the wind itself.";
			traitName = "Windswept";
			traitDescription = "Hitting an enemy increases movement speed for 5 seconds";
			active = false;
			level = 0;
		}

		public override void Activate()
		{
			LostArtifacts.Instance.Log("Activating " + traitName);

			runSpeed = HeroController.instance.RUN_SPEED;
			runSpeedCh = HeroController.instance.RUN_SPEED_CH;
			runSpeedChCombo = HeroController.instance.RUN_SPEED_CH_COMBO;
			walkSpeed = HeroController.instance.WALK_SPEED;
			underwaterSpeed = HeroController.instance.UNDERWATER_SPEED;
			multiplier = 0.1f * level + 1;

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail)
			{
				StopAllCoroutines();
				StartCoroutine(SpeedControl());
			}
			orig(self, hitInstance);
		}

		private IEnumerator SpeedControl()
		{
			HeroController.instance.RUN_SPEED = runSpeed * multiplier;
			HeroController.instance.RUN_SPEED_CH = runSpeedCh * multiplier;
			HeroController.instance.RUN_SPEED_CH_COMBO = runSpeedChCombo * multiplier;
			HeroController.instance.WALK_SPEED = walkSpeed * multiplier;
			HeroController.instance.UNDERWATER_SPEED = underwaterSpeed * multiplier;
			yield return new WaitForSeconds(5f);
			HeroController.instance.RUN_SPEED = runSpeed;
			HeroController.instance.RUN_SPEED_CH = runSpeedCh;
			HeroController.instance.RUN_SPEED_CH_COMBO = runSpeedChCombo;
			HeroController.instance.WALK_SPEED = walkSpeed;
			HeroController.instance.UNDERWATER_SPEED = underwaterSpeed;
		}

		public override void Deactivate()
		{
			LostArtifacts.Instance.Log("Deactivating " + traitName);
			On.HealthManager.Hit -= HealthManagerHit;
			HeroController.instance.RUN_SPEED = runSpeed;
			HeroController.instance.RUN_SPEED_CH = runSpeedCh;
			HeroController.instance.RUN_SPEED_CH_COMBO = runSpeedChCombo;
			HeroController.instance.WALK_SPEED = walkSpeed;
			HeroController.instance.UNDERWATER_SPEED = underwaterSpeed;
		}
	}
}