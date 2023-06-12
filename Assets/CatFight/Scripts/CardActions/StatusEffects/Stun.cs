using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class Stun : CardAction
    {

        public Stun() : base()
        {
        }


        protected override IEnumerator PerformAction()
        {
            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            foreach (var target in targetCharacters)
            {
                target.InflictStatusEffect(this);
                Debug.Log("Inflicting stun debuff on character for " + turnToLast + " turns");
            }
            yield return new WaitForSeconds(1);
        }
    }
}
