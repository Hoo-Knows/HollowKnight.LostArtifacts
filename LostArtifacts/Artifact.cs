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
		public abstract string Description();
		public abstract string LevelInfo();
		public abstract string TraitName();
		public abstract string TraitDescription();
		public abstract AbstractLocation Location();

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
	}
}