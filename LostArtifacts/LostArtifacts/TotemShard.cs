using System;
using System.Collections.Generic;
using Satchel;
using System.Collections;
using UnityEngine;

namespace LostArtifacts
{
	public class TotemShard : Artifact
	{
		public override int ID() => 5;
		public override string Name() => "Totem Shard";
		public override string Description() => "The scholars of the Soul Sanctum were fascinated by a totem that released SOUL " +
			"as pure destructive energy. Though it has shattered from overuse, the shards retain some of its original power.";
		public override string Levels() => "5, 7.5, 10 second duration";
		public override string TraitName() => "Soulful";
		public override string TraitDescription() => "Deal +25% damage after healing (Deep Focus gives +50%, Quick Focus adds 2.5 seconds)";

		private bool buffActive;
		private PlayMakerFSM spellFSM;

		public override void Activate()
		{
			base.Activate();

			buffActive = false;

			spellFSM = HeroController.instance.spellControl;
			spellFSM.AddCustomAction("Focus Get Finish", () =>
			{
				StopAllCoroutines();
				StartCoroutine(DamageControl());
			});
			spellFSM.AddCustomAction("Focus Get Finish 2", () =>
			{
				StopAllCoroutines();
				StartCoroutine(DamageControl());
			});

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				if(buffActive)
				{
					float multiplier = PlayerData.instance.GetBool("equippedCharm_34") ? 1.5f : 1.25f;
					hitInstance.DamageDealt = (int)(hitInstance.DamageDealt * multiplier);
				}
			}
			orig(self, hitInstance);
		}

		private IEnumerator DamageControl()
		{
			buffActive = true;

			float duration = 0f;
			if(level == 1) duration = 5f;
			if(level == 2) duration = 7.5f;
			if(level == 3) duration = 10f;
			if(PlayerData.instance.GetBool("equippedCharm_07")) duration += 2.5f;

			yield return new WaitForSeconds(duration);

			buffActive = false;
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();

			spellFSM.RemoveAction("Focus Get Finish", 15);
			spellFSM.RemoveAction("Focus Get Finish 2", 17);

			buffActive = false;
		}
	}
}
