using UnityEngine;
using UnityEngine.UI;

namespace LostArtifacts.UI
{
	public class ArtifactCursor : MonoBehaviour
	{
		public static ArtifactCursor Instance;
		private GameObject glow;

		private void Awake()
		{
			Instance = this;
		}

		private void Start()
		{
			glow = gameObject.transform.parent.Find("Glow").gameObject;
		}

		public void UpdatePos()
		{
			GameObject selected = ArtifactManager.Instance.selected;

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
		
		private void Update()
		{
			glow.transform.position = gameObject.transform.position;
		}
	}
}