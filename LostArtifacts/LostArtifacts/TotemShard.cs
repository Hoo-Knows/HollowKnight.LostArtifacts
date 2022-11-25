using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Locations;
using System.Collections;
using UnityEngine;

namespace LostArtifacts.Artifacts
{
	public class TotemShard : Artifact
	{
		public override int ID() => 5;
		public override string Name() => "Totem Shard";
		public override string LoreDescription() => "The scholars of the Soul Sanctum were fascinated by a totem that released SOUL " +
			"as pure destructive energy. Though it has shattered from overuse, the shards retain some of its original power.";
		public override string LevelInfo() => 2.5 * level + " second base duration after healing";
		public override string TraitName() => "Soulful";
		public override string TraitDescription() => "Deal +20% damage after healing. Deep Focus gives +100% " +
			"and Quick Focus doubles the duration.";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = nameof(SceneNames.Ruins1_32),
				x = 13.9f,
				y = 96.4f,
				elevation = 0f
			};
		}

		private GameObject auraGO;
		private bool buffActive;
		private tk2dSpriteAnimator heroAnimator = null;
		private tk2dSpriteAnimator HeroAnimator => heroAnimator ??= HeroController.instance.gameObject.GetComponent<tk2dSpriteAnimator>();

		public override void Activate()
		{
			base.Activate();

			buffActive = false;

			On.HealthManager.Hit += HealthManagerHit;
		}

		public void Update()
		{
			if(active && HeroAnimator.IsPlaying("Focus Get Once"))
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
					float multiplier = PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_34)) ? 1f : 0.2f;
					hitInstance.Multiplier += multiplier;
				}
			}
			orig(self, hitInstance);
		}

		private IEnumerator DamageControl()
		{
			buffActive = true;

			if(auraGO == null) FindAura();
			auraGO.SetActive(true);
			auraGO.GetComponent<MeshRenderer>().enabled = true;
			auraGO.GetComponent<tk2dSpriteAnimator>().Play("Focus Effect");

			float duration = level * 2.5f;
			if(PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_7))) duration *= 2f;

			yield return new WaitForSeconds(duration);

			auraGO.GetComponent<tk2dSpriteAnimator>().Play("Focus Effect End");
			yield return new WaitWhile(() => auraGO.GetComponent<tk2dSpriteAnimator>().IsPlaying("Focus Effect End"));
			auraGO.GetComponent<MeshRenderer>().enabled = false;
			auraGO.SetActive(false);

			buffActive = false;
		}

		private void FindAura()
		{
			auraGO = Instantiate(HeroController.instance.spellControl.FsmVariables.FindFsmGameObject("Lines Anim").Value);
			auraGO.transform.parent = HeroController.instance.spellControl.transform;
			auraGO.transform.localPosition = new Vector3();
			DontDestroyOnLoad(auraGO);
			auraGO.SetActive(false);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
			StopAllCoroutines();

			Destroy(auraGO);
			buffActive = false;
		}
	}
}
