using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using UnityEngine;
using Satchel;
using System.Collections;

namespace LostArtifacts.Artifacts
{
	public class VoidEmblem : Artifact
	{
		public override int ID() => 18;
		public override string Name() => "Void Emblem";
		public override string LoreDescription() => "An emblem crafted by the ancient civilization designed to contain void energy. " +
			"Holding it provides a strange sort of comfort.";
		public override string LevelInfo() => 9 - 2 * level + " hits to reach maximum buff";
		public override string TraitName() => "Abyssal";
		public override string TraitDescription() => "Striking an enemy repeatedly builds up the damage of the next spell " +
			"with a maximum +50% increase.";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Abyss_09),
				x = 210.2f,
				y = 50.4f,
				elevation = 0f
			};
		}

		private int counter;
		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			counter = 0;
			multiplier = 0f;

			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
			On.HealthManager.Hit += HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter += GetPlayerDataIntOnEnter;
			On.HutongGames.PlayMaker.Actions.SetVelocity2d.OnEnter += SetVelocity2dOnEnter;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				counter++;

				HeroController.instance.shadowRechargePrefab.SetActive(true);
				HeroController.instance.shadowRechargePrefab.LocateMyFSM("Recharge Effect").SetState("Play anim");
			}
			orig(self, hitInstance);
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Spell)
			{
				hitInstance.Multiplier += multiplier;
			}
			orig(self, hitInstance);
		}

		private void GetPlayerDataIntOnEnter(On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.orig_OnEnter orig,
			GetPlayerDataInt self)
		{
			orig(self);
			if(self.Fsm.GameObjectName == "Knight" && self.Fsm.Name == "Spell Control" &&
				(self.State.Name == "Has Fireball?" || self.State.Name == "Has Quake?" || self.State.Name == "Has Scream?"))
			{
				multiplier = 0.5f * Mathf.Min(counter, 9f - 2f * level) / (9f - 2f * level);

				if(counter > 0)
				{
					StartCoroutine(ShadowRing());
					counter = 0;
				}
			}
		}

		private void SetVelocity2dOnEnter(On.HutongGames.PlayMaker.Actions.SetVelocity2d.orig_OnEnter orig, SetVelocity2d self)
		{
			orig(self);
			if(self.Fsm.GameObjectName == "Knight" && self.Fsm.Name == "Spell Control" && self.State.Name == "Spell End")
			{
				multiplier = 0f;
			}
		}

		private IEnumerator ShadowRing()
		{
			yield return new WaitForSeconds(0.1f);
			GameObject shadowRing = HeroController.instance.shadowRingPrefab.Spawn(HeroController.instance.transform);
			iTweenScaleTo tweenAction = shadowRing.LocateMyFSM("Play Effect").GetAction<iTweenScaleTo>("Grow", 0);
			tweenAction.easeType = iTween.EaseType.linear;
			tweenAction.vectorScale = new Vector3(10f, 10f, 10f);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
			On.HealthManager.Hit -= HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter -= GetPlayerDataIntOnEnter;
			On.HutongGames.PlayMaker.Actions.SetVelocity2d.OnEnter -= SetVelocity2dOnEnter;
		}
	}
}
