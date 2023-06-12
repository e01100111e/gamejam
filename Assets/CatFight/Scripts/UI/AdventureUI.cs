using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatFight
{
    public class AdventureUI : MonoBehaviour
    {

        public Transform lockedPanel;
        public Transform lockedIcon;

        public TMP_Text displayText;
        public AdventureData adventure;
        public Image adventureGoalImage;
        public Button button;

        private void Update()
        {

        }

        // Start is called before the first frame update
        public void Start()
        {
            if (adventure.StartingAdventure == true && DialogueLua.GetVariable("Adventure" + (adventure.AdventureNumber).ToString() + "Completed").AsBool == false)
            {
                UnlockAdventure();
                return;
            }


            else if (adventure.StartingAdventure == true && DialogueLua.GetVariable("Adventure" + (adventure.AdventureNumber).ToString() + "Completed").AsBool == true)
            {
                UnlockAdventure();
                button.interactable = false;
                displayText.text = adventure.CatToRescue.CharacterName + " RESCUED!";
                return;
            }

            if ((adventure.AdventureNumber == 3 || adventure.AdventureNumber == 2) && DialogueLua.GetVariable("Adventure1Completed").AsBool == true)
            {
                UnlockAdventure();
            }

            else if (DialogueLua.GetVariable("Adventure" + (adventure.AdventureNumber - 1).ToString() + "Completed").AsBool == true)
            {
                UnlockAdventure();
            }
            else
            {
                LockAdventure();
            }

            if (DialogueLua.GetVariable("Adventure" + (adventure.AdventureNumber).ToString() + "Completed").AsBool == true)
            {
                UnlockAdventure();
                button.interactable = false;
                displayText.text = adventure.CatToRescue.CharacterName + " RESCUED!";
            }

        }

        void LockAdventure()
        {
            lockedPanel.gameObject.SetActive(true);
            lockedIcon.gameObject.SetActive(true);
            adventureGoalImage.gameObject.SetActive(false);
            button.interactable = false;
        }

        void UnlockAdventure()
        {
            lockedPanel.gameObject.SetActive(false);
            lockedIcon.gameObject.SetActive(false);
            adventureGoalImage.gameObject.SetActive(true);
            button.interactable = true;
        }
    }
}
