using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class Deck
    {
        public List<Card> Cards = new();

        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
        }

        public void Clear()
        {
            Cards.Clear();
        }

        public void ShuffleDeck()
        {
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int r = Random.Range(0, i);
                (Cards[r], Cards[i]) = (Cards[i], Cards[r]);
            }

        }

    }
}
