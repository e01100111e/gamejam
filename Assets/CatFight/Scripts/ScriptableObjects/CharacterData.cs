using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CatFight
{
    public class CharacterData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string characterName;
        [SerializeField] private int health;
        [SerializeField] private int startingAttackBonus;
        [SerializeField] private int startingDefence;
        [SerializeField] private Sprite characterSprite;
        [SerializeField] private List<CardData> cardList;
        [SerializeField] private CharacterType characterType;
        [SerializeField] private GameObject characterPrefab;
        [SerializeField] private int positionOnBattleground;
        [SerializeField] private bool isDefaultCharacter;
        public string Id => id;
        public string CharacterName => characterName;       
        public int Health => health;      
        public int StartingAttackBonus => startingAttackBonus;
        public int StartingDefence => startingDefence;
        public Sprite CharacterSprite => characterSprite;
        public List<CardData> CardList => cardList;

        public GameObject CharacterPrefab => characterPrefab;
        
        public CharacterType CharacterType => characterType;

        public int PositionOnBattleground => positionOnBattleground;

        public bool IsDefaultCharacter => isDefaultCharacter;

    }
}
