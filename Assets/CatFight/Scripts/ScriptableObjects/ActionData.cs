using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace CatFight
{
    [CreateAssetMenu(fileName = "New ActionData", menuName = "CatFight/ActionData")]
    public class ActionData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string actionName;
        [SerializeField] private int actionValue;
        [SerializeField] private ActionType actionType;
        [SerializeField] private Target targetType;
        [SerializeField] private int turnToDelay;
        [SerializeField] private int turnToLast;
        [SerializeField] private StatusEffect statusEffect;
        [SerializeField] private ReactionTrigger reaction;
        [SerializeField] private Sprite effectIcon;
        [SerializeField] private bool canStack; 

        //Create expression-bodied members for properties above all at once
        public string Id => id;
        public string ActionName => actionName;
        public int ActionValue => actionValue;
        public Target TargetType => targetType;
        public int TurnToDelay => turnToDelay;
        public int TurnToLast => turnToLast;

        public ActionType ActionType => actionType;

        public StatusEffect StatusEffect => statusEffect;
        public ReactionTrigger Reaction => reaction;
        public Sprite EffectIcon => effectIcon;

        public bool CanStack => canStack;

    }
}
