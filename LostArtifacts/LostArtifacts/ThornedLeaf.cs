using ItemChanger;
using ItemChanger.Locations;
using Modding;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class ThornedLeaf : Artifact
	{
		public override int ID() => 11;
		public override string Name() => "Thorned Leaf";
		public override string Description() => "An extremely sharp and prickly leaf taken from the hostile foliage of the " +
			"Queen’s Gardens. Even holding it by the stem is dangerous enough.";
		public override string LevelInfo() => "0.75, 0.5, 0.25 seconds per damage tick";
		public override string TraitName() => "Lacerating";
		public override string TraitDescription() => "Striking an enemy inflicts a damage over time effect " +
			"for 5 seconds (cannot stack)";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Fungus3_10),
				Test = new SDBool("Battle Scene", nameof(SceneNames.Fungus3_10)),
				falseLocation = new EnemyLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Fungus3_10),
					objectName = "Battle Scene/Wave 6/Dragonfly Summon (3)/Mantis Heavy Flyer"
				},
				trueLocation = new CoordinateLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.Fungus3_10),
					x = 55f,
					y = 10.4f,
					elevation = 0f
				}
			};
		}

		public override void Activate()
		{
			base.Activate();

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				if(self.gameObject.GetComponent<Laceration>() != null)
				{
					Destroy(self.gameObject.GetComponent<Laceration>());
				}
				if(!self.IsInvincible) self.gameObject.AddComponent<Laceration>();
			}
			orig(self, hitInstance);

			if(hitInstance.Source.name == "LostArtifactsGO")
			{
				ReflectionHelper.SetField(self, "evasionByHitRemaining", 0f);
			}
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
		}
	}

	public class Laceration : MonoBehaviour
	{
		private float damageInterval;

		private void Start()
		{
			int level = 1;
			foreach(Artifact artifact in LostArtifacts.Instance.artifacts)
			{
				if(artifact != null && artifact.Name() == "Thorned Leaf")
				{
					level = artifact.level;
					break;
				}
			}
			damageInterval = 1f - 0.25f * level;

			StartCoroutine(DealDamage());
		}

		private IEnumerator DealDamage()
		{
			for(int i = 0; i < 5f / damageInterval; i++)
			{
				if(gameObject.GetComponent<HealthManager>() != null)
				{
					if(gameObject.GetComponent<HealthManager>().hp <= 0 ||
						gameObject.GetComponent<HealthManager>().IsInvincible) break;

					gameObject.GetComponent<HealthManager>().ApplyExtraDamage(1);
					gameObject.GetComponent<SpriteFlash>().flashWhiteQuick();
				}
				yield return new WaitForSeconds(damageInterval);
			}
			Destroy(this);
		}
	}
}
