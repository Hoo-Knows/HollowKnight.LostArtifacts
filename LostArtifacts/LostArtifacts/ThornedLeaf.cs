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
		public override string LoreDescription() => "An extremely sharp and prickly leaf taken from the hostile foliage of the " +
			"Queen’s Gardens. Even holding it by the stem is dangerous enough.";
		public override string LevelInfo() => level * 2 + " damage ticks per second";
		public override string TraitName() => "Lacerating";
		public override string TraitDescription() => "Striking an enemy inflicts a damage over time effect for 5 seconds (cannot stack).";
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

			On.HealthManager.TakeDamage += HealthManagerTakeDamage;
		}

		private void HealthManagerTakeDamage(On.HealthManager.orig_TakeDamage orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail || hitInstance.AttackType == AttackTypes.NailBeam)
			{
				if(self.gameObject.GetComponent<Laceration>() != null)
				{
					self.gameObject.GetComponent<Laceration>().numTicks = 0;
				}
				else
				{
					self.gameObject.AddComponent<Laceration>();
				}
				self.gameObject.GetComponent<Laceration>().level = level;
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

			On.HealthManager.TakeDamage -= HealthManagerTakeDamage;
		}
	}

	internal class Laceration : MonoBehaviour
	{
		public int level;
		public int numTicks = 0;
		private float damageInterval;
		private HealthManager hm;
		private SpriteFlash flash;

		private void Start()
		{
			float tps = level * 2f;
			damageInterval = 1f / tps;
			hm = GetComponent<HealthManager>();
			flash = GetComponent<SpriteFlash>();

			StartCoroutine(DealDamage());
		}

		private IEnumerator DealDamage()
		{
			for(; numTicks < 5f / damageInterval; numTicks++)
			{
				if(hm != null && hm.hp > 0 && !hm.IsInvincible)
				{
					hm.ApplyExtraDamage(1);
					flash.flash(Color.white, 0.5f, 0.001f, 0.05f, 0.001f);
				}
				yield return new WaitForSeconds(damageInterval);
			}
			Destroy(this);
		}
	}
}
