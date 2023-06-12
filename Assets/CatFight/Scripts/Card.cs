using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace CatFight
{
    public class Card
    {

        public string id;
        public string cardName;
        public int manaCost;
        public Sprite cardSprite;
        public Color outlineColor;
        public string description;
        public List<ActionData> cardActionList;
        public List<OverrideData> overrideActionList;
        public bool isEnemyCard;

        public bool cardActionCompleted = false;

        public Card(CardData cardData)
        {
            id = cardData.Id;
            cardName = cardData.CardName;
            manaCost = cardData.ManaCost;
            cardSprite = cardData.CardSprite;
            outlineColor = cardData.OutlineColor;
            description = cardData.Description;
            cardActionList = cardData.CardActionList;
            overrideActionList = cardData.Overrides;
            isEnemyCard = cardData.IsEnemyCard;
        }

        public Card()
        {

        }

        public bool IsManaAffordable()
        {
            return BattleManager.Instance.CurrentMana >= manaCost;
        }

        public void PlayCard()
        {
            if (!isEnemyCard) BattleManager.Instance.SpendMana(manaCost);
            foreach (ActionData actionData in cardActionList)
            {
                Type type = Type.GetType("CatFight." + actionData.ActionName);
                CardAction action = (CardAction)Activator.CreateInstance(type);
                action.InitializeAction(actionData);

                if (overrideActionList.Count > 0)
                {
                    foreach (OverrideData overrideData in overrideActionList)
                    {
                        if (overrideData.overriddenActionIndex == cardActionList.IndexOf(actionData))
                        {
                            if (overrideData.overrideActionTarget != Target.None) action.TargetType = overrideData.overrideActionTarget;
                            if (overrideData.overrideTurnCount != 0) action.Effect.activeTurnCount = overrideData.overrideTurnCount;
                            if (overrideData.overrideActionValue != 0) action.ActionValue = overrideData.overrideActionValue;
                        }
                    }
                }

                action.StartAction(OnCardActionCompleted);
            }
        }


        public bool IsTargetCorrect(Character targetCharacter = null)
        {

            Target target;

            target = GetFirstActionTarget();


            if (!targetCharacter.IsTargetable())
            {
                return false;
            }


            if (target == Target.SingleEnemy && targetCharacter.characterType == CharacterType.Enemy)
            {
                return true;
            }
            else if ((target == Target.SingleAlly || target == Target.Self) && targetCharacter.characterType == CharacterType.Ally)
            {
                return true;
            }
            else if (target == Target.AllEnemies && targetCharacter.characterType == CharacterType.Enemy)
            {
                return true;
            }
            else if (target == Target.AllAllies && targetCharacter.characterType == CharacterType.Ally)
            {
                return true;
            }
            else if (target == Target.AllCharacters)
            {
                return true;
            }

            else
            {
                return false;
            }
        }


        void OnCardActionCompleted()
        {
            cardActionCompleted = true;
        }

        public Target GetFirstActionTarget()
        {
            if (overrideActionList != null && overrideActionList.Count > 0 && overrideActionList.FirstOrDefault().overrideActionTarget != Target.None)
            { return overrideActionList.FirstOrDefault().overrideActionTarget; }
            else
                return cardActionList.FirstOrDefault().TargetType;
        }
    }
}
