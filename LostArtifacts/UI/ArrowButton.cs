using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifactsUI
{
	public class ArrowButton : Button
	{
		private bool left;
		private bool alt;

		protected override void Start()
		{
			left = gameObject.name.StartsWith("Left");
			alt = gameObject.name.EndsWith("Alt");
		}

		public override void OnSelect(BaseEventData eventData)
		{
			ArtifactManager.Instance.selectedButton = null;
			if(alt)
			{
				if(left) ArtifactManager.Instance.Left();
				else ArtifactManager.Instance.Right();
			}

			ArtifactAudio.Instance.Play(ArtifactAudio.Instance.select);

			//Move cursor
			ArtifactCursor.Instance.UpdatePos();

			//Update artifact panel
			ArtifactManager.Instance.SetArtifactPanel("", ArtifactManager.Instance.empty, "");
		}

		public void Confirm()
		{
			if(left) ArtifactManager.Instance.Left();
			else ArtifactManager.Instance.Right();
		}
	}
}