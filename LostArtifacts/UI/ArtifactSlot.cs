using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LostArtifactsUI
{
    public class ArtifactSlot : Button
    {
        public ArtifactButton button;
        public bool unlocked;

        private int id;
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
            if(button != null) sprite = button.artifact.sprite;
            else sprite = ArtifactManager.Instance.empty;

            unlocked = ArtifactManager.Instance.nailLevel > id;
            gameObject.GetComponent<Image>().sprite = unlocked ? sprite : ArtifactManager.Instance.locked;

            ResetTraits();
        }

        public override void OnSelect(BaseEventData eventData)
        {
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
                iTween.ShakePosition(ArtifactCursor.Instance.gameObject, Vector3.one * 0.7f, 0.3f);
            }
        }

        public void Equip()
        {
            //Set sprite
            gameObject.GetComponent<Image>().sprite = button.artifact.sprite;

            //Update trait panel
            GameObject.Find(names[id] + " Name").GetComponent<Text>().text =
                names[id] + " - " + button.artifact.traitName + " " + level[id];
            GameObject.Find(names[id] + " Description").GetComponent<Text>().text = button.artifact.traitDescription;

            equipped = true;
        }

        public void Unequip()
        {
            //Reset sprite
            gameObject.GetComponent<Image>().sprite = ArtifactManager.Instance.empty;

            //Update trait panel
            ResetTraits();

            //Reset artifactGO
            button = null;

            equipped = false;
        }

        private void ResetTraits()
        {
            GameObject.Find(names[id] + " Name").GetComponent<Text>().text = names[id] + " - " + (unlocked ? "None" : "Locked");
            GameObject.Find(names[id] + " Description").GetComponent<Text>().text = "";
        }
    }
}