using Modding;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifacts.UI
{
	public class ArtifactManager : MonoBehaviour
	{
		public static ArtifactManager Instance;
		public Transform parent;

		public ArtifactButton defaultButton;
		public ArtifactButton selectedButton;
		public ArtifactButton schyButton;
		public GameObject selected;

		public ArtifactSlot[] slots;

		public Image traitOvercharge;
		public Image slotOvercharge;
		public int overchargeNum;
		public bool canOvercharge;

		public EventSystem eventSystem;

		private Text artifactName;
		private Image artifactSprite;
		private Text artifactDescription;

		public Sprite[] nailSprites;

		public Sprite empty;
		public Sprite locked;
		public Sprite cursor;
		public Sprite traitOverchargeSprite;
		public Sprite slotOverchargeSprite;

		private bool jumpWasPressed;
		private bool attackWasPressed;
		private bool upWasPressed;
		private bool downWasPressed;
		private bool leftWasPressed;
		private bool rightWasPressed;

		private void Awake()
		{
			Instance = this;
			parent = gameObject.transform.parent;

			defaultButton = parent.Find("Canvas/Artifacts/Artifact 0").gameObject.GetComponent<ArtifactButton>();
			selectedButton = defaultButton;
			schyButton = parent.Find("Canvas/Artifacts/Artifact 20").gameObject.GetComponent<ArtifactButton>();
			selected = selectedButton.gameObject;

			slots = new ArtifactSlot[4];
			slots[0] = parent.Find("Canvas/Nail Panel/Slot Handle/Artifact Handle").gameObject.GetComponent<ArtifactSlot>();
			slots[1] = parent.Find("Canvas/Nail Panel/Slot Blade L/Artifact Blade L").gameObject.GetComponent<ArtifactSlot>();
			slots[2] = parent.Find("Canvas/Nail Panel/Slot Blade R/Artifact Blade R").gameObject.GetComponent<ArtifactSlot>();
			slots[3] = parent.Find("Canvas/Nail Panel/Slot Head/Artifact Head").gameObject.GetComponent<ArtifactSlot>();

			traitOvercharge = parent.Find("Canvas/Trait Overcharge").gameObject.GetComponent<Image>();
			slotOvercharge = parent.Find("Canvas/Slot Overcharge").gameObject.GetComponent<Image>();
			traitOverchargeSprite = traitOvercharge.sprite;
			slotOverchargeSprite = slotOvercharge.sprite;

			eventSystem = parent.Find("EventSystem").gameObject.GetComponent<EventSystem>();

			artifactName = parent.Find("Canvas/Artifact Panel/Artifact Name").gameObject.GetComponent<Text>();
			artifactSprite = parent.Find("Canvas/Artifact Panel/Artifact Sprite").gameObject.GetComponent<Image>();
			artifactDescription = parent.Find("Canvas/Artifact Panel/Artifact Description").gameObject.GetComponent<Text>();

			foreach(Text text in parent.gameObject.GetComponentsInChildren<Text>())
			{
				if(text.gameObject.name == "Artifact Title" ||
					text.gameObject.name == "Traits Title" ||
					text.gameObject.name == "Nail Title")
				{
					text.font = CanvasUtil.TrajanNormal;
				}
				else
				{
					text.font = CanvasUtil.GetFont("Perpetua");
				}
			}
		}

		private void OnEnable()
		{
			//Set selected
			selected = selectedButton.gameObject;
			eventSystem.SetSelectedGameObject(selected);
			selectedButton.Select();

			//Update UI
			if(LostArtifacts.Instance.artifacts[selectedButton.id] != null && LostArtifacts.Settings.unlocked[selectedButton.id])
			{
				SetArtifactPanel(LostArtifacts.Instance.artifacts[selectedButton.id]);
			}
			else
			{
				SetArtifactPanel();
			}

			//Update button movement for Hidden Memento
			for(int i = 10; i < 20; i++)
			{
				ArtifactButton button = parent.Find("Canvas/Artifacts/Artifact " + i).gameObject.GetComponent<ArtifactButton>();
				Navigation nav = button.navigation;
				if(LostArtifacts.Settings.unlocked[20]) nav.selectOnDown = schyButton;
				else nav.selectOnDown = null;
				button.navigation = nav;
			}
			
			//Update Overcharge
			canOvercharge = PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)) == 4 || LostArtifacts.Settings.unlockedSlots;
			if(canOvercharge)
			{
				if(LostArtifacts.Settings.overchargedSlot == -1) LostArtifacts.Settings.overchargedSlot = 0;
				overchargeNum = LostArtifacts.Settings.overchargedSlot;
				traitOvercharge.sprite = traitOverchargeSprite;
				slotOvercharge.sprite = slotOverchargeSprite;
				SetOvercharge();
			}
			else
			{
				overchargeNum = -1;
				traitOvercharge.sprite = empty;
				slotOvercharge.sprite = empty;
			}
		}

		public void SetSelected(GameObject selected)
		{
			this.selected = selected;
			eventSystem.SetSelectedGameObject(selected);
			ArtifactCursor.Instance.UpdatePos();
			ArtifactAudio.Instance.Play(ArtifactAudio.Instance.select);
			if(selected.GetComponent<ArtifactButton>() != null)
			{
				selectedButton = selected.GetComponent<ArtifactButton>();
			}
		}

		public void SetArtifactPanel(Artifact artifact)
		{
			artifactName.text = artifact.Name();
			artifactSprite.sprite = artifact.sprite.Value;
			artifactDescription.text = artifact.Description();
		}

		public void SetArtifactPanel()
		{
			artifactName.text = "";
			artifactSprite.sprite = empty;
			artifactDescription.text = "";
		}

		public void SetOvercharge()
		{
			LostArtifacts.Instance.LogDebug("Overcharging slot " + overchargeNum);
			string[] names = { "Handle", "Blade L", "Blade R", "Head" };
			traitOvercharge.transform.position = GameObject.Find(names[overchargeNum] + " Name").transform.position + new Vector3(30f, 0f, 0f);
			slotOvercharge.transform.position = slots[overchargeNum].transform.position;
			if(slots[overchargeNum].button != null)
			{
				slots[overchargeNum].button.artifact.Deactivate();
				slots[overchargeNum].button.artifact.level = slots[overchargeNum].level + 1;
				slots[overchargeNum].button.artifact.Activate();
				slots[overchargeNum].SetTraits();
			}
		}

		private void Update()
		{
			//Handle UI input
			if(InputHandler.Instance.inputActions.jump.IsPressed && !jumpWasPressed)
			{
				if(selected.GetComponent<ArtifactButton>() != null)
				{
					selected.GetComponent<ArtifactButton>().Confirm();
				}
				else if(selected.GetComponent<ArtifactSlot>() != null)
				{
					selected.GetComponent<ArtifactSlot>().Confirm();
				}
				else if(selected.GetComponent<ArrowButton>() != null)
				{
					selected.GetComponent<ArrowButton>().Confirm();
				}
			}
			if(InputHandler.Instance.inputActions.attack.IsPressed && !attackWasPressed && canOvercharge)
			{
				LostArtifacts.Instance.LogDebug("De-overcharging slot " + overchargeNum);
				foreach(ArtifactSlot slot in slots)
				{
					if(slot.id == overchargeNum && slot.button != null)
					{
						slot.button.artifact.Deactivate();
						slot.button.artifact.level = slot.level;
						slot.button.artifact.Activate();
						slot.SetTraits();
					}
				}

				overchargeNum++;
				if(overchargeNum > 3) overchargeNum = 0;
				LostArtifacts.Settings.overchargedSlot = overchargeNum;

				SetOvercharge();
			}
			if(InputHandler.Instance.inputActions.up.IsPressed && !upWasPressed &&
				selected.GetComponent<Button>().FindSelectableOnUp() != null)
			{
				eventSystem.SetSelectedGameObject(selected.GetComponent<Button>().FindSelectableOnUp().gameObject);
			}
			if(InputHandler.Instance.inputActions.down.IsPressed && !downWasPressed &&
				selected.GetComponent<Button>().FindSelectableOnDown() != null)
			{
				eventSystem.SetSelectedGameObject(selected.GetComponent<Button>().FindSelectableOnDown().gameObject);
			}
			if(InputHandler.Instance.inputActions.left.IsPressed && !leftWasPressed &&
				selected.GetComponent<Button>().FindSelectableOnLeft() != null)
			{
				eventSystem.SetSelectedGameObject(selected.GetComponent<Button>().FindSelectableOnLeft().gameObject);
			}
			if(InputHandler.Instance.inputActions.right.IsPressed && !rightWasPressed &&
				selected.GetComponent<Button>().FindSelectableOnRight() != null)
			{
				eventSystem.SetSelectedGameObject(selected.GetComponent<Button>().FindSelectableOnRight().gameObject);
			}

			jumpWasPressed = InputHandler.Instance.inputActions.jump.IsPressed;
			attackWasPressed = InputHandler.Instance.inputActions.attack.IsPressed;
			upWasPressed = InputHandler.Instance.inputActions.up.IsPressed;
			downWasPressed = InputHandler.Instance.inputActions.down.IsPressed;
			leftWasPressed = InputHandler.Instance.inputActions.left.IsPressed;
			rightWasPressed = InputHandler.Instance.inputActions.right.IsPressed;
		}
	}
}