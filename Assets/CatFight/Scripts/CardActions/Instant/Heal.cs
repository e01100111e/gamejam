using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class Heal : CardAction
    {
        public Heal() : base()
        {
        }

        protected override IEnumerator PerformAction()
        {
            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            foreach (var target in targetCharacters)
            {
                target.Heal(ActionValue);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
