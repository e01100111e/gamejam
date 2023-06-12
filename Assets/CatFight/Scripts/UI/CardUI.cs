using DG.Tweening;
using Lean.Touch;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatFight
{
    public class CardUI : MonoBehaviour
    {
        [HideInInspector] public Card card;
        [SerializeField] private TMP_Text cardName;
        [SerializeField] private TMP_Text description;
        [SerializeField] private TMP_Text manaCost;
        [SerializeField] private Image image;
        [SerializeField] private int index;

        [SerializeField] private bool isLocked;

        private LeanDragTranslate drag;
        [HideInInspector] public bool HasCard => card != null;

        public int Index { get => index; set => index = value; }


        //Initialize the card UI with the card data

        private void Awake()
        {
            drag = GetComponent<LeanDragTranslate>();
        }
        private void Update()
        {
            if(BattleManager.Instance != null && !BattleManager.Instance.handReady) drag.enabled = false;
            else drag.enabled = true;
        }
        public void Initialize(Card card)
        {
            this.card = card;
            cardName.text = card.cardName;
            description.text = card.description;
            manaCost.text = card.manaCost.ToString();
            if(card.cardSprite == null)
            {
                image.color = card.outlineColor;
            }
            else
            {
                image.sprite = card.cardSprite;

            }
            gameObject.SetActive(true);
        }

        public void OnCardSelected()
        {
            transform.DOScale(GameManager.Instance.GameSettings.CardResizeValue, 0.3f);
            HandManager.Instance.SelectedCard = this;
        }

        public void OnCardDeselected()
        {
            transform.DOScale(1, 0.3f);
            HandManager.Instance.SelectedCard = null;
        }

      
    }
}
