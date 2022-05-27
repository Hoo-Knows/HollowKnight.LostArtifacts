using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifactsUI
{
	public class ArtifactButton : Button
	{
		public LostArtifacts.Artifact artifact;
		public bool choosing;
		public bool equipped;

		private int id;
		private ArtifactSlot slot;
		private Vector3 pos;

		protected override void Start()
		{
			id = int.Parse(name.Split(new char[] { ' ' })[1]);
			pos = gameObject.transform.localPosition;

			artifact = ArtifactManager.Instance.artifacts[id];
			choosing = false;
			equipped = false;

			if(artifact != null && artifact.unlocked) gameObject.GetComponent<Image>().sprite = artifact.sprite;
			else gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.empty;
		}

		public override void OnSelect(BaseEventData eventData)
		{
			ArtifactAudio.Instance.Play(ArtifactAudio.Instance.select);

			//Move cursor
			ArtifactCursor.Instance.UpdatePos();

			//Update artifact panel
			if(artifact != null && artifact.unlocked)
			{
				ArtifactManager.Instance.SetArtifactPanel(artifact.name, artifact.sprite, artifact.description);
			}
			else
			{
				ArtifactManager.Instance.SetArtifactPanel("", ArtifactManager.Instance.empty, "");
			}
		}

		public void Confirm()
		{
			if(artifact == null) return;
			if(ArtifactManager.Instance.nailLevel == 0 || !artifact.unlocked) return;

			ArtifactAudio.Instance.Play(ArtifactAudio.Instance.submit);

			if(!equipped)
			{
				choosing = true;

				//Set default slot
				if(slot == null)
				{
					slot = ArtifactManager.Instance.defaultSlot;
				}

				//Make cursor track slots
				slot.Select();

				//Set this artifact as cursor's selected button
				ArtifactManager.Instance.selectedButton = this;
			}
			else
			{
				if(slot != null)
				{
					Unequip();
				}
			}
		}

		public void Equip()
		{
			//Set slot
			slot = ArtifactManager.Instance.eventSystem.currentSelectedGameObject.GetComponent<ArtifactSlot>();

			//Reset cursor selected
			ArtifactManager.Instance.selectedButton = null;

			//Reset position
			choosing = false;
			gameObject.transform.localPosition = pos;

			//Select this
			Select();

			//Dim sprite
			gameObject.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);

			//Start artifact effect
			if(artifact != null && !artifact.active)
			{
				artifact.Activate();
				artifact.active = true;
			}

			equipped = true;
		}

		public void Unequip()
		{
			//Call slot unequip
			slot.Unequip();

			//Reset slot
			slot = null;

			//Lighten sprite
			gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f);

			//Stop artifact effect
			if(artifact != null && artifact.active)
			{
				artifact.Deactivate();
				artifact.active = false;
			}

			equipped = false;
		}

		private void Update()
		{
			if(choosing)
			{
				gameObject.transform.position = ArtifactCursor.Instance.transform.position;
			}
		}
	}
}