using ItemChanger;
using ItemChanger.Locations;

namespace LostArtifacts.Artifacts
{
	public class TemplateArtifact : Artifact
	{
		public override int ID() => 0;
		public override string Name() => "";
		public override string LoreDescription() => "";
		public override string LevelInfo() => "";
		public override string TraitName() => "";
		public override string TraitDescription() => "";
		public override AbstractLocation Location()
		{
			return new CoordinateLocation()
			{
				name = InternalName(),
				sceneName = "",
				x = 0f,
				y = 0f,
				elevation = 0f
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

			}
			orig(self, hitInstance);
		}

		public override void Deactivate()
		{
			base.Deactivate();

			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}
