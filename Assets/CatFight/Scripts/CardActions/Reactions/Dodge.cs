using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class Dodge : CardAction
    {
        public Dodge() : base()
        {
        }

        protected override IEnumerator PerformAction()
        {
            BattleManager.Instance.TargetedCharacter.CurrentCharacter.AddReaction(this);
            Debug.Log("Inflicting DODGE on character for " + turnToLast + " turns");
            yield return new WaitForSeconds(1);
        }

        public override bool ReactionTriggered(ReactionTrigger trigger)
        {
            if (trigger == Reaction.trigger)
            {
                Reaction.hasTriggered = true;
            }

            return Reaction.hasTriggered;
        }

    }
}
