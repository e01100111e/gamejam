using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CatFight
{
    public class HandManager : MonoBehaviour
    {
        public static HandManager Instance;
        private void Awake()
        {
            Instance = this;
        }

        public List<CardUI> hand = new();
        public List<Card> discardPile = new();

        private List<Vector2> initCardPositions = new();
        public List<CardUI> startingCards = new();

        [SerializeField] Transform discardPileTransform;
        [SerializeField] private CardUI _draggingCard;
        [SerializeField] private CardUI _selectedCard;

        [SerializeField] private RectTransform handRect;
        [SerializeField] private RectTransform cardDetectRect;
        public CardUI DraggingCard { get => _draggingCard; set => _draggingCard = value; }
        public CardUI SelectedCard { get => _selectedCard; set => _selectedCard = value; }

        public static event Action<int> OnHandChange;
        public GameObject targetGlow;
        CharacterUI cardOnCharacterUI;

        private bool IsCardOnCharacter = false;

        public bool playedACard = false;

        public void InitHand()
        {
            initCardPositions = startingCards.ConvertAll(x => x.GetComponent<RectTransform>().anchoredPosition);
            ClearHand();
            FillHand();
            //for (int i = 0; i < GameManager.Instance.GameSettings.MaxCardCountInHand; i++)
            //{
            //    Card newCard = BattleManager.Instance.DrawCardFromDeck();
            //    if (newCard != null)
            //    {
            //        hand[i].Initialize(newCard);                  
            //        hand[i].Index = i;
            //        //initCardPositions[i] = hand[i].transform.GetComponent<RectTransform>().anchoredPosition;
            //    }
            //}
        }

        private void Update()
        {

            if (!BattleManager.Instance.IsPlayerTurn || !BattleManager.Instance.IsGameOn)
            {
                return;
            }

            if (TouchManager.Instance.IsDragging && SelectedCard != null && BattleManager.Instance.handReady)
            {
                DraggingCard = SelectedCard;

                Vector2 touchPos = TouchManager.Instance.GetTouchPosition();

                if (RectTransformUtility.RectangleContainsScreenPoint(cardDetectRect, touchPos, UIManager.Instance.UiCamera))
                {
                    cardOnCharacterUI = UIManager.Instance.CheckCharacterOnTouchPosition(touchPos);
                    if (cardOnCharacterUI != null && DraggingCard.card.IsTargetCorrect(cardOnCharacterUI.CurrentCharacter))
                    {
                        IsCardOnCharacter = true;
                        UIManager.Instance.UpdateTargetGlows(DraggingCard.card.GetFirstActionTarget(), cardOnCharacterUI);
                    }
                    else
                    {
                        IsCardOnCharacter = false;
                        UIManager.Instance.HideTargetGlow();
                    }
                }
                else
                {
                    UIManager.Instance.HideTargetGlow();
                    IsCardOnCharacter = false;
                }
            }

            if (!TouchManager.Instance.IsTouching && DraggingCard != null && IsCardOnCharacter && DraggingCard.card.IsManaAffordable())
            {
                Debug.Log("Can play the card!!");

                PlayCard(cardOnCharacterUI, DraggingCard);
            }

            if (!TouchManager.Instance.IsTouching && DraggingCard != null &&
                ((IsCardOnCharacter && !DraggingCard.card.IsManaAffordable()) || !IsCardOnCharacter))
            {
                Debug.Log("can't play the card!");
                SnapCardBackToItsPosition(DraggingCard);
                DraggingCard = null;
                SelectedCard = null;
                UIManager.Instance.HideTargetGlow();
            }


        }
        public void AddCardToHand(int cardCountToAdd = 1)
        {
            for (int i = 0; i < cardCountToAdd; i++)
            {
                if (hand.Count == GameManager.Instance.GameSettings.MaxCardCountInHand)
                {
                    Debug.Log("hand is already full");
                    return;
                }

                Card newCard = BattleManager.Instance.DrawCardFromDeck();
                if (newCard != null)
                {
                    CardUI newCardUI = Instantiate(GameManager.Instance.GameSettings.CardPrefab);
                    newCardUI.Initialize(newCard);
                    hand.Add(newCardUI);
                    OnHandChange?.Invoke(hand.Count);
                    newCardUI.Index = hand.IndexOf(newCardUI);
                    newCardUI.transform.SetParent(handRect.transform, false);
                    newCardUI.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                    newCardUI.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                    SnapCardBackToItsPosition(newCardUI);
                }
            }
        }

        public void RemoveCardFromHand(CardUI removeCard)
        {
            removeCard.gameObject.SetActive(false);

            hand.Remove(removeCard);
            discardPile.Add(removeCard.card);
            removeCard.transform.SetParent(discardPileTransform);
            UpdateCardUIIndexes();
            OnHandChange?.Invoke(hand.Count);
        }

        public void MoveCardToIndex(int currentIndex, int toIndex)
        {
            if (currentIndex == toIndex) return; // Same index, do nothing
            CardUI card = hand[currentIndex];
            hand.RemoveAt(currentIndex);
            hand.Insert(toIndex, card);
            card.transform.SetSiblingIndex(toIndex);

        }

        public void SnapCardBackToItsPosition(CardUI card)
        {
            if (card == null) { Debug.Log("snap back card is null"); return; }
            card.transform.GetComponent<RectTransform>().DOAnchorPos(initCardPositions[card.Index], 0.3f, snapping: true);
        }

        public void UpdateCardUIIndexes()
        {
            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].Index = i;
                SnapCardBackToItsPosition(hand[i]);
            }
        }

        public void ClearHand()
        {
            for (int i = 0; i < hand.Count; i++)
            {
                hand[i].card = null;
                hand[i].gameObject.SetActive(false);
            }

            hand.Clear();
        }

        public void FillHand()
        {
            int cardAmountToAdd = GameManager.Instance.GameSettings.MaxCardCountInHand - hand.Count;
            AddCardToHand(cardAmountToAdd);
            playedACard = false;
        }

        public void PlayCard(CharacterUI target, CardUI cardUI)
        {
            playedACard = true;

            cardUI.gameObject.SetActive(false);
            DraggingCard = null;
            SelectedCard = null;
            IsCardOnCharacter = false;
            RemoveCardFromHand(cardUI);
            UIManager.Instance.HideTargetGlow();
            BattleManager.Instance.TargetedCharacter = target;
            cardUI.card.PlayCard();

        }


        public CardUI GetRandomCardFromHand()
        {
            int randomIndex = Random.Range(0, hand.Count);
            return hand[randomIndex];
        }

    }

}
