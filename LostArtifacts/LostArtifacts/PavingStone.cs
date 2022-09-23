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
		public override string LevelInfo() => "-10%, -20%, -30% decrease";
		public override string TraitName() => "Stagspeed";
		public override string TraitDescription() => "Decreases attack cooldown and nail art charge time for 3 seconds after a dash";
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
		private int buffActive;
		private Coroutine cooldown;

		public override void Activate()
		{
			base.Activate();

			ATTACK_COOLDOWN_TIME = HeroController.instance.ATTACK_COOLDOWN_TIME;
			ATTACK_COOLDOWN_TIME_CH = HeroController.instance.ATTACK_COOLDOWN_TIME_CH;
			NAIL_CHARGE_TIME_CHARM = HeroController.instance.NAIL_CHARGE_TIME_CHARM;
			NAIL_CHARGE_TIME_DEFAULT = HeroController.instance.NAIL_CHARGE_TIME_DEFAULT;

			multiplier = 1f - level * 0.1f;

			On.HeroController.Dash += HeroControllerDash;
			ModHooks.AttackHook += AttackHook;
		}

		private void HeroControllerDash(On.HeroController.orig_Dash orig, HeroController self)
		{
			orig(self);
			StartCoroutine(BuffControl());
		}

		private IEnumerator BuffControl()
		{
			buffActive++;
			if(buffActive == 1)
			{
				HeroController.instance.ATTACK_COOLDOWN_TIME *= multiplier;
				HeroController.instance.ATTACK_COOLDOWN_TIME_CH *= multiplier;
				HeroController.instance.NAIL_CHARGE_TIME_CHARM *= multiplier;
				HeroController.instance.NAIL_CHARGE_TIME_DEFAULT *= multiplier;
			}
			yield return new WaitForSeconds(3f);
			buffActive--;
			if(buffActive == 0)
			{
				HeroController.instance.ATTACK_COOLDOWN_TIME = ATTACK_COOLDOWN_TIME;
				HeroController.instance.ATTACK_COOLDOWN_TIME_CH = ATTACK_COOLDOWN_TIME_CH;
				HeroController.instance.NAIL_CHARGE_TIME_CHARM = NAIL_CHARGE_TIME_CHARM;
				HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = NAIL_CHARGE_TIME_DEFAULT;
			}
			yield break;
		}

		private void AttackHook(GlobalEnums.AttackDirection obj)
		{
			if(cooldown != null) StopCoroutine(cooldown);
			cooldown = StartCoroutine(CooldownControl());
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

			On.HeroController.Dash -= HeroControllerDash;
			ModHooks.AttackHook -= AttackHook;
			StopAllCoroutines();

			HeroController.instance.ATTACK_COOLDOWN_TIME = ATTACK_COOLDOWN_TIME;
			HeroController.instance.ATTACK_COOLDOWN_TIME_CH = ATTACK_COOLDOWN_TIME_CH;
			HeroController.instance.NAIL_CHARGE_TIME_CHARM = NAIL_CHARGE_TIME_CHARM;
			HeroController.instance.NAIL_CHARGE_TIME_DEFAULT = NAIL_CHARGE_TIME_DEFAULT;
		}
	}
}
