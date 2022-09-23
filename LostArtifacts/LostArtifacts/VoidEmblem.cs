using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class VoidEmblem : Artifact
	{
		public override int ID() => 18;
		public override string Name() => "Void Emblem";
		public override string Description() => "An emblem crafted by the ancient civilization designed to contain void energy. " +
			"Holding it provides a strange sort of comfort.";
		public override string LevelInfo() => "7, 5, 3 hits to reach max";
		public override string TraitName() => "Abyssal";
		public override string TraitDescription() => "Striking an enemy repeatedly builds up the damage of the next spell " +
			"(max +50% increase)";
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

		private float counter;
		private float multiplier;

		public override void Activate()
		{
			base.Activate();

			counter = 0f;
			multiplier = 0f;

			On.HealthManager.Hit += HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter += GetPlayerDataIntOnEnter;
			On.HutongGames.PlayMaker.Actions.SetVelocity2d.OnEnter += SetVelocity2dOnEnter;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				counter++;
			}
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
				counter = Mathf.Min(counter, (4f - level) * 2f + 1f);
				multiplier = 0.5f * counter / ((4f - level) * 2f + 1f);
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

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			On.HutongGames.PlayMaker.Actions.GetPlayerDataInt.OnEnter -= GetPlayerDataIntOnEnter;
			On.HutongGames.PlayMaker.Actions.SetVelocity2d.OnEnter -= SetVelocity2dOnEnter;
		}
	}
}
