using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace LostArtifacts.UI
{
    public class ArtifactManager : MonoBehaviour
    {
        public static ArtifactManager Instance;
        public Transform parent;

        public ArtifactButton defaultButton;
        public ArtifactButton selectedButton;
        public GameObject selected;

        public ArtifactSlot slotHandle;
        public ArtifactSlot slotBladeL;
        public ArtifactSlot slotBladeR;
        public ArtifactSlot slotHead;

		public EventSystem eventSystem;

		public Text artifactName;
        public Image artifactSprite;
        public Text artifactDescription;

        public int nailLevel;
        public Sprite[] nailSprites;

        public Sprite empty;
        public Sprite locked;
        public Sprite cursor;

        private void Awake()
        {
            Instance = this;
            parent = gameObject.transform.parent;

            defaultButton = parent.Find("Canvas/Artifacts/Artifact 0").gameObject.GetComponent<ArtifactButton>();
            selectedButton = defaultButton;
            selected = selectedButton.gameObject;

            slotHandle = parent.Find("Canvas/Nail Panel/Slot Handle/Artifact Handle").gameObject.GetComponent<ArtifactSlot>();
            slotBladeL = parent.Find("Canvas/Nail Panel/Slot Blade L/Artifact Blade L").gameObject.GetComponent<ArtifactSlot>();
            slotBladeR = parent.Find("Canvas/Nail Panel/Slot Blade R/Artifact Blade R").gameObject.GetComponent<ArtifactSlot>();
            slotHead = parent.Find("Canvas/Nail Panel/Slot Head/Artifact Head").gameObject.GetComponent<ArtifactSlot>();

            eventSystem = parent.Find("EventSystem").gameObject.GetComponent<EventSystem>();

            artifactName = parent.Find("Canvas/Artifact Panel/Artifact Name").gameObject.GetComponent<Text>();
            artifactSprite = parent.Find("Canvas/Artifact Panel/Artifact Sprite").gameObject.GetComponent<Image>();
            artifactDescription = parent.Find("Canvas/Artifact Panel/Artifact Description").gameObject.GetComponent<Text>();
        }

        public void OnEnable()
		{
            //Update nail level
            nailLevel = PlayerData.instance.GetInt("nailSmithUpgrades");

            //Set selected
            selected = selectedButton.gameObject;
            eventSystem.SetSelectedGameObject(selected);
            selectedButton.Select();

            //Update UI
            ArtifactCursor.Instance.UpdatePos();
            LostArtifacts la = LostArtifacts.Instance;
            if(la.artifacts[selectedButton.id] != null && la.artifacts[selectedButton.id].unlocked)
            {
                SetArtifactPanel(la.artifacts[selectedButton.id].Name(),
                    la.artifacts[selectedButton.id].sprite,
                    la.artifacts[selectedButton.id].Description());
            }
            else
            {
                SetArtifactPanel("", empty, "");
            }
        }

        public void OnDisable()
		{
			ArtifactCursor.Instance.UpdatePos();
        }

        public void SetArtifactPanel(string name, Sprite sprite, string description)
        {
            artifactName.text = name;
            artifactSprite.sprite = sprite;
            artifactDescription.text = description;
        }

        public void Update()
        {
            //Handle UI input
            if(GetKeyDown(InputHandler.Instance.inputActions.jump))
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
            if(GetKeyDown(InputHandler.Instance.inputActions.up) && selected.GetComponent<Button>().FindSelectableOnUp() != null)
            {
                selected = selected.GetComponent<Button>().FindSelectableOnUp().gameObject;
                eventSystem.SetSelectedGameObject(selected);
            }
            if(GetKeyDown(InputHandler.Instance.inputActions.down) && selected.GetComponent<Button>().FindSelectableOnDown() != null)
            {
                selected = selected.GetComponent<Button>().FindSelectableOnDown().gameObject;
                eventSystem.SetSelectedGameObject(selected);
            }
            if(GetKeyDown(InputHandler.Instance.inputActions.left) && selected.GetComponent<Button>().FindSelectableOnLeft() != null)
            {
                selected = selected.GetComponent<Button>().FindSelectableOnLeft().gameObject;
                eventSystem.SetSelectedGameObject(selected);
            }
            if(GetKeyDown(InputHandler.Instance.inputActions.right) && selected.GetComponent<Button>().FindSelectableOnRight() != null)
            {
                selected = selected.GetComponent<Button>().FindSelectableOnRight().gameObject;
                eventSystem.SetSelectedGameObject(selected);
            }
        }

        private bool GetKeyDown(InControl.PlayerAction key)
		{
            InputHandler.KeyOrMouseBinding binding = InputHandler.Instance.GetKeyBindingForAction(key);
            return Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), binding.Key.ToString()));
        }
    }
}