using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace CatFight
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance;
        private void Awake()
        {
            Instance = this;
            Character.OnCharacterDead += CheckWinOrLose;

        }

        public TMP_Text deckCardCountUI;

        [SerializeField] private int deckCardCount = 0;

        private int currentMana;
        private int maxMana;

        public static event Action<int> OnManaChange;
        public static event Action<int> OnMaxManaChange;
        public static event Action<Character> OnCharacterSelect;


        #region Event Triggers

        public static event Action<ReactionTrigger> OnReactionTrigger;
        public static event Action<CardAction> OnReactionExpired;
        #endregion

        public int CurrentMana
        {
            get { return currentMana; }
            set
            {
                currentMana = Mathf.Clamp(value, 0, maxMana);
                OnManaChange?.Invoke(currentMana);
            }
        }

        public int MaxMana
        {
            get { return maxMana; }
            set
            {
                maxMana = Mathf.Clamp(value, 0, GameManager.Instance.GameSettings.MaxManaPerBattle);
                OnMaxManaChange?.Invoke(maxMana);
            }
        }

        private List<Card> cards;

        [SerializeField] private List<Transform> allyWorldTransforms;
        [SerializeField] private List<Transform> enemyWorldTransforms;

        private List<Character> allies;
        private List<Character> enemies;

        public bool allEnemiesDead;
        public bool allAlliesDead;

        private WaveData level;

        private bool win = false;
        private bool lose = false;

        private bool isPlayerTurn;
        public bool IsPlayerTurn => isPlayerTurn;

        public bool handReady;
        public bool IsGameOn => !win && !lose;

        private bool doABigSneezeAvailable = false;
        private bool shredACardAvailable = false;

        private bool sitOnACardAvailable = false;
        //bool sitCardSelected = false;
        //bool sitOnACardWaiting = false;


        public bool aReactionTriggered = false;

        private CharacterUI selectedCharacter;
        private CharacterUI targetedCharacter;

        public SpriteRenderer background;
        public int DeckCardCount
        {
            get => deckCardCount;
            set
            {
                deckCardCount = value;
                deckCardCountUI.text = DeckCardCount.ToString();
            }
        }

        public bool DoABigSneezeAvailable { get => doABigSneezeAvailable; set => doABigSneezeAvailable = value; }
        public bool ShredACardAvailable { get => shredACardAvailable; set => shredACardAvailable = value; }
        public bool SitOnACardAvailable { get => sitOnACardAvailable; set => sitOnACardAvailable = value; }

        public List<Character> Allies => allies;
        public List<Character> Enemies => enemies;
        public CharacterUI SelectedCharacter
        {
            get => selectedCharacter;
            set
            {
                selectedCharacter = value;
            }
        }

        public CharacterUI TargetedCharacter
        {
            get => targetedCharacter;
            set
            {
                targetedCharacter = value;
            }
        }

        [ReadOnly]
        public bool IsEnemyTurn;

        [ReadOnly]
        public int currentWaveIndex = -1;
        [ReadOnly]
        private int lastDialogueIndex = 1;


        void OnDisable()
        {
            Character.OnCharacterDead -= CheckWinOrLose;
        }

        private void Start()
        {

        }

        private void Update()
        {
            if (IsGameOn)
            {

                if (allAlliesDead) { Lose(); }
                if (allEnemiesDead) { Win(); }
            }

            if (isPlayerTurn)
            {
                if (HandManager.Instance.playedACard)
                {
                    UIManager.Instance.ToggleSneezeButton(false);
                }

                else if (HandManager.Instance.playedACard == false && DoABigSneezeAvailable)
                {
                    UIManager.Instance.ToggleSneezeButton(true);
                }
            }



        }
        IEnumerator GameLoop()
        {
            while (IsGameOn)
            {
                if (IsPlayerTurn)
                {
                    // Player's turn
                    UIManager.Instance.SelectFirstAvailableAlly();
                    GainMana();
                    RefreshMana();
                    yield return StartCoroutine(PlayerTurn());
                }
                else
                {
                    // Enemy's turn
                    UIManager.Instance.DisableEndTurnButton();
                    if (!EnemyManager.Instance.enemyPlaying)
                    {
                        yield return StartCoroutine(EnemyManager.Instance.EnemyPlayingItsTurn());
                    }

                    isPlayerTurn = true;
                }
            }
        }

        IEnumerator PlayerTurn()
        {


            allies.Where(x => x.IsDead() == false).ToList().ForEach(x => x.UpdateStatusEffects());
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead() == false)
                    enemy.UpdateStatusEffects();
            }
            HandManager.Instance.FillHand();
            yield return new WaitUntil(() => HandManager.Instance.hand.Count == GameManager.Instance.GameSettings.MaxCardCountInHand);
            handReady = true;
            UIManager.Instance.EnableEndTurnButton();
            yield return new WaitUntil(() => IsPlayerTurn == false);
        }

        public void InitWaves()
        {
            ClearCharacters();
            LoadAllies();
            LoadEnemies();
        }

        public void LoadAllies()
        {
            allies = new List<Character>(GameManager.Instance.party.Cats);

            for (int i = 0; i < allies.Count; i++)
            {
                int positionIndex = i + 1;
                if (i + 1 >= allies.Count) positionIndex = 0;
                Transform catTransform = allyWorldTransforms[positionIndex];
                Instantiate(allies[i].characterPrefab, catTransform);
                allyWorldTransforms[positionIndex].gameObject.SetActive(true);
                UIManager.Instance.LoadCharacterUI(allies[i], positionIndex);
            }

        }

        public void LoadEnemies()
        {
            enemies = new List<Character>(GameManager.Instance.currentAdventure.waves[currentWaveIndex].wave.enemies);

            for (int i = 0; i < enemies.Count; i++)
            {
                int positionIndex = i + 1;
                if (i + 1 >= enemies.Count) positionIndex = 0;
                Transform ratTransform = enemyWorldTransforms[positionIndex];
                Instantiate(enemies[i].characterPrefab, ratTransform);
                enemyWorldTransforms[positionIndex].gameObject.SetActive(true);
                UIManager.Instance.LoadCharacterUI(enemies[i], positionIndex);
            }
            background.sprite = GameManager.Instance.currentAdventure.waves[currentWaveIndex].wave.background;
            EnemyManager.Instance.LoadEnemyHand();

            Debug.Log("Current Wave:" + currentWaveIndex.ToString());
        }

        public List<Character> GetTargets(Target target)
        {
            List<Character> targets = new();

            switch (target)
            {
                case Target.SingleAlly:
                    targets.Add(TargetedCharacter.CurrentCharacter);
                    break;
                case Target.SingleEnemy:
                    targets.Add(TargetedCharacter.CurrentCharacter);
                    break;
                case Target.AllCharacters:
                    targets = allies;
                    targets.AddRange(enemies);
                    break;
                case Target.None:
                    break;
                case Target.AllAllies:
                    targets = allies;
                    break;
                case Target.AllEnemies:
                    targets = enemies;
                    break;
                case Target.Self:
                    targets.Add(selectedCharacter.CurrentCharacter);
                    break;
                default:
                    targets = new List<Character>();
                    break;
            }

            return targets;
        }

        public Card DrawCardFromDeck()
        {
            if (cards.Count == 0)
            {
                DeckCardCount = 0;
                Debug.Log("Deck is empty");
                ShuffleDiscardPileIntoDeck();
            }
            int randomCard = Random.Range(0, cards.Count);
            DeckCardCount = cards.Count;
            Card card = cards[randomCard];
            cards.Remove(card);
            DeckCardCount = cards.Count;
            return card;
        }
        public void ShuffleDiscardPileIntoDeck()
        {
            cards.AddRange(HandManager.Instance.discardPile);
            ShuffleDeck();
            Debug.Log("Discard pile shuffled and made deck again");
        }

        public void ClearCharacters()
        {
            allyWorldTransforms.ForEach(x => x.gameObject.SetActive(false));
            enemyWorldTransforms.ForEach(x => x.gameObject.SetActive(false));

            foreach (Transform child in allyWorldTransforms)
            {
                if (child.childCount > 0)
                    Destroy(child.GetChild(0).gameObject);
            }

            foreach (Transform child in enemyWorldTransforms)
            {
                if (child.childCount > 0)
                    Destroy(child.GetChild(0).gameObject);
            }

            UIManager.Instance.ClearCharacters();

        }

        public void EndPlayerTurn()
        {
            isPlayerTurn = false;
            aReactionTriggered = false;
            handReady = false;
            allies.ForEach(x => x.ClearExpiredReactions());
            EnemyManager.Instance.CreateRandomOrderQueue();
        }

        public void ShuffleDeck()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int r = Random.Range(0, i);
                (cards[r], cards[i]) = (cards[i], cards[r]);
            }

        }

        #region Mana
        public void SpendMana(int manaCost)
        {

            CurrentMana -= manaCost;
            Debug.Log("CurrentMana: " + CurrentMana);
        }

        public void GainMana()
        {
            MaxMana += GameManager.Instance.GameSettings.DefaultManaPerTurn;
            Debug.Log("GAINED MANA??:" + MaxMana);
        }

        public void RefreshMana()
        {
            CurrentMana = MaxMana;
        }

        #endregion

        #region EndGame
        public void CheckWinOrLose(CharacterType characterType)
        {
            if (characterType == CharacterType.Ally)
            {
                allAlliesDead = allies.All(x => x.CurrentHealth == 0);
            }

            if (characterType == CharacterType.Enemy)
            {
                allEnemiesDead = enemies.All(x => x.CurrentHealth == 0);
            }

        }
        public void Win()
        {
            win = true;
            UIManager.Instance.ShowWinScreen();
            Debug.Log("WINNNNNNNN");

        }

        public void Continue()
        {
            if (GameManager.Instance.currentAdventure.adventureName == "RescuingSamurai" && currentWaveIndex == 0)
            {
                currentWaveIndex = 2;
                LoadBattle();
            }

            else if (UIManager.Instance.shopView.gameObject.activeInHierarchy)
            {
                UIManager.Instance.CloseShopView();
                currentWaveIndex++;
                LoadBattle();
            }

            else if (GameManager.Instance.currentAdventure.waves[currentWaveIndex].isNextFight)
            {
                currentWaveIndex++;
                LoadBattle();
            }
            else
            {
                lastDialogueIndex++;
                if (lastDialogueIndex < GameManager.Instance.currentAdventure.adventureStoryMoments.Count)
                {
                    string conversation = GameManager.Instance.currentAdventure.adventureStoryMoments[lastDialogueIndex];
                    DialogueManager.StartConversation(conversation);
                }

                else
                {
                    GameManager.Instance.LoadBackToMainScene();
                }
            }

        }
        public void Lose()
        {
            lose = true;
            UIManager.Instance.ShowLoseScreen();
            Debug.Log("LOSSSSSSSSSE");
        }

        #endregion

        #region Skills

        public void DoABigSneeze()
        {
            DoABigSneezeAvailable = false;
            UIManager.Instance.ToggleSneezeButton(false);
            //do this a coroutine so animation and shit and it waits
            int handCount = HandManager.Instance.hand.Count;
            List<CardUI> cards = new List<CardUI>(HandManager.Instance.hand);
            for (int i = 0; i < handCount; i++)
            {
                HandManager.Instance.RemoveCardFromHand(cards[i]);
            }

            HandManager.Instance.AddCardToHand(handCount);

        }

        public void ShredACard()
        {
            ShredACardAvailable = false;
            HandManager.Instance.RemoveCardFromHand(HandManager.Instance.GetRandomCardFromHand());
            HandManager.Instance.AddCardToHand(1);

        }

        #endregion


        public void ReactionExpired(CardAction cardAction)
        {
            OnReactionExpired?.Invoke(cardAction);
        }

        public void LoadBattle()
        {
            allEnemiesDead = false;
            allAlliesDead = false;
            win = false;
            lose = false;

            cards = new(GameManager.Instance.party.Deck.Cards);
            DeckCardCount = cards.Count;
            InitWaves();
            isPlayerTurn = true;
            MaxMana = GameManager.Instance.GameSettings.StartingMana;
            CurrentMana = MaxMana;
            ShredACardAvailable = true;
            DoABigSneezeAvailable = true;
            HandManager.Instance.InitHand();
            UIManager.Instance.HideWinScreen();
            UIManager.Instance.FadeIn();
            HandManager.Instance.playedACard = false;
            StartBattle();
        }

        public void StartBattle()
        {
            StartCoroutine(GameLoop());
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        }

        [Button]
        public void ForceWin()
        {
            allEnemiesDead = true;
        }

        [Button]
        public void ForceLose()
        {
            allAlliesDead = true;
        }


        public void GoBackToMainMenu()
        {
            GameManager.Instance.LoadBackToMainScene();
        }

        public void CloseApp()
        {
            Application.Quit();
        }
    }

}
