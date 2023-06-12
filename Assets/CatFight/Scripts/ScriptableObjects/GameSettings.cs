using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{

    [CreateAssetMenu(fileName = "GameSettings", menuName = "CatFight/GameSettings", order = 0)]
    public class GameSettings : ScriptableObject
    {
        [SerializeField] private List<CharacterData> allCats;
        [SerializeField] private List<RatData> allRats;
        [SerializeField] private List<CardData> allCards;
        [SerializeField] private List<ActionData> allActions;
        [SerializeField] private List<WaveData> allLevels;
        [SerializeField] private List<AdventureData> allAdventures;
        [SerializeField] private int maxCardCountInHand;
        [SerializeField] private int maxPartyCharacterCount;
        [SerializeField] private float cardResizeValue;
        [SerializeField] private int defaultHealth;

        [SerializeField] private CardUI cardPrefab;
        [SerializeField] private GameObject healthPrefab;
        [SerializeField] private GameObject activeEffectUIPrefab;
        [SerializeField] private GameObject activeReactionUIPrefab;
        [SerializeField] private CharacterMenuUI characterMenuUIPrefab;


        [SerializeField] private int startingMana;
        [SerializeField] private int defaultManaPerTurn;
        [SerializeField] private int maxManaPerBattle;
        public List<CharacterData> AllCats => allCats;
        public List<RatData> AllRats => allRats;
        public List<CardData> AllCards => allCards;
        public List<ActionData> AllActions => allActions;
        public List<WaveData> AllLevels => allLevels;
        public List<AdventureData> AllAdventures => allAdventures;
        public int MaxCardCountInHand => maxCardCountInHand;
        public int MaxPartyCharacterCount => maxPartyCharacterCount;
        public float CardResizeValue => cardResizeValue;

        public int DefaultHealth => defaultHealth;
        public CardUI CardPrefab => cardPrefab;
        public GameObject HealthPrefab => healthPrefab;
        public GameObject ActiveEffectUIPrefab => activeEffectUIPrefab;
        public GameObject ActiveReactionUIPrefab => activeReactionUIPrefab;
        public CharacterMenuUI CharacterMenuUIPrefab => characterMenuUIPrefab;
        public int StartingMana => startingMana;
        public int DefaultManaPerTurn => defaultManaPerTurn;
        public int MaxManaPerBattle => maxManaPerBattle;



    }
}
