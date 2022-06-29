using UnityEngine;
using UnityEngine.UI;

namespace LostArtifacts.UI
{
	public class NailDisplay : MonoBehaviour
	{
		protected void OnEnable()
		{
			gameObject.GetComponent<Image>().sprite = 
				ArtifactManager.Instance.nailSprites[PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades))];
		}
	}
}