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

		public ArtifactSlot slotHandle;
		public ArtifactSlot slotBladeL;
		public ArtifactSlot slotBladeR;
		public ArtifactSlot slotHead;

		public Image traitOvercharge;
		public Image slotOvercharge;
		public int overchargeNum;
		public bool canOvercharge;

		public EventSystem eventSystem;

		private Text artifactName;
		private Image artifactSprite;
		private Text artifactDescription;
		private Text levelsTitle;
		private Text artifactLevels;

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

			slotHandle = parent.Find("Canvas/Nail Panel/Slot Handle/Artifact Handle").gameObject.GetComponent<ArtifactSlot>();
			slotBladeL = parent.Find("Canvas/Nail Panel/Slot Blade L/Artifact Blade L").gameObject.GetComponent<ArtifactSlot>();
			slotBladeR = parent.Find("Canvas/Nail Panel/Slot Blade R/Artifact Blade R").gameObject.GetComponent<ArtifactSlot>();
			slotHead = parent.Find("Canvas/Nail Panel/Slot Head/Artifact Head").gameObject.GetComponent<ArtifactSlot>();

			traitOvercharge = parent.Find("Canvas/Trait Overcharge").gameObject.GetComponent<Image>();
			slotOvercharge = parent.Find("Canvas/Slot Overcharge").gameObject.GetComponent<Image>();
			traitOverchargeSprite = traitOvercharge.sprite;
			slotOverchargeSprite = slotOvercharge.sprite;

			eventSystem = parent.Find("EventSystem").gameObject.GetComponent<EventSystem>();

			artifactName = parent.Find("Canvas/Artifact Panel/Artifact Name").gameObject.GetComponent<Text>();
			artifactSprite = parent.Find("Canvas/Artifact Panel/Artifact Sprite").gameObject.GetComponent<Image>();
			artifactDescription = parent.Find("Canvas/Artifact Panel/Artifact Description").gameObject.GetComponent<Text>();
			levelsTitle = parent.Find("Canvas/Artifact Panel/Levels Title").gameObject.GetComponent<Text>();
			artifactLevels = parent.Find("Canvas/Artifact Panel/Artifact Levels").gameObject.GetComponent<Text>();

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
			LostArtifacts la = LostArtifacts.Instance;
			if(la.artifacts[selectedButton.id] != null && LostArtifacts.Settings.unlocked[selectedButton.id])
			{
				SetArtifactPanel(la.artifacts[selectedButton.id]);
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
			canOvercharge = PlayerData.instance.GetInt(nameof(PlayerData.nailSmithUpgrades)) == 4;
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
			levelsTitle.text = "Levels";
			artifactLevels.text = artifact.LevelInfo();
		}

		public void SetArtifactPanel()
		{
			artifactName.text = "";
			artifactSprite.sprite = empty;
			artifactDescription.text = "";
			levelsTitle.text = "";
			artifactLevels.text = "";
		}

		public void SetOvercharge()
		{
			LostArtifacts.Instance.Log("Overcharging slot " + overchargeNum);
			if(overchargeNum == 0)
			{
				traitOvercharge.transform.position = GameObject.Find("Handle Name").transform.position + new Vector3(30f, 0f, 0f);
				slotOvercharge.transform.position = slotHandle.transform.position;
				if(slotHandle.button != null)
				{
					slotHandle.button.artifact.Deactivate();
					slotHandle.button.artifact.level = 2;
					slotHandle.button.artifact.Activate();
				}
			}
			if(overchargeNum == 1)
			{
				traitOvercharge.transform.position = GameObject.Find("Blade L Name").transform.position + new Vector3(30f, 0f, 0f);
				slotOvercharge.transform.position = slotBladeL.transform.position;
				if(slotBladeL.button != null)
				{
					slotBladeL.button.artifact.Deactivate();
					slotBladeL.button.artifact.level = 3;
					slotBladeL.button.artifact.Activate();
				}
			}
			if(overchargeNum == 2)
			{
				traitOvercharge.transform.position = GameObject.Find("Blade R Name").transform.position + new Vector3(30f, 0f, 0f);
				slotOvercharge.transform.position = slotBladeR.transform.position;
				if(slotBladeR.button != null)
				{
					slotBladeR.button.artifact.Deactivate();
					slotBladeR.button.artifact.level = 3;
					slotBladeR.button.artifact.Activate();
				}
			}
			if(overchargeNum == 3)
			{
				traitOvercharge.transform.position = GameObject.Find("Head Name").transform.position + new Vector3(30f, 0f, 0f);
				slotOvercharge.transform.position = slotHead.transform.position;
				if(slotHead.button != null)
				{
					slotHead.button.artifact.Deactivate();
					slotHead.button.artifact.level = 4;
					slotHead.button.artifact.Activate();
				}
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
				LostArtifacts.Instance.Log("De-overcharging slot " + overchargeNum);
				if(overchargeNum == 0 && slotHandle.button != null)
				{
					slotHandle.button.artifact.Deactivate();
					slotHandle.button.artifact.level = 1;
					slotHandle.button.artifact.Activate();
				}
				if(overchargeNum == 1 && slotBladeL.button != null)
				{
					slotBladeL.button.artifact.Deactivate();
					slotBladeL.button.artifact.level = 2;
					slotBladeL.button.artifact.Activate();
				}
				if(overchargeNum == 2 && slotBladeR.button != null)
				{
					slotBladeR.button.artifact.Deactivate();
					slotBladeR.button.artifact.level = 2;
					slotBladeR.button.artifact.Activate();
				}
				if(overchargeNum == 3 && slotHead.button != null)
				{
					slotHead.button.artifact.Deactivate();
					slotHead.button.artifact.level = 3;
					slotHead.button.artifact.Activate();
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