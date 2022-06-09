using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifacts.UI
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
			//Set selected
			ArtifactManager.Instance.selected = gameObject;

			if(alt)
			{
				if(left) LostArtifacts.Instance.pageFSM.SendEvent("LEFT");
				else LostArtifacts.Instance.pageFSM.SendEvent("RIGHT");
			}

			ArtifactAudio.Instance.Play(ArtifactAudio.Instance.select);

			//Move cursor
			ArtifactCursor.Instance.UpdatePos();

			//Update artifact panel
			ArtifactManager.Instance.SetArtifactPanel("", ArtifactManager.Instance.empty, "");
		}

		public void Confirm()
		{
			if(left) LostArtifacts.Instance.pageFSM.SendEvent("LEFT");
			else LostArtifacts.Instance.pageFSM.SendEvent("RIGHT");
		}
	}
}