using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class LightAttack : CardAction
    {
        public LightAttack() : base()
        {
        }

        protected override IEnumerator ActionCoroutine()
        {
            Debug.Log("Attempting to startingAttackBonus");


            yield return BattleManager.Instance.StartCoroutine(PerformAction());

            Debug.Log("CardAction completed!");

            // Call the completion callback
            if (completionCallback != null)
            {
                completionCallback();
            }

        }

        protected override IEnumerator PerformAction()
        {
            int attackDebuffs = BattleManager.Instance.SelectedCharacter.CurrentCharacter.GetActiveAttackDebuffs();

            int resultValue = Mathf.Clamp(ActionValue - attackDebuffs, 0, ActionValue);

            BattleManager.Instance.SelectedCharacter.DealDamageAnimation();

            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            foreach (var target in targetCharacters)
            {
                target.TakeDamage(resultValue);
            }
            yield return new WaitForSeconds(2);
        }
    }
}
