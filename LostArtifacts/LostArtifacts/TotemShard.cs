﻿using System;
using System.Collections.Generic;
using Satchel;
using System.Collections;
using UnityEngine;
using HutongGames.PlayMaker.Actions;

namespace LostArtifacts
{
	public class TotemShard : Artifact
	{
		public override int ID() => 5;
		public override string Name() => "Totem Shard";
		public override string Description() => "The scholars of the Soul Sanctum were fascinated by a totem that released SOUL " +
			"as pure destructive energy. Though it has shattered from overuse, the shards retain some of its original power.";
		public override string Levels() => "2.5, 5, 7.5 second duration";
		public override string TraitName() => "Soulful";
		public override string TraitDescription() => "Deal +25% damage for a short time after healing; Deep Focus gives +75% " +
			"instead, Quick Focus adds 5 seconds";

		private bool buffActive;

		public override void Activate()
		{
			base.Activate();

			buffActive = false;

			On.HealthManager.Hit += HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.SendEventByName.OnEnter += SendEventByNameOnEnter;
		}

		private void SendEventByNameOnEnter(On.HutongGames.PlayMaker.Actions.SendEventByName.orig_OnEnter orig, SendEventByName self)
		{
			orig(self);
			if(self.Fsm.Name == "Spell Control" && self.State.Name.Contains("Focus Get Finish") && self.sendEvent.Value == "FOCUS END")
			{
				StopAllCoroutines();
				StartCoroutine(DamageControl());
			}
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				if(buffActive)
				{
					float multiplier = PlayerData.instance.GetBool("equippedCharm_34") ? 0.25f : 0.75f;
					hitInstance.Multiplier += multiplier;
				}
			}
			orig(self, hitInstance);
		}

		private IEnumerator DamageControl()
		{
			buffActive = true;

			float duration = level * 2.5f;
			if(PlayerData.instance.GetBool("equippedCharm_07")) duration *= 2f;

			yield return new WaitForSeconds(duration);

			buffActive = false;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.SendEventByName.OnEnter -= SendEventByNameOnEnter;
			StopAllCoroutines();

			buffActive = false;
		}
	}
}
