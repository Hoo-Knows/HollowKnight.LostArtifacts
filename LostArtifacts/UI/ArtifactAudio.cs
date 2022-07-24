using UnityEngine;

namespace LostArtifacts.UI
{
	public class ArtifactAudio : MonoBehaviour
	{
		public static ArtifactAudio Instance;
		public AudioClip select;
		public AudioClip reject;
		public AudioClip submit;

		private void Awake()
		{
			Instance = this;
		}

		public void Play(AudioClip clip)
		{
			HeroController.instance.GetComponent<AudioSource>().PlayOneShot(clip, 1f);
		}
	}
}