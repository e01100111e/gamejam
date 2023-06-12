using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CatFight
{
    public class ActiveReactionUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] CardAction activeReaction;

        public CardAction ActiveReaction => activeReaction;

        public void UpdateEffectUI(CardAction effect)
        {
            icon.sprite = effect.EffectIcon;
            activeReaction = effect;
        }
    }
}
