using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class IncreaseMaxHealth : CardAction
    {
        public IncreaseMaxHealth() : base()
        {
        }

        protected override IEnumerator PerformAction()
        {
            List<Character> targetCharacters = BattleManager.Instance.GetTargets(TargetType);
            foreach (var target in targetCharacters)
            {
                if(target.CurrentHealth == target.MaxHealth)
                {
                    target.MaxHealth += ActionValue;
                    target.CurrentHealth += ActionValue;
                }
                else
                {
                    target.MaxHealth += ActionValue;
                }



            }
            yield return new WaitForSeconds(1);
        }
    }
}
