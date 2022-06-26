using ItemChanger;
using UnityEngine;

namespace LostArtifacts
{
	public class ArtifactSprite : ISprite
	{
		public Sprite Value => LostArtifacts.Instance.GetArtifactSprite(name);

		public ISprite Clone()
		{
			return (ISprite)base.MemberwiseClone();
		}

		public string name;
	}
}
