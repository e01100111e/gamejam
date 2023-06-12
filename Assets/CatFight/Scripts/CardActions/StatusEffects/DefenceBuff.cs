using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DG.Tweening.DOTweenAnimation;

namespace CatFight
{
    public class DefenceBuff : CardAction
    {

        public DefenceBuff() : base()
        {
        }

        protected override IEnumerator PerformAction()
        {
            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            int value = ActionValue;
            foreach (var target in targetCharacters)
            {
                target.AddDefence(value);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
