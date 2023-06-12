using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class ClearStatus : CardAction
    {
        public ClearStatus() : base() { }
        protected override IEnumerator PerformAction()
        {
            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            foreach (var target in targetCharacters)
            {
                target.ClearStatusEffects();
                Debug.Log("Clearing all status effects on character");
            }
            yield return new WaitForSeconds(1);
        }
    }

}
