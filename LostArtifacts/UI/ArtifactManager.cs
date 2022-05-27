using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifactsUI
{
    public class ArtifactManager : MonoBehaviour
    {
        public static ArtifactManager Instance;
        public LostArtifacts.Artifact[] artifacts;
        private Transform parent;

        public ArtifactButton defaultButton;
        public ArtifactButton selectedButton;
        public ArtifactSlot defaultSlot;

        private EventSystem prevEventSystem;
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
            artifacts = new LostArtifacts.Artifact[20];
            parent = gameObject.transform.parent;

            defaultButton = parent.Find("Canvas/Artifacts/Artifact 0").gameObject.GetComponent<ArtifactButton>();
            selectedButton = defaultButton;

            defaultSlot = parent.Find("Canvas/Nail Panel/Slot Handle/Artifact Handle").gameObject.GetComponent<ArtifactSlot>();

            eventSystem = parent.Find("EventSystem").gameObject.GetComponent<EventSystem>();

            artifactName = parent.Find("Canvas/Artifact Panel/Artifact Name").gameObject.GetComponent<Text>();
            artifactSprite = parent.Find("Canvas/Artifact Panel/Artifact Sprite").gameObject.GetComponent<Image>();
            artifactDescription = parent.Find("Canvas/Artifact Panel/Artifact Description").gameObject.GetComponent<Text>();
        }

        public void OnEnable()
		{
            //Set selected stuff
            prevEventSystem = EventSystem.current;
            EventSystem.current = eventSystem;
			eventSystem.SetSelectedGameObject(defaultButton.gameObject);

            //Update UI
            ArtifactCursor.Instance.UpdatePos();
            if(artifacts[0] != null && artifacts[0].unlocked)
            {
                SetArtifactPanel(artifacts[0].name, artifacts[0].sprite, artifacts[0].description);
            }
            else
            {
                SetArtifactPanel("", empty, "");
            }
        }

        public void OnDisable()
		{
            EventSystem.current = prevEventSystem;
			eventSystem.SetSelectedGameObject(null);
			ArtifactCursor.Instance.UpdatePos();
        }

        public void Confirm()
        {
            GameObject selected = eventSystem.currentSelectedGameObject;
            if(selected.GetComponent<ArtifactButton>() != null)
            {
                selected.GetComponent<ArtifactButton>().Confirm();
            }
            if(selected.GetComponent<ArtifactSlot>() != null)
            {
                selected.GetComponent<ArtifactSlot>().Confirm();
            }
            if(selected.GetComponent<ArrowButton>() != null)
            {
                selected.GetComponent<ArrowButton>().Confirm();
            }
        }

        public void AddArtifact<T>() where T : LostArtifacts.Artifact
        {
            LostArtifacts.Artifact artifact = gameObject.AddComponent<T>();
            artifact.Initialize();
            artifact.sprite = LostArtifacts.LostArtifacts.Instance.GetArtifactSprite(artifact.name);
            artifacts[artifact.id] = artifact;
        }

        public void ToggleArtifact(int id)
        {
            artifacts[id].unlocked = !artifacts[id].unlocked;
        }

        public void SetArtifactPanel(string name, Sprite sprite, string description)
        {
            artifactName.text = name;
            artifactSprite.sprite = sprite;
            artifactDescription.text = description;
        }

        public void Left()
        {
            LostArtifacts.LostArtifacts.Instance.pageFSM.SendEvent("LEFT");
        }

        public void Right()
        {
            LostArtifacts.LostArtifacts.Instance.pageFSM.SendEvent("RIGHT");
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                Confirm();
            }
        }
    }
}