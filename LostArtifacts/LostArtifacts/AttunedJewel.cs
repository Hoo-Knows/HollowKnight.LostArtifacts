using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class AttunedJewel : Artifact
	{
		public override int ID() => 19;
		public override string Name() => "Attuned Jewel";
		public override string LoreDescription() => "The Godseekers crafted this jewel through harnessing the power of the Gods of " +
			"Thunder and Rain. It reveals the bearer’s inner nature and allows them to ascend ever higher.";
		public override string LevelInfo() => string.Format("+{0}% range, movement speed, or damage after a spell", 10 + level * 5);
		public override string TraitName() => "Attuned";
		public override string TraitDescription() => "Casting Fireball, Dive, or Shriek grants bonus attack range, movement speed, " +
			"or damage for 5 seconds.";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.GG_Workshop),
				x = 241.7f,
				y = 6.4f,
				elevation = 0f
			};
		}

		private int rangeBuffActive;
		private int speedBuffActive;
		private int damageBuffActive;
		private float duration;

		private List<NailSlash> slashList;
		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			rangeBuffActive = 0;
			speedBuffActive = 0;
			damageBuffActive = 0;
			duration = 5f;

			slashList = new List<NailSlash>()
			{
				HeroController.instance.normalSlash,
				HeroController.instance.alternateSlash,
				HeroController.instance.upSlash,
				HeroController.instance.downSlash
			};
			multiplier = 1.1f + level * 0.05f;

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
					StartCoroutine(FireballControl());
				}
				if(self.State.Name == "Scream End" || self.State.Name == "Scream End 2")
				{
					StartCoroutine(ScreamControl());
				}
			}
		}

		private void SendMessageOnEnter(On.HutongGames.PlayMaker.Actions.SendMessage.orig_OnEnter orig, SendMessage self)
		{
			orig(self);
			if(self.Fsm.GameObjectName == "Knight" && self.Fsm.Name == "Spell Control" && self.State.Name == "Quake Finish")
			{
				StartCoroutine(DiveControl());
			}
		}

		private IEnumerator FireballControl()
		{
			rangeBuffActive++;
			if(rangeBuffActive == 1)
			{
				foreach(NailSlash slash in slashList)
				{
					slash.scale.x *= multiplier;
					slash.scale.y *= multiplier;
				}
			}
			yield return new WaitForSeconds(duration);
			rangeBuffActive--;
			if(rangeBuffActive == 0)
			{
				foreach(NailSlash slash in slashList)
				{
					slash.scale.x /= multiplier;
					slash.scale.y /= multiplier;
				}
			}
			yield break;
		}

		private IEnumerator DiveControl()
		{
			speedBuffActive++;
			if(speedBuffActive == 1)
			{
				HeroController.instance.RUN_SPEED *= multiplier;
				HeroController.instance.RUN_SPEED_CH *= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO *= multiplier;
				HeroController.instance.WALK_SPEED *= multiplier;
				HeroController.instance.UNDERWATER_SPEED *= multiplier;
			}
			yield return new WaitForSeconds(duration);
			speedBuffActive--;
			if(speedBuffActive == 0)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;
			}
			yield break;
		}

		private IEnumerator ScreamControl()
		{
			damageBuffActive++;
			if(damageBuffActive == 1)
			{
				On.HealthManager.Hit += HealthManagerHit;
			}
			yield return new WaitForSeconds(duration);
			damageBuffActive--;
			if(damageBuffActive == 0)
			{
				On.HealthManager.Hit -= HealthManagerHit;
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

			if(rangeBuffActive > 0)
			{
				HeroController.instance.normalSlash.scale.x /= multiplier;
				HeroController.instance.normalSlash.scale.y /= multiplier;
				HeroController.instance.alternateSlash.scale.x /= multiplier;
				HeroController.instance.alternateSlash.scale.y /= multiplier;
				HeroController.instance.upSlash.scale.x /= multiplier;
				HeroController.instance.upSlash.scale.y /= multiplier;
				HeroController.instance.downSlash.scale.x /= multiplier;
				HeroController.instance.downSlash.scale.y /= multiplier;

				rangeBuffActive = 0;
			}
			if(speedBuffActive > 0)
			{
				HeroController.instance.RUN_SPEED /= multiplier;
				HeroController.instance.RUN_SPEED_CH /= multiplier;
				HeroController.instance.RUN_SPEED_CH_COMBO /= multiplier;
				HeroController.instance.WALK_SPEED /= multiplier;
				HeroController.instance.UNDERWATER_SPEED /= multiplier;

				speedBuffActive = 0;
			}
			if(damageBuffActive > 0)
			{
				On.HealthManager.Hit -= HealthManagerHit;

				damageBuffActive = 0;
			}
		}
	}
}
