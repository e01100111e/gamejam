using DG.Tweening;
using Language.Lua;
using Lean.Touch;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.ChatMapper;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR;

namespace CatFight
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            //Singleton method
            if (Instance == null)
            {
                //First run, set the instance
                Instance = this;
                DontDestroyOnLoad(gameObject);

            }
            else if (Instance != this)
            {
                //Instance is not the same as the one we have, destroy old one, and reset to newest one
                Destroy(Instance.gameObject);
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }


        }


        [SerializeField] private GameSettings gameSettings;
        public GameSettings GameSettings { get { return gameSettings; } }

        public Party party;
        [HideInInspector] public List<Character> roster;

        public RectTransform partyRect;
        public RectTransform rosterRect;

        private bool draggingCharacterOnParty = false;
        private bool draggingCharacterOnRoster = false;

        public List<Transform> rosterSlotPositions = new();
        public List<Transform> partySlotPositions = new();

        public CharacterMenuUI selectedCharacter;
        public CharacterMenuUI draggingCharacter;

        private List<Character> allCats;
        public List<Character> AllCats { get { return GetAllCats(); } }

        private List<Card> allCards;
        public List<Card> AllCards { get { return GetAllCards(); } }

        private List<Adventure> allAdventures;
        public List<Adventure> AllAdventures { get { return GetAllAdventures(); } }

        public List<Adventure> GetAllAdventures()
        {
            List<Adventure> adventures = new List<Adventure>();
            foreach (var adventureData in gameSettings.AllAdventures)
            {
                Adventure adventure = new(adventureData);
                adventures.Add(adventure);
            }

            return adventures;
        }

        [ReadOnly]
        public Adventure currentAdventure;

        public TMP_Text treatAmountText;

        public Transform deckList;
        public GameObject deckView;
        public GameObject emptyDeckPopup;

        public Camera uiCamera;

        public GameObject confirmExitPanel;
        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;

        }

        private void UpdateTreatText(int treatAmount)
        {
            if (treatAmountText != null) { treatAmountText.text = treatAmount.ToString(); }
        }

        public int GetCurrentWaveReward()
        {
            return currentAdventure.waves[BattleManager.Instance.currentWaveIndex].wave.treatAward;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MenuScene")
            {
                LoadRoster();
                LoadRosterUI();
                UpdateTreatText(party.TreatAmount);

            }

            if (scene.name == "BattleScene")
            {
                string conversation = GameManager.Instance.currentAdventure.adventureStoryMoments[1];
                DialogueManager.StartConversation(conversation);
                if (BattleManager.Instance.currentWaveIndex < 0)
                {
                    BattleManager.Instance.currentWaveIndex = 0;
                }
            }
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            GameManager.Instance.party.TreatAmountUpdated -= UpdateTreatText;
        }

        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

        public List<Card> GetAllCards()
        {
            List<Card> cards = new List<Card>();
            foreach (var cardData in gameSettings.AllCards)
            {
                Card card = new(cardData);
                cards.Add(card);
            }

            return cards;
        }

        public List<Character> GetAllCats()
        {
            List<Character> cats = new List<Character>();
            foreach (var catData in gameSettings.AllCats)
            {
                Character card = new(catData);
                cats.Add(card);
            }

            return cats;
        }

        private void Start()
        {
            if (DialogueLua.GetVariable("Adventure1Completed").AsBool == false)
            {
                DialogueManager.StartConversation("Arnold's Story Set Up");
            }
        }
        private void Update()
        {

            //if (TouchManager.Instance.IsDragging && selectedCharacter != null)
            //{
            //    draggingCharacter = selectedCharacter;
            //    Debug.Log("character selected");
            //    Vector2 touchPos = TouchManager.Instance.GetTouchPosition();

            //    if (RectTransformUtility.RectangleContainsScreenPoint(partyRect, touchPos, uiCamera))
            //    {
            //        draggingCharacterOnParty = true;
            //        draggingCharacterOnRoster = false;
            //        Debug.Log("dragging to party");


            //    }
            //    else if (RectTransformUtility.RectangleContainsScreenPoint(rosterRect, touchPos, uiCamera))
            //    {
            //        draggingCharacterOnParty = false;
            //        draggingCharacterOnRoster = true;
            //        Debug.Log("dragging to roster");

            //    }
            //}

            //if (!TouchManager.Instance.IsTouching && draggingCharacter != null)
            //{
            //    Debug.Log("Released the dragging character");

            //    if (draggingCharacterOnParty)
            //    {
            //        SnapToPartyPosition(draggingCharacter);
            //    }
            //    else if (draggingCharacterOnRoster)
            //    {
            //        SnapToRosterPosition(draggingCharacter);
            //    }
            //    else
            //    {
            //        SnapToRosterPosition(draggingCharacter);
            //    }

            //}

        }

        public void SnapToPartyPosition(CharacterMenuUI snapCharacter)
        {
            for (int i = 0; i < partySlotPositions.Count; i++)
            {
                if (partySlotPositions[i].childCount == 0 || partySlotPositions[i].GetChild(0) == null)
                {
                    snapCharacter.transform.SetParent(partySlotPositions[i].transform, false);
                    snapCharacter.transform.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.3f, snapping: true);
                    if (party.Cats.Contains(snapCharacter.character) == false)
                    {
                        party.AddCat(snapCharacter.character);
                    }

                    if (roster.Contains(snapCharacter.character))
                    {
                        roster.Remove(snapCharacter.character);
                    }
                    draggingCharacter = null;
                }

            }
        }

        public void SnapToRosterPosition(CharacterMenuUI snapCharacter)
        {
            for (int i = 0; i < rosterSlotPositions.Count; i++)
            {
                if (rosterSlotPositions[i].childCount == 0 || rosterSlotPositions[i].GetChild(0) == null)
                {
                    snapCharacter.transform.SetParent(rosterSlotPositions[i].transform, false);
                    snapCharacter.transform.GetComponent<RectTransform>().DOAnchorPos(Vector2.zero, 0.3f, snapping: true);
                    StretchTransform(snapCharacter.transform.GetComponent<RectTransform>());
                    if (roster.Contains(snapCharacter.character) == false)
                    {
                        roster.Add(snapCharacter.character);
                    }

                    if (party.Cats.Contains(snapCharacter.character))
                    {
                        party.Cats.Remove(snapCharacter.character);
                    }
                    draggingCharacter = null;

                }
            }
        }

        public void LoadBackToMainScene()
        {
            SceneManager.LoadScene("MenuScene");

        }

        public void LoadRoster()
        {
            roster = new();
            for (int i = 0; i < AllAdventures.Count; i++)
            {
                int adventureNumber = AllAdventures[i].adventureNumber;
                if (DialogueLua.GetVariable("Adventure" + adventureNumber.ToString() + "Completed").AsBool == true)
                {
                    Character cat = AllCats.Where(x => x.CharacterName == AllAdventures[i].catToRescue.CharacterName).FirstOrDefault();
                    if (cat != null)
                    {
                        roster.Add(cat);
                    }
                }
            }

            AllCats.Where(x => x.isDefaultCharacter == true).ToList().ForEach(x => roster.Add(x));
        }

        public void LoadRosterUI()
        {
            for (int i = 0; i < roster.Count; i++)
            {
                CharacterMenuUI characterUI = Instantiate(Instance.GameSettings.CharacterMenuUIPrefab);
                characterUI.Initialize(roster[i]);
                characterUI.transform.SetParent(rosterRect.transform, false);
                characterUI.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                characterUI.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                characterUI.GetComponent<LeanDragTranslate>().Camera = uiCamera;
                SnapToRosterPosition(characterUI);
            }
        }

        private void StretchTransform(RectTransform rectTransform)
        {

            // Set the anchors to stretch to all sides of the parent RectTransform
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);

            // Set the pivot to the center of the RectTransform
            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            // Set the offset to zero to ensure the RectTransform is centered
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public void GainTraits(int treats)
        {
            party.TreatAmount += treats;
        }

        public void Spendtreats(int treats)
        {
            party.TreatAmount -= treats;
        }

        public void LoadDeckCards()
        {
            deckView.SetActive(true);
            if(party.Deck.Cards.Count == 0)
            {
                deckList.gameObject.SetActive(false);
                emptyDeckPopup.gameObject.SetActive(true);
            }
            else
            {
                deckList.gameObject.SetActive(true);
                emptyDeckPopup.gameObject.SetActive(false);

                foreach (Transform card in deckList)
                {
                    Destroy(card.gameObject);
                }
                for (int i = 0; i < party.Deck.Cards.Count; i++)
                {
                    CardUI newCardUI = Instantiate(GameManager.Instance.GameSettings.CardPrefab);
                    newCardUI.Initialize(party.Deck.Cards[i]);
                    newCardUI.transform.SetParent(deckList, false);
                    newCardUI.transform.GetComponentInChildren<LeanSelectableByFinger>().enabled = false;
                }
            }
        

        }

        public void CloseDeckList()
        {
            deckView.SetActive(false);
        }

        public void OpenConfirmExit()
        {
            confirmExitPanel.SetActive(true);
            AdventureManager.Instance.fadeBG.gameObject.SetActive(true);
        }

        public void CloseConfirmExit()
        {
            confirmExitPanel.SetActive(false);
            AdventureManager.Instance.fadeBG.gameObject.SetActive(false);
        }

        public void CloseApp()
        {
            Application.Quit();
        }

        
    }


}
