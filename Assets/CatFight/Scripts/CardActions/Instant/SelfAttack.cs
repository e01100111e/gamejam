using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class SelfAttack : CardAction
    {
        public SelfAttack() : base()
        {
        }

        protected override IEnumerator PerformAction()
        {
            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            foreach (var target in targetCharacters)
            {
                target.TakeDamage(ActionValue);
                Debug.Log("Inflicting startingAttackBonus debuff on character for " + turnToLast + " turns");
            }
           
            Debug.Log("Dealing damage to self");
            yield return new WaitForSeconds(2);
        }
    }
}
