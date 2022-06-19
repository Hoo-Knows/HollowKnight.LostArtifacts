using Modding;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class PavingStone : Artifact
	{
		public override int ID() => 1;
		public override string Name() => "Paving Stone";
		public override string Description() => "A stone from the interconnected crossroads beneath Dirtmouth. Through the Stags' " +
			"repeated use, it is imbued with a trace amount of their power.";
		public override string Levels() => "-5%, -10%, -15% attack cooldown";
		public override string TraitName() => "Stagspeed";
		public override string TraitDescription() => "Decreases attack cooldown";

		private float multiplier;
		private float ATTACK_COOLDOWN_TIME;
		private float ATTACK_COOLDOWN_TIME_CH;

		public override void Activate()
		{
			base.Activate();

			ATTACK_COOLDOWN_TIME = HeroController.instance.ATTACK_COOLDOWN_TIME;
			ATTACK_COOLDOWN_TIME_CH = HeroController.instance.ATTACK_COOLDOWN_TIME_CH;

			if(level == 1) multiplier = 0.95f;
			if(level == 2) multiplier = 0.9f;
			if(level == 3) multiplier = 0.85f;

			HeroController.instance.ATTACK_COOLDOWN_TIME *= multiplier;
			HeroController.instance.ATTACK_COOLDOWN_TIME_CH *= multiplier;

			ModHooks.AttackHook += AttackHook;
		}

		private void AttackHook(GlobalEnums.AttackDirection obj)
		{
			StopAllCoroutines();
			StartCoroutine(CooldownControl());
		}

		private IEnumerator CooldownControl()
		{
			if(!PlayerData.instance.GetBool("equippedCharm_32"))
			{
				yield return new WaitForSeconds(HeroController.instance.ATTACK_COOLDOWN_TIME);
			}
			else
			{
				yield return new WaitForSeconds(HeroController.instance.ATTACK_COOLDOWN_TIME_CH);
			}
			HeroController.instance.cState.attacking = false;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			ModHooks.AttackHook -= AttackHook;
			StopAllCoroutines();

			HeroController.instance.ATTACK_COOLDOWN_TIME = ATTACK_COOLDOWN_TIME;
			HeroController.instance.ATTACK_COOLDOWN_TIME_CH = ATTACK_COOLDOWN_TIME_CH;
		}
	}
}
