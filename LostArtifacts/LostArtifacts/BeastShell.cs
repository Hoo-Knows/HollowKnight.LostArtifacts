using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class BeastShell : Artifact
	{
		public override int ID() => 14;
		public override string Name() => "Beast Shell";
		public override string Description() => "A trophy from the God Tamer’s beast. It was deeply loyal toward its owner.";
		public override string LevelInfo() => "+100%, +150%, +200% minion damage";
		public override string TraitName() => "Beast Tamer";
		public override string TraitDescription() => "Striking an enemy buffs minion damage for 5 seconds";
		public override AbstractLocation Location()
		{
			return new EnemyLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Room_Colosseum_Gold),
				objectName = "Lobster",
				removeGeo = false
			};
		}

		private float multiplier;
		private bool buffActive;

		public override void Activate()
		{
			base.Activate();

			multiplier = 0.5f + 0.5f * level;
			buffActive = false;

			On.HealthManager.Hit += HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter += IntOperatorOnEnter;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				StopAllCoroutines();
				StartCoroutine(DamageControl());
			}
			if(buffActive && hitInstance.Source.transform.parent != null && 
				hitInstance.Source.transform.parent.name.Contains("Hatchling"))
			{
				hitInstance.Multiplier += multiplier;
			}
			orig(self, hitInstance);
		}

		private IEnumerator DamageControl()
		{
			buffActive = true;
			yield return new WaitForSeconds(5f);
			buffActive = false;
		}

		private void IntOperatorOnEnter(On.HutongGames.PlayMaker.Actions.IntOperator.orig_OnEnter orig, IntOperator self)
		{
			if(!buffActive)
			{
				orig(self);
				return;
			}

			if(self.Fsm.GameObject.name == "Enemy Damager" && self.Fsm.Name == "Attack")
			{
				int value = self.integer2.Value;
				self.integer2.Value *= (int)(multiplier + 1f);
				orig(self);
				self.integer2.Value = value;
				return;
			}

			orig(self);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.IntOperator.OnEnter -= IntOperatorOnEnter;
			StopAllCoroutines();

			buffActive = false;
		}
	}
}
