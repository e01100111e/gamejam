using Lean.Touch;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatFight
{
    public class AdventureManager : MonoBehaviour
    {
        public static AdventureManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        public Adventure CurrentAdventure;
        public WaveData currentLevel;

        public Image characterPicture;
        public TMP_Text characterName;

        public TMP_Text characterStats;

        public TMP_Text adventureDetails;
        public List<CardUI> CharacterCards;

        public Transform adventureDetailPopup;

        public Transform warningPopup;

        public Transform fadeBG;

        public Transform cardList;
        public TMP_Text cardListTitle;


        private void Start()
        {
            CloseLevelDetails();
        }
        public void LoadAdventureDetails(AdventureData adventureData)
        {
            if (GameManager.Instance.party.Cats.Count == 0)
            {
                fadeBG.gameObject.SetActive(true);
                warningPopup.gameObject.SetActive(true);
                return;
            }

            CurrentAdventure = new Adventure(adventureData);
            characterPicture.sprite = CurrentAdventure.catToRescue.CharacterSprite;
            characterName.text = CurrentAdventure.catToRescue.CharacterName;
            adventureDetails.text = DialogueManager.instance.MasterDatabase.GetConversation(CurrentAdventure.adventureStoryMoments[0]).dialogueEntries[1].subtitleText;
            fadeBG.gameObject.SetActive(true);
            adventureDetailPopup.gameObject.SetActive(true);
            GameManager.Instance.currentAdventure = CurrentAdventure;
            cardListTitle.text = CurrentAdventure.catToRescue.CharacterName + "'s Cards";


            characterStats.text = "Health: " + CurrentAdventure.catToRescue.Health + "<br>" + "Defence: " + CurrentAdventure.catToRescue.StartingDefence;

            foreach (Transform card in cardList)
            {
                Destroy(card.gameObject);
            }
            for (int i = 0; i < CurrentAdventure.catToRescue.CardList.Count; i++)
            {

                Card newCard = new Card(CurrentAdventure.catToRescue.CardList[i]);
                CardUI newCardUI = Instantiate(GameManager.Instance.GameSettings.CardPrefab);
                newCardUI.Initialize(newCard);
                newCardUI.transform.SetParent(cardList, false);
                newCardUI.transform.GetComponentInChildren<LeanSelectableByFinger>().enabled = false;
            }
        }
        public void CloseLevelDetails()
        {
            fadeBG.gameObject.SetActive(false);
            adventureDetailPopup.gameObject.SetActive(false);
        }

        public void CloseWarning()
        {
            fadeBG.gameObject.SetActive(false);
            warningPopup.gameObject.SetActive(false);
        }
    }
}
