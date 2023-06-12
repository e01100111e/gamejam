using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class Protected : CardAction
    {
        public Protected() : base() { }

        protected override IEnumerator PerformAction()
        {
            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            foreach (var target in targetCharacters)
            {
                target.InflictStatusEffect(this);
                Debug.Log("Inflicting protected debuff on character for " + turnToLast + " turns");
            }
            yield return new WaitForSeconds(1);
        }
    }
}
