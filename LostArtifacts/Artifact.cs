using UnityEngine;

namespace LostArtifacts
{
    public abstract class Artifact : MonoBehaviour
    {
        public int id;
        public bool unlocked;
        public new string name;
        public string description;
        public string traitName;
        public string traitDescription;
        public Sprite sprite;
        public bool active;

		public abstract void Initialize();

		public virtual void Activate()
		{
			LostArtifacts.Instance.Log("Activating " + name);
			On.HealthManager.Hit += HealthManagerHit;
		}

		private void HealthManagerHit(On.HealthManager.orig_Hit orig, HealthManager self, HitInstance hitInstance)
		{
			if(hitInstance.AttackType == AttackTypes.Nail)
			{
				LostArtifacts.Instance.Log("Hit enemy with " + name);
			}
			orig(self, hitInstance);
		}

		public virtual void Deactivate()
		{
			LostArtifacts.Instance.Log("Deactivating " + name);
			On.HealthManager.Hit -= HealthManagerHit;
		}
	}
}