using UnityEngine;
using UnityEngine.UI;

namespace LostArtifactsUI
{
	public class ArtifactCursor : MonoBehaviour
	{
		public static ArtifactCursor Instance;

		private void Awake()
		{
			Instance = this;
		}

		public void UpdatePos()
		{
			GameObject selected = ArtifactManager.Instance.eventSystem.currentSelectedGameObject;

			if(selected == null)
			{
				gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.empty;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.cursor;
				iTween.MoveTo(gameObject, selected.transform.position, 0.3f);
			}
		}
	}
}