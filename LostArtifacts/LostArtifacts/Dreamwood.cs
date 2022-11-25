using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class Dreamwood : Artifact
	{
		public override int ID() => 9;
		public override string Name() => "Dreamwood";
		public override string LoreDescription() => "A small piece of a Whispering Root. It enhances the Dream Nail, allowing it to " +
			"weaken enemies’ defenses by draining their energy.";
		public override string LevelInfo() => string.Format("+{0}% bonus damage per Dream Nail", 5 * (level + 1));
		public override string TraitName() => "Enervating";
		public override string TraitDescription() => "Striking an enemy with the Dream Nail makes them take bonus " + 
			"damage for 15 seconds (can stack).";
		public override AbstractLocation Location()
		{
			return new DualLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.RestingGrounds_05),
				Test = new PDBool(nameof(PlayerData.completedRGDreamPlant)),
				falseLocation = new DreamwoodLocation()
				{
					name = InternalName(),
					sceneName = nameof(SceneNames.RestingGrounds_05)
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

		public GameObject dreamParticlesGO;
		private float multiplier;
		private Dictionary<HealthManager, int> hmDict;

		public override void Activate()
		{
			base.Activate();

			PlayMakerFSM revekFSM = LostArtifacts.Preloads["RestingGrounds_08"]["Ghost revek"].LocateMyFSM("Appear");
			dreamParticlesGO = revekFSM.FsmVariables.FindFsmGameObject("Idle Pt").Value;

			multiplier = 0.05f * (level + 1);
			hmDict = new Dictionary<HealthManager, int>();

			On.EnemyDreamnailReaction.RecieveDreamImpact += EnemyDreamnailReactionRecieveDreamImpact;
			On.HealthManager.Hit += HealthManagerHit;
		}

		private void EnemyDreamnailReactionRecieveDreamImpact(
			On.EnemyDreamnailReaction.orig_RecieveDreamImpact orig, EnemyDreamnailReaction self)
		{
			orig(self);
			HealthManager hm = self.gameObject.GetComponent<HealthManager>();
			if(hm == null) return;

			if(!hmDict.ContainsKey(hm))
			{
				hmDict.Add(hm, 0);
			}
			StartCoroutine(BuffControl(hm));
		}

		private IEnumerator BuffControl(HealthManager hm)
		{
			hmDict[hm]++;
			GameObject dreamParticles = Instantiate(dreamParticlesGO, hm.gameObject.transform);
			dreamParticles.transform.position += new Vector3(0f, 0f, 1f);
			// It's not obsolete
#pragma warning disable CS0618 // Type or member is obsolete
			dreamParticles.GetComponent<ParticleSystem>().enableEmission = true;
#pragma warning restore CS0618 // Type or member is obsolete
			yield return new WaitForSeconds(15f);
			Destroy(dreamParticles);
			hmDict[hm]--;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hmDict.ContainsKey(self))
			{
				hitInstance.Multiplier += multiplier * hmDict[self];
			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.EnemyDreamnailReaction.RecieveDreamImpact -= EnemyDreamnailReactionRecieveDreamImpact;
			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();
		}
	}

	internal class DreamwoodLocation : ContainerLocation
	{
		protected override void OnLoad()
		{
			On.DreamPlant.CheckOrbs += DreamPlantCheckOrbs;
		}

		private IEnumerator DreamPlantCheckOrbs(On.DreamPlant.orig_CheckOrbs orig, DreamPlant self)
		{
			yield return orig(self);
			if(self.gameObject.scene.name == sceneName)
			{
				GameObject obj;
				string containerType;
				GetContainer(out obj, out containerType);
				Container.GetContainer(containerType).ApplyTargetContext(obj, self.gameObject, 0f);
				if(!obj.activeSelf)
				{
					obj.SetActive(true);
				}
			}
		}

		protected override void OnUnload()
		{
			On.DreamPlant.CheckOrbs -= DreamPlantCheckOrbs;
		}
	}
}
