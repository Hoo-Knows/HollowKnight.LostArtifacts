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

		private bool jumpWasPressed;
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
			if(LostArtifacts.Settings.unlocked[20])
			{
				for(int i = 10; i < 20; i++)
				{
					ArtifactButton button = parent.Find("Canvas/Artifacts/Artifact " + i).gameObject.GetComponent<ArtifactButton>();
					Navigation nav = button.navigation;
					nav.selectOnDown = schyButton;
					button.navigation = nav;
				}
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
			upWasPressed = InputHandler.Instance.inputActions.up.IsPressed;
			downWasPressed = InputHandler.Instance.inputActions.down.IsPressed;
			leftWasPressed = InputHandler.Instance.inputActions.left.IsPressed;
			rightWasPressed = InputHandler.Instance.inputActions.right.IsPressed;
		}
	}
}