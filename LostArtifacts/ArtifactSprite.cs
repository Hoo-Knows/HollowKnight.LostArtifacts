using ItemChanger;
using UnityEngine;
using System.IO;
using Satchel;
using Newtonsoft.Json;

namespace LostArtifacts
{
	public class ArtifactSprite : ISprite
	{
		public string name;
		[JsonIgnore] public Sprite Value => GetArtifactSprite();

		public ArtifactSprite(string name)
		{
			this.name = name;
		}

		public ISprite Clone()
		{
			return (ISprite)base.MemberwiseClone();
		}

		public Sprite GetArtifactSprite()
		{
			Texture2D texture = TextureUtils.createTextureOfColor(64, 64, Color.clear);

			string path = Path.Combine(LostArtifacts.Instance.iconPath, name + ".png");

			//Extract sprite from Resources if it doesn't exist
			if(!File.Exists(path))
			{
				try
				{
					Stream stream = typeof(LostArtifacts).Assembly.GetManifestResourceStream("LostArtifacts.Resources." + name + ".png");
					{
						var buffer = new byte[stream.Length];
						stream.Read(buffer, 0, buffer.Length);
						File.WriteAllBytes(path, buffer);
						stream.Dispose();
					}
				}
				catch
				{
					LostArtifacts.Instance.Log("Failed to extract sprite for " + name);
				}
			}
			if(File.Exists(path)) texture = GetPremultipliedAlpha(TextureUtils.LoadTextureFromFile(path));

			return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 64f);
		}

		public static Texture2D GetPremultipliedAlpha(Texture2D source)
		{
			//Snippet from Kaykao to remove white halo around sprites
			Color[] sourcePixels = source.GetPixels();
			Color[] destPixels = new Color[sourcePixels.Length];

			for(int i = 0; i < sourcePixels.Length; i++)
			{
				Color px = sourcePixels[i];
				destPixels[i] = new Color(px.r * px.a, px.g * px.a, px.b * px.a, px.a);
			}

			Texture2D dest = new(source.width, source.height, TextureFormat.ARGB32, false);
			dest.SetPixels(destPixels);
			dest.Apply();
			return dest;
		}
	}
}
