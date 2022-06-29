using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifacts.UI
{
	public class ArtifactSlot : Button
	{
		public ArtifactButton button;
		public bool unlocked;
		public int id;

		private Sprite sprite;
		private bool equipped;
		private readonly string[] names = new string[] { "Handle", "Blade L", "Blade R", "Head" };
		private readonly string[] level = new string[] { "I", "II", "II", "III" };

		protected override void Start()
		{
			switch(name)
			{
				case "Artifact Handle":
					id = 0;
					break;
				case "Artifact Blade L":
					id = 1;
					break;
				case "Artifact Blade R":
					id = 2;
					break;
				case "Artifact Head":
					id = 3;
					break;
			}
		}

		protected override void OnEnable()
		{
			if(button != null) sprite = button.artifact.sprite.Value;
			else sprite = ArtifactManager.Instance.empty;

			unlocked = PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)) > id;
			gameObject.GetComponent<Image>().sprite = unlocked ? sprite : ArtifactManager.Instance.locked;

			SetTraits();
		}

		public override void OnSelect(BaseEventData eventData)
		{
			//Set selected
			ArtifactManager.Instance.selected = gameObject;

			ArtifactAudio.Instance.Play(ArtifactAudio.Instance.select);

			ArtifactCursor.Instance.UpdatePos();
		}

		public void Confirm()
		{
			if(unlocked)
			{
				if(ArtifactManager.Instance.selectedButton == null) return;

				//Submit
				ArtifactAudio.Instance.Play(ArtifactAudio.Instance.submit);

				//Get rid of currently equipped
				if(equipped) button.Unequip();

				//Update equipped
				button = ArtifactManager.Instance.selectedButton;
				button.Equip();
				Equip();
			}
			else
			{
				//Rejected
				ArtifactAudio.Instance.Play(ArtifactAudio.Instance.reject);
			}
		}

		public void Equip()
		{
			//Set sprite
			gameObject.GetComponent<Image>().sprite = button.artifact.sprite.Value;

			//Update trait panel
			SetTraits();

			//Update settings
			if(id == 0) LostArtifacts.Settings.slotHandle = button.id;
			if(id == 1) LostArtifacts.Settings.slotBladeL = button.id;
			if(id == 2) LostArtifacts.Settings.slotBladeR = button.id;
			if(id == 3) LostArtifacts.Settings.slotHead = button.id;

			equipped = true;
		}

		public void Unequip()
		{
			//Reset sprite
			gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.empty;

			//Reset button
			button = null;

			//Update trait panel
			SetTraits();

			//Update settings
			if(id == 0) LostArtifacts.Settings.slotHandle = -1;
			if(id == 1) LostArtifacts.Settings.slotBladeL = -1;
			if(id == 2) LostArtifacts.Settings.slotBladeR = -1;
			if(id == 3) LostArtifacts.Settings.slotHead = -1;

			equipped = false;
		}

		private void SetTraits()
		{
			if(button != null)
			{
				GameObject.Find(names[id] + " Name").GetComponent<Text>().text =
					names[id] + " - " + button.artifact.TraitName() + " " + level[id];
				GameObject.Find(names[id] + " Description").GetComponent<Text>().text = button.artifact.TraitDescription();
			}
			else
			{
				GameObject.Find(names[id] + " Name").GetComponent<Text>().text = names[id] + " - " + (unlocked ? "None" : "Locked");
				GameObject.Find(names[id] + " Description").GetComponent<Text>().text = "";
			}
		}
	}
}