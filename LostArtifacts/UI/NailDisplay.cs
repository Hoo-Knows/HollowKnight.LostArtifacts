using UnityEngine;
using UnityEngine.UI;

namespace LostArtifacts.UI
{
    public class NailDisplay : MonoBehaviour
    {
        void Start()
        {
            gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.nailSprites[ArtifactManager.Instance.nailLevel];
        }
    }
}