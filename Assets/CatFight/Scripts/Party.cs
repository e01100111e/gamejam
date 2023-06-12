using PixelCrushers.DialogueSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    [Serializable]
    public class Party
    {
        public List<Character> Cats = new();
        public Deck Deck = new();

        private int treatCount = 0;
        public int TreatAmount
        {
            get => DialogueLua.GetVariable("TreatAmount").AsInt;
            set
            {
                DialogueLua.SetVariable("TreatAmount", value);
                TreatAmountUpdated?.Invoke(TreatAmount);

            }
        }
        public event Action<int> TreatAmountUpdated;


        
        public void AddCat(Character cat)
        {
            Cats.Add(cat);
            foreach (var card in cat.cardList)
            {
                Deck.AddCard(card);
            }
        }

        public void RemoveCat(Character cat)
        {
            Cats.Remove(cat);

            foreach (var card in cat.cardList)
            {
                Deck.RemoveCard(card);
            }
        }

        public void ClearCats()
        {
            Cats.Clear();
        }

    }
}
