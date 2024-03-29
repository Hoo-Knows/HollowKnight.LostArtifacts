using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifacts.UI
{
	public class ArtifactButton : Button
	{
		public int id;
		public Artifact artifact;
		public bool choosing;
		public bool equipped;

		private ArtifactSlot slot;
		private Vector3 pos;

		protected override void Start()
		{
			id = int.Parse(name.Split(new char[] { ' ' })[1]);
			artifact = LostArtifacts.Instance.artifacts[id];
			choosing = false;
			equipped = false;

			if(artifact != null && LostArtifacts.Settings.unlocked[artifact.ID()])
			{
				gameObject.GetComponent<Image>().sprite = artifact.sprite.Value;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.empty;
			}

			pos = gameObject.transform.localPosition;

			//Check if already equipped
			for(int i = 0; i < 4; i++)
			{
				if(LostArtifacts.Settings.slots[i] == id)
				{
					gameObject.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
					if(!equipped)
					{
						slot = ArtifactManager.Instance.slots[i];
						slot.button = this;
						slot.Equip();
						equipped = true;
					}
					break;
				}
			}
		}

		protected override void OnEnable()
		{
			if(artifact != null && LostArtifacts.Settings.unlocked[id])
			{
				gameObject.GetComponent<Image>().sprite = artifact.sprite.Value;
			}
			else
			{
				gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.empty;
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
			ArtifactManager.Instance.SetSelected(gameObject);

			//Update artifact panel
			if(artifact != null && LostArtifacts.Settings.unlocked[id])
			{
				ArtifactManager.Instance.SetArtifactPanel(artifact);
			}
			else
			{
				ArtifactManager.Instance.SetArtifactPanel();
			}

			//Set navigation for left and right menu buttons and Schy's artifact
			if(id == 0 || id == 10)
			{
				Navigation nav = FindSelectableOnLeft().navigation;
				nav.selectOnRight = this;
				FindSelectableOnLeft().navigation = nav;
			}
			if(id == 9 || id == 19)
			{
				Navigation nav = FindSelectableOnRight().navigation;
				nav.selectOnLeft = this;
				FindSelectableOnRight().navigation = nav;
			}
			if(id >= 10 && id < 20 && LostArtifacts.Settings.unlocked[20])
			{
				Navigation nav = FindSelectableOnDown().navigation;
				nav.selectOnUp = this;
				FindSelectableOnDown().navigation = nav;
			}
		}

		public void Confirm()
		{
			if(artifact == null) return;
			if(!LostArtifacts.Settings.unlocked[artifact.ID()]) return;

			ArtifactAudio.Instance.Play(ArtifactAudio.Instance.submit);

			if(!equipped)
			{
				choosing = true;

				//Set default slot
				if(slot == null)
				{
					slot = ArtifactManager.Instance.slots[0];
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
			slot = ArtifactManager.Instance.selected.GetComponent<ArtifactSlot>();

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
				if(slot.id == ArtifactManager.Instance.overchargeNum) artifact.level++;
				artifact.Activate();
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