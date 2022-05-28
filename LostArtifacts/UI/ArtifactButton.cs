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
		public int id;

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

			//Check if already equipped
			if(LostArtifacts.LostArtifacts.Settings.slotHandle == id ||
				LostArtifacts.LostArtifacts.Settings.slotBladeL == id ||
				LostArtifacts.LostArtifacts.Settings.slotBladeR == id ||
				LostArtifacts.LostArtifacts.Settings.slotHead == id)
			{
				gameObject.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
			}
			if(LostArtifacts.LostArtifacts.Settings.slotHead == id && !equipped)
			{
				slot = ArtifactManager.Instance.slotHead;
				ArtifactManager.Instance.slotHead.button = this;
				ArtifactManager.Instance.slotHead.Equip();
				equipped = true;
			}
			if(LostArtifacts.LostArtifacts.Settings.slotBladeR == id && !equipped)
			{
				slot = ArtifactManager.Instance.slotBladeR;
				ArtifactManager.Instance.slotBladeR.button = this;
				ArtifactManager.Instance.slotBladeR.Equip();
				equipped = true;
			}
			if(LostArtifacts.LostArtifacts.Settings.slotBladeL == id && !equipped)
			{
				slot = ArtifactManager.Instance.slotBladeL;
				ArtifactManager.Instance.slotBladeL.button = this;
				ArtifactManager.Instance.slotBladeL.Equip();
				equipped = true;
			}
			if(LostArtifacts.LostArtifacts.Settings.slotHandle == id && !equipped)
			{
				slot = ArtifactManager.Instance.slotHandle;
				ArtifactManager.Instance.slotHandle.button = this;
				ArtifactManager.Instance.slotHandle.Equip();
				equipped = true;
			}
		}

		protected override void OnDisable()
		{
			if(choosing)
			{
				choosing = false;
				gameObject.transform.localPosition = pos;
			}
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

			//Set ArtifactManager's selected button
			ArtifactManager.Instance.selectedButton = this;
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
					slot = ArtifactManager.Instance.slotHandle;
				}

				//Make cursor track slots
				slot.Select();
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
				switch(slot.id)
				{
					case 0:
						artifact.level = 1;
						break;
					case 1:
						artifact.level = 2;
						break;
					case 2:
						artifact.level = 2;
						break;
					case 3:
						artifact.level = 3;
						break;
				}
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
				artifact.level = 0;
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