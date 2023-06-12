using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace CatFight
{
    [Serializable]
    public class StatusEffect
    {
        public StatusEffectType type;
        public int activeTurnCount;

    }

    [Serializable]
    public class Reaction
    {
        public ReactionTrigger trigger;
        public bool hasTriggered;
    }


    public class CardAction
    {
        private ActionData actionData;

        protected string id;
        protected string actionName;
        protected int actionValue;
        protected Target targetType;
        protected int turnToDelay;
        protected int turnToLast;
        protected StatusEffect effect;
        protected Reaction reaction;
        protected Sprite effectIcon;
        protected bool canStack;

        public CardAction()
        {
        }

        public void InitializeAction(ActionData actionData)
        {
            Id = actionData.Id;
            ActionName = actionData.ActionName;
            ActionValue = actionData.ActionValue;
            TargetType = actionData.TargetType;
            TurnToDelay = actionData.TurnToDelay;
            TurnToLast = actionData.TurnToLast;

            StatusEffect statusEffect = new();
            statusEffect.type = actionData.StatusEffect.type;
            statusEffect.activeTurnCount = actionData.TurnToLast;

            Effect = statusEffect;

            Reaction reaction = new();
            reaction.trigger = actionData.Reaction;

            Reaction = reaction;

            EffectIcon = actionData.EffectIcon;
            CanStack = actionData.CanStack;
        }

        // Define the delegate type for the completion callback
        public delegate void CompletionCallback();

        // Define a variable to hold the completion callback
        protected CompletionCallback completionCallback;

        public string Id { get => id; set => id = value; }
        public string ActionName { get => actionName; set => actionName = value; }
        public int ActionValue { get => actionValue; set => actionValue = value; }
        public Target TargetType { get => targetType; set => targetType = value; }
        public int TurnToDelay { get => turnToDelay; set => turnToDelay = value; }
        public int TurnToLast { get => turnToLast; set => turnToLast = value; }

        public StatusEffect Effect { get => effect; set => effect = value; }
        public Reaction Reaction { get => reaction; set => reaction = value; }
        public Sprite EffectIcon { get => effectIcon; set => effectIcon = value; }
        public bool CanStack { get => canStack; set => canStack = value; }


        
        public virtual CardAction Clone()
        {
            CardAction clone = Activator.CreateInstance(this.GetType()) as CardAction;
            
            clone.Id = Id;
            clone.ActionName = ActionName;
            clone.ActionValue = ActionValue;
            clone.TargetType = TargetType;
            clone.TurnToDelay = TurnToDelay;
            clone.TurnToLast = TurnToLast;
            clone.CanStack = CanStack;


            StatusEffect statusEffect = new();
            statusEffect.type = effect.type;
            statusEffect.activeTurnCount = effect.activeTurnCount;

            clone.Effect = statusEffect;
            clone.EffectIcon = EffectIcon;
            clone.CanStack = CanStack;



            return clone;
        }

        // Start the action and wait for it to complete
        public void StartAction(CompletionCallback callback)
        {
            // Set the completion callback
            completionCallback = callback;

            // Start the action coroutine
            BattleManager.Instance.StartCoroutine(ActionCoroutine());
        }

        // Coroutine to perform the action and wait for it to complete
        protected virtual IEnumerator ActionCoroutine()
        {
            Debug.Log("Starting action...");

            // Wait for the action to complete
            yield return BattleManager.Instance.StartCoroutine(PerformAction());

            Debug.Log("CardAction completed!");

            // Call the completion callback
            if (completionCallback != null)
            {
                completionCallback();
            }
        }

        // Method to perform the action (to be implemented in derived classes)
        protected virtual IEnumerator PerformAction()
        {
            Debug.Log("Performing action...");
            yield return null;
        }

        public virtual bool ReactionTriggered(ReactionTrigger trigger)
        {
            return false;
        }
    }
}
