using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class IncreaseMaxMana : CardAction
    {
        public IncreaseMaxMana() : base()
        {
        }

        protected override IEnumerator PerformAction()
        {
            BattleManager.Instance.MaxMana += ActionValue;
            yield return new WaitForSeconds(1);
        }
    }
}
