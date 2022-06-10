using UnityEngine;

namespace LostArtifacts
{
    public abstract class Artifact : MonoBehaviour
    {
        public bool unlocked { get; set; }
        public bool active { get; set; }
        public int level { get; set; }
        public Sprite sprite { get; set; }

        public abstract int ID();
        public abstract string Name();
        public abstract string Description();
        public abstract string TraitName();
        public abstract string TraitDescription();

        public virtual void Activate()
		{
            LostArtifacts.Instance.Log("Activating " + TraitName() + " " + new string('I', level));
            active = true;
        }

        public virtual void Deactivate()
		{
            LostArtifacts.Instance.Log("Deactivating " + TraitName() + " " + new string('I', level));
            active = false;
            level = 0;
        }

        public void Unlock()
		{
            unlocked = true;
            LostArtifacts.Settings.unlocked[ID()] = true;
		}
	}
}