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
			ArtifactManager.Instance.SetSelected(gameObject);

			if(alt)
			{
				if(left) LostArtifacts.Instance.pageFSM.SendEvent("LEFT");
				else LostArtifacts.Instance.pageFSM.SendEvent("RIGHT");
			}

			//Update artifact panel
			ArtifactManager.Instance.SetArtifactPanel();
		}

		public void Confirm()
		{
			if(left) LostArtifacts.Instance.pageFSM.SendEvent("LEFT");
			else LostArtifacts.Instance.pageFSM.SendEvent("RIGHT");
		}
	}
}