using System;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class AttunedJewel : Artifact
	{
		public override int ID() => 19;
		public override string Name() => "Attuned Jewel";
		public override string Description() => "The Godseekers crafted this jewel through harnessing the power of the Gods of " +
			"Thunder and Rain. It reveals the bearer’s inner nature and allows them to ascend ever higher.";
		public override string Levels() => "+10%, +20%, +30% to each stat";
		public override string TraitName() => "Attuned";
		public override string TraitDescription() => "Casting Fireball, Dive, or Shriek grants bonus range, movement speed, or " +
			"damage for 5 seconds, respectively";

		private bool rangeBuffActive;
		private bool speedBuffActive;
		private bool damageBuffActive;
		private float duration;

		private List<NailSlash> slashList;
		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			rangeBuffActive = false;
			speedBuffActive = false;
			damageBuffActive = false;
			duration = 5f;

			slashList = new List<NailSlash>()
			{
				HeroController.instance.normalSlash,
				HeroController.instance.alternateSlash,
				HeroController.instance.upSlash,
				HeroController.instance.downSlash
			};
			multiplier = 1f + level * 0.1f;

			On.HutongGames.PlayMaker.Actions.SendEventByName.OnEnter += SendEventByNameOnEnter;
			On.HutongGames.PlayMaker.Actions.SendMessage.OnEnter += SendMessageOnEnter;
		}

		private void SendEventByNameOnEnter(On.HutongGames.PlayMaker.Actions.SendEventByName.orig_OnEnter orig, SendEventByName self)
		{
			orig(self);
			if(self.Fsm.GameObjectName == "Knight" && self.Fsm.Name == "Spell Control")
			{
				if(self.State.Name == "Fireball Recoil")
				{
					StopCoroutine(FireballControl());
					StartCoroutine(FireballControl());
				}
				if(self.State.Name == "Scream End" || self.State.Name == "Scream End 2")
				{
					StopCoroutine(ScreamControl());
					StartCoroutine(ScreamControl());
				}
			}
		}

		private void SendMessageOnEnter(On.HutongGames.PlayMaker.Actions.SendMessage.orig_OnEnter orig, SendMessage self)
		{
			orig(self);
			if(self.Fsm.GameObjectName == "Knight" && self.Fsm.Name == "Spell Control" && self.State.Name == "Quake Finish")
			{
				StopCoroutine(DiveControl());
				StartCoroutine(DiveControl());
			}
		}

		private IEnumerator FireballControl()
		{
			if(!rangeBuffActive)
			{
				foreach(NailSlash slash in slashList)
				{
					slash.scale.x *= multiplier;
					slash.scale.y *= multiplier;
					slash.SetMantis(true);
				}

				rangeBuffActive = true;
			}
			yield return new WaitForSeconds(duration);
			if(rangeBuffActive)
			{
				foreach(NailSlash slash in slashList)
				{
					slash.scale.x /= multiplier;
					slash.scale.y /= multiplier;
					slash.SetMantis(PlayerData.instance.GetBool("equippedCharm_13"));
				}

				rangeBuffActive = false;
			}
			yield break;
		}

		private IEnumerator DiveControl()
		{
			if(!speedBuffActive)
			{
				HeroController.instance.RUN_SPEED *= multiplier;
				HeroController.instance.RUN_SPEED_CH *= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO *= multiplier;
				HeroController.instance.WALK_SPEED *= multiplier;
				HeroController.instance.UNDERWATER_SPEED *= multiplier;

				speedBuffActive = true;
			}
			yield return new WaitForSeconds(duration);
			if(speedBuffActive)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;

				speedBuffActive = false;
			}
			yield break;
		}

		private IEnumerator ScreamControl()
		{
			if(!damageBuffActive)
			{
				On.HealthManager.Hit += HealthManagerHit;

				damageBuffActive = true;
			}
			yield return new WaitForSeconds(duration);
			if(damageBuffActive)
			{
				On.HealthManager.Hit -= HealthManagerHit;

				damageBuffActive = false;
			}
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				hitInstance.Multiplier += multiplier - 1f;
			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HutongGames.PlayMaker.Actions.SendEventByName.OnEnter -= SendEventByNameOnEnter;
			On.HutongGames.PlayMaker.Actions.SendMessage.OnEnter -= SendMessageOnEnter;
			StopAllCoroutines();

			if(rangeBuffActive)
			{
				HeroController.instance.normalSlash.scale.x /= multiplier;
				HeroController.instance.normalSlash.scale.y /= multiplier;
				HeroController.instance.alternateSlash.scale.x /= multiplier;
				HeroController.instance.alternateSlash.scale.y /= multiplier;
				HeroController.instance.upSlash.scale.x /= multiplier;
				HeroController.instance.upSlash.scale.y /= multiplier;
				HeroController.instance.downSlash.scale.x /= multiplier;
				HeroController.instance.downSlash.scale.y /= multiplier;

				rangeBuffActive = false;
			}
			if(speedBuffActive)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;

				speedBuffActive = false;
			}
			if(damageBuffActive)
			{
				On.HealthManager.Hit -= HealthManagerHit;

				damageBuffActive = false;
			}
		}
	}
}
