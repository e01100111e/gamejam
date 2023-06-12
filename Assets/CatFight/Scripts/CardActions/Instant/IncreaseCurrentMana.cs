using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class IncreaseCurrentMana : CardAction
    {
        public IncreaseCurrentMana() : base()
        {
        }

        protected override IEnumerator PerformAction()
        {
            BattleManager.Instance.CurrentMana += ActionValue;
            yield return new WaitForSeconds(1);
        }
    }
}
