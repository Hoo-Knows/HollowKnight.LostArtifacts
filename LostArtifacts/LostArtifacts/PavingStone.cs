using ItemChanger;
using ItemChanger.Locations;
using Modding;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class PavingStone : Artifact
	{
		public override int ID() => 1;
		public override string Name() => "Paving Stone";
		public override string Description() => "A stone from the interconnected crossroads beneath Dirtmouth. Through the Stags' " +
			"repeated use, it is imbued with a trace amount of their power.";
		public override string LevelInfo() => "-10%, -15%, -20% decrease";
		public override string TraitName() => "Stagspeed";
		public override string TraitDescription() => "Decreases attack cooldown and nail art charge time";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Crossroads_47),
				x = 14.2f,
				y = 3.4f,
				elevation = 0f
			};
		}

		private float multiplier;
		private float ATTACK_COOLDOWN_TIME;
		private float ATTACK_COOLDOWN_TIME_CH;
		private float NAIL_CHARGE_TIME_CHARM;
		private float NAIL_CHARGE_TIME_DEFAULT;

		public override void Activate()
		{
			base.Activate();

			ATTACK_COOLDOWN_TIME = HeroController.instance.ATTACK_COOLDOWN_TIME;
			ATTACK_COOLDOWN_TIME_CH = HeroController.instance.ATTACK_COOLDOWN_TIME_CH;
			NAIL_CHARGE_TIME_CHARM = HeroController.instance.NAIL_CHARGE_TIME_CHARM;
			NAIL_CHARGE_TIME_DEFAULT = HeroController.instance.NAIL_CHARGE_TIME_DEFAULT;

			if(level == 1) multiplier = 0.9f;
			if(level == 2) multiplier = 0.85f;
			if(level == 3) multiplier = 0.8f;

			HeroController.instance.ATTACK_COOLDOWN_TIME *= multiplier;
			HeroController.instance.ATTACK_COOLDOWN_TIME_CH *= multiplier;
			HeroController.instance.NAIL_CHARGE_TIME_CHARM *= multiplier;
			HeroController.instance.NAIL_CHARGE_TIME_DEFAULT *= multiplier;

			ModHooks.AttackHook += AttackHook;
		}

		private void AttackHook(GlobalEnums.AttackDirection obj)
		{
			StopAllCoroutines();
			StartCoroutine(CooldownControl());
		}

		private IEnumerator CooldownControl()
		{
			if(!PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_32)))
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
			HeroController.instance.NAIL_CHARGE_TIME_CHARM = NAIL_CHARGE_TIME_CHARM;
			HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = NAIL_CHARGE_TIME_DEFAULT;
		}
	}
}
