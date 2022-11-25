using ItemChanger;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LostArtifacts
{
	public abstract class Artifact : MonoBehaviour
	{
		public bool active { get; set; }
		public int level { get; set; }
		public ArtifactSprite sprite { get; set; }

		public abstract int ID();
		public abstract string Name();
		public string InternalName() => Regex.Replace(Name(), @"[^0-9a-zA-Z\._]", "");
		public abstract string LoreDescription();
		public abstract string TraitName();
		public abstract string TraitDescription();
		public abstract string LevelInfo();
		public abstract AbstractLocation Location();

		public virtual string Description()
		{
			return string.Format("{0}\n\nThis artifact carries the trait <b>{1}</b>: {2}", 
				LoreDescription(), TraitName(), TraitDescription());
		}

		public virtual void Activate()
		{
			LostArtifacts.Instance.LogDebug("Activating " + TraitName() + " " + (level == 4 ? "IV" : new string('I', level)));
			active = true;
		}

		public virtual void Deactivate()
		{
			LostArtifacts.Instance.LogDebug("Deactivating " + TraitName() + " " + (level == 4 ? "IV" : new string('I', level)));
			active = false;
			level = 0;
		}
	}
}