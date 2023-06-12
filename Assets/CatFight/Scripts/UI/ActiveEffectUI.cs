using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace CatFight
{
    public class ActiveEffectUI : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text turnCount;
        [SerializeField] CardAction activeEffect;
        [SerializeField] private TMP_Text value;

        public CardAction ActiveEffect => activeEffect;

        public void UpdateEffectUI(CardAction effect)
        {
            icon.sprite = effect.EffectIcon;
            turnCount.gameObject.SetActive(true);
            turnCount.text = effect.Effect.activeTurnCount.ToString();
            activeEffect = effect;
            if (effect.ActionValue > 0)
            {
                value.gameObject.SetActive(true);
                value.text = effect.ActionValue.ToString();
            }
            else
            {
                value.gameObject.SetActive(false);
            }
        }

    }
}
