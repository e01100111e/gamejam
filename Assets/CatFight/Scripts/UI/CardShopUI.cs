using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatFight
{
    public class CardShopUI : MonoBehaviour
    {

        public TMP_Text costText;
        public CardUI cardUI;
        public CardData cardData;
        public Button purchaseButton;

        private CanvasGroup alphaThingie;
        public int cost;

        public void Start()
        {
            Card card = new(cardData);
            cardUI.Initialize(card);

            costText.text = cost.ToString();
            gameObject.name = card.cardName;
            alphaThingie = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (GameManager.Instance.party.TreatAmount >= cost)
            {
                purchaseButton.interactable = true;
                alphaThingie.alpha = 1;
            }
            else
            {
                purchaseButton.interactable = false;
                alphaThingie.alpha = 0.5f;
            }
        }

        public void Purchase()
        {
            if (GameManager.Instance.party.TreatAmount >= cost)
            {
                GameManager.Instance.party.Deck.AddCard(cardUI.card);
                purchaseButton.interactable = false;
                GameManager.Instance.Spendtreats(cost);
            }
        }
    }
}
