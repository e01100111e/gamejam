using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CatFight
{
    public class Character
    {


        private string id;
        private string characterName;
        private float health;
        private float maxHealth;
        private int attack;
        private int defence;
        public Sprite characterSprite;
        public List<Card> cardList = new();
        public CharacterType characterType;
        public GameObject characterPrefab;
        public int positionOnBattleground;
        public bool isDefaultCharacter;

        public List<CardAction> activeStatusEffects = new();
        public List<CardAction> activeReactions = new();

        public event Action<float, float> OnHealthChange;
        public event Action<int> OnDefenceChange;
        public event Action<bool> OnDefenceUp;
        public event Action<int> OnAttackChange;

        public event Action<int> OnTakeDamage;

        public static event Action<CharacterType> OnCharacterDead;

        public event Action<CardAction> OnStatusEffectInflicted;
        public event Action<CardAction> OnStatusEffectEnded;
        public event Action<CardAction> OnStatusEffectUpdated;


        public event Action<CardAction> OnReactionAdded;
        public event Action<CardAction> OnReactionRemove;

        public float CurrentHealth
        {
            get { return health; }
            set
            {
                health = Mathf.Clamp(value, 0, MaxHealth); // keep the current health between 0 and MaxHealth
                OnHealthChange?.Invoke(health, maxHealth); // invoke the event with the current health
            }
        }


        public float MaxHealth
        {
            get => maxHealth;
            set
            {
                maxHealth = value;
                OnHealthChange?.Invoke(health, maxHealth);
            }
        }
        public int Attack { get => attack; set => attack = value; }
        public int Defence
        {
            get => defence;
            set
            {
                defence = Mathf.Clamp(value, 0, int.MaxValue);
                OnDefenceChange?.Invoke(defence);
            }
        }

        public string CharacterName { get => characterName; set => characterName = value; }
        public string Id { get => id; set => id = value; }


        public bool IsDead() => CurrentHealth == 0;
        public Character(CharacterData data)
        {
            Id = data.Id;
            CharacterName = data.CharacterName;
            MaxHealth = data.Health;
            health = MaxHealth;
            Attack = data.StartingAttackBonus;
            Defence = data.StartingDefence;
            characterSprite = data.CharacterSprite;
            foreach (var _card in data.CardList)
            {
                Card newCard = new(_card);
                cardList.Add(newCard);
            }
            characterType = data.CharacterType;
            characterPrefab = data.CharacterPrefab;
            positionOnBattleground = data.PositionOnBattleground;
            CurrentHealth = MaxHealth;
            activeReactions = new();
            activeStatusEffects = new();
            isDefaultCharacter = data.IsDefaultCharacter;
        }

        public Character()
        {

        }

        public void TakeDamage(int damage, bool rawDamage = false)
        {
            if (damage > 0) { OnTakeDamage?.Invoke(damage); }

            if (rawDamage)
            {
                CurrentHealth -= damage;
            }
            else
            {
                int exceedDamage = damage - Defence;
                if (exceedDamage > 0)
                {
                    Defence = 0;
                    CurrentHealth -= exceedDamage;
                }

                else if (exceedDamage == 0)
                {
                    Defence = 0;
                }

                else
                {
                    Defence -= damage;
                }
            }
            if (CurrentHealth <= 0)
            {
                OnCharacterDead?.Invoke(characterType);
            }

        }

        public void Heal(int heal)
        {
            CurrentHealth += heal;
        }

        public void HealToMax()
        {
            CurrentHealth = MaxHealth;
        }

        public bool IsTargetable(bool isAttacked = false)
        {
            if (BattleManager.Instance.SelectedCharacter.CurrentCharacter.characterType == CharacterType.Enemy && isAttacked && activeStatusEffects.Count > 0 && activeStatusEffects.Any(x => x.Effect.type == StatusEffectType.Protected && x.Effect.activeTurnCount > 0))
            {
                Debug.Log(characterName + " has active status effect PROTECTED");
                return false;
            }

            if (IsDead())
                return false;

            return true;
        }

        public bool IsSelectable()
        {
            if (activeStatusEffects.Count > 0 && activeStatusEffects.Any(x => x.Effect.type == StatusEffectType.Stun && x.Effect.activeTurnCount > 0))
            {
                Debug.Log(characterName + " has active status effect STUN");
                return false;
            }

            if (IsDead())
                return false;

            //if (characterType != CharacterType.Ally) return false;

            return true;

        }

        public bool CanAttack()
        {

            if (activeStatusEffects.Count > 0 && activeStatusEffects.Any(x => x.Effect.type == StatusEffectType.Stun && x.Effect.activeTurnCount > 0))
            {
                Debug.Log(characterName + " has active status effect STUN");
                return false;
            }

            if (IsDead())
                return false;

            return true;
        }

        public bool IsInvulnerable()
        {
            if (activeReactions.Count > 0 && activeReactions.Any(x => x is Dodge))
            {
                Debug.Log(characterName + " has active reaction DODGE");
                return true;
            }

            if (IsDead())
                return true;

            return false;
        }

        public int GetActiveAttackDebuffs()
        {
            if (activeStatusEffects.Any(x => x is AttackDebuff) == false) return 0;
            return activeStatusEffects.Where(x => x is AttackDebuff).Select(x => x.ActionValue).Sum();
        }

        public void AddDefence(int value)
        {
            Defence += value;
            OnDefenceUp?.Invoke(true);
        }

        public int GetPoisonDamagePerTurn()
        {
            if (activeStatusEffects.Any(x => x is Poison) == false) return 0;
            return activeStatusEffects.Where(x => x is Poison).Select(x => x.ActionValue).Sum();
        }

        public void ApplyPoison()
        {
            int totalPoisonDamage = GetPoisonDamagePerTurn();
            TakeDamage(totalPoisonDamage, true);
        }


        #region Status Effects
        public void InflictStatusEffect(CardAction statusEffectAction)
        {
            if (activeStatusEffects.Any(x => x.GetType() == statusEffectAction.GetType() && x.CanStack == false))
            {
                Debug.Log(characterName + " has already has this effect active: " + statusEffectAction.ActionName);
                return;
            }
            else if (activeStatusEffects.Any(x => x.GetType() == statusEffectAction.GetType() && x.CanStack == true))
            {
                CardAction existingStatusEffect = activeStatusEffects.Find(x => x.GetType() == statusEffectAction.GetType());
                existingStatusEffect.Effect.activeTurnCount += statusEffectAction.Effect.activeTurnCount;
                OnStatusEffectUpdated?.Invoke(existingStatusEffect);
            }
            else if (activeStatusEffects.Count == 0 || activeStatusEffects.Any(x => x.GetType() != statusEffectAction.GetType()))
            {
                CardAction newStatusEffect = statusEffectAction.Clone();
                activeStatusEffects.Add(newStatusEffect);
                OnStatusEffectInflicted?.Invoke(newStatusEffect);
            }
        }

        public void UpdateStatusEffects()
        {
            if (IsDead()) return;
            ApplyPoison();

            List<CardAction> activeStatusEffectsCopy = new List<CardAction>(activeStatusEffects);

            for (int i = 0; i < activeStatusEffectsCopy.Count; i++)
            {
                if (activeStatusEffects[i] == null) continue;
                activeStatusEffects[i].Effect.activeTurnCount--;
                if (activeStatusEffects[i].Effect.activeTurnCount == 0)
                {
                    OnStatusEffectEnded?.Invoke(activeStatusEffects[i]);
                    activeStatusEffects.RemoveAt(i);

                }
                else
                {
                    OnStatusEffectUpdated(activeStatusEffects[i]);
                }
            }
        }

        #endregion

        #region Reactions

        public void AddReaction(CardAction action)
        {
            if (activeReactions.Any(x => x.GetType() == action.GetType()))
            {
                Debug.Log(characterName + " has already has this reaction active: " + action.ActionName);
                return;
            }
            else
            {
                CardAction newReaction = action.Clone();
                activeReactions.Add(newReaction);
                OnReactionAdded?.Invoke(newReaction);
            }
        }

        public void RemoveReaction(CardAction reaction)
        {
            if (activeReactions.Any(x => x.GetType() != reaction.GetType()))
            {
                Debug.Log(characterName + " does not have: " + reaction.ActionName);
                return;
            }
            else
            {
                activeReactions.Remove(reaction);
                OnReactionRemove?.Invoke(reaction);
            }
        }


        public IEnumerator CheckIfAReactionTriggered(ReactionTrigger reaction)
        {
            foreach (var activeReaction in activeReactions)
            {
                if (activeReaction.Reaction.hasTriggered != false && activeReaction.ReactionTriggered(reaction))
                {
                    Debug.Log(activeReaction.ActionName + " triggered");
                    BattleManager.Instance.aReactionTriggered = true;
                    yield break;
                }
            }

            yield return null;
        }

        public void ClearExpiredReactions()
        {
            int count = activeReactions.Count;

            for (int i = 0; i < count; i++)
            {
                if (activeReactions[i] != null && activeReactions[i].Reaction != null && activeReactions[i].Reaction.hasTriggered == true)
                {
                    OnReactionRemove?.Invoke(activeReactions[i]);
                    activeReactions.RemoveAt(i);
                }
            }
        }

        public void ClearStatusEffects()
        {
            List<CardAction> activeStatusEffectsCopy = new List<CardAction>(activeStatusEffects);

            for (int i = 0; i < activeStatusEffectsCopy.Count; i++)
            {
                if (activeStatusEffects[i].Effect.type == StatusEffectType.Poison ||
                    activeStatusEffects[i].Effect.type == StatusEffectType.AttackDebuff ||
                    activeStatusEffects[i].Effect.type == StatusEffectType.Stun
                    )
                {
                    OnStatusEffectEnded?.Invoke(activeStatusEffects[i]);
                    activeStatusEffects.RemoveAt(i);

                }

                OnStatusEffectUpdated(activeStatusEffects[i]);
            }
        }
        #endregion

    }
}
