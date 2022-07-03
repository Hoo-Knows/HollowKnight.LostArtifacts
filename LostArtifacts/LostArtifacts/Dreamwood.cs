using ItemChanger;
using ItemChanger.Locations;
using Modding;
using System.Collections.Generic;

namespace LostArtifacts.Artifacts
{
	public class Dreamwood : Artifact
	{
		public override int ID() => 9;
		public override string Name() => "Dreamwood";
		public override string Description() => "A small piece of a Whispering Root. It has an affinity with dreams that enhances " +
			"the Dream Nail’s ability to mediate between dreams and reality.";
		public override string LevelInfo() => "Effect stacks up to 1, 2, 3 times on an enemy";
		public override string TraitName() => "Dreamlink";
		public override string TraitDescription() => "Using Dream Nail on an enemy links them to the nail, making them take 1/6 " +
			"base nail damage every swing";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.RestingGrounds_05),
				Test = new SDBool("Dream Plant", nameof(SceneNames.RestingGrounds_05)),
				falseLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.RestingGrounds_05),
					x = 15.9f,
					y = -100f,
					elevation = 0f
				},
				trueLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.RestingGrounds_05),
					x = 15.9f,
					y = 69.4f,
					elevation = 0f
				}
			};
		}

		private int damage;
		private Dictionary<HealthManager, int> hmDict;

		public override void Activate()
		{
			base.Activate();

			damage = PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)) / 6 + 1;
			hmDict = new Dictionary<HealthManager, int>();

			On.EnemyDreamnailReaction.RecieveDreamImpact += EnemyDreamnailReactionRecieveDreamImpact;
			ModHooks.AttackHook += AttackHook;
		}

		private void EnemyDreamnailReactionRecieveDreamImpact(
			On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
		{
			orig(self);
			HealthManager hm = self.gameObject.GetComponent<HealthManager>();
			if(hm != null)
			{
				if(hmDict.ContainsKey(hm))
				{
					hmDict[hm]++;
					if(hmDict[hm] > 3) hmDict[hm] = 3;
				}
				else
				{
					hmDict.Add(hm, 1);
				}
			}
		}

		private void AttackHook(GlobalEnums.AttackDirection obj)
		{
			foreach(HealthManager hm in hmDict.Keys)
			{
				if(hm == null)
				{
					hmDict.Remove(hm);
					continue;
				}
				HitInstance hi = new HitInstance
				{
					AttackType = AttackTypes.Nail,
					Direction = 0f,
					DamageDealt = damage * hmDict[hm],
					Source = HeroController.instance.gameObject,
					IgnoreInvulnerable = true,
					MagnitudeMultiplier = 0f,
					Multiplier = 1f
				};
				hm.Hit(hi);
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.EnemyDreamnailReaction.RecieveDreamImpact -= EnemyDreamnailReactionRecieveDreamImpact;
			ModHooks.AttackHook -= AttackHook;
		}
	}
}
