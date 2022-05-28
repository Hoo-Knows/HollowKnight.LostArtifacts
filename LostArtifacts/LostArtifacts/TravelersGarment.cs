using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostArtifacts
{
	public class TravelersGarment : Artifact
	{
		private float runSpeed;
		private float runSpeedCh;
		private float runSpeedChCombo;
		private float walkSpeed;

		public override void Initialize()
		{
			id = 0;
			unlocked = true;
			name = "Traveler's Garment";
			description = "A small cloth from a traveler who braved the wasteland beyond to reach Hallownest. It carries the aura of its former owner.";
			traitName = "Resilience";
			traitDescription = "Increases damage with velocity";
			active = false;
		}

		public override void Activate()
		{
			LostArtifacts.Instance.Log("Activating " + traitName);

			runSpeed = HeroController.instance.RUN_SPEED;
			runSpeedCh = HeroController.instance.RUN_SPEED_CH;
			runSpeedChCombo = HeroController.instance.RUN_SPEED_CH_COMBO;
			walkSpeed = HeroController.instance.WALK_SPEED;

			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			LostArtifacts.Instance.Log("Deactivating " + traitName);
			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}