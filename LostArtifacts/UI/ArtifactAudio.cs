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
            gameObject.GetComponent<AudioSource>().PlayOneShot(clip, 0.5f);
        }
    }
}