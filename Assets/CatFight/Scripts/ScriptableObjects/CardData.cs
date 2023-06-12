using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CatFight
{
   
    [Serializable]
    public class OverrideData
    {
        public Target overrideActionTarget = Target.None;
        public int overrideTurnCount = 0;
        public int overrideActionValue = 0;
        public int overriddenActionIndex = 0;

    }

    //Add menu item to create a new CardData asset
    [CreateAssetMenu(fileName = "New CardData", menuName = "CatFight/CardData")]

    public class CardData : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string cardName;
        [SerializeField] private int manaCost;
        [SerializeField] private Sprite cardSprite;
        [SerializeField] private Color outlineColor;
        [SerializeField] private string description;
        [SerializeField] private List<ActionData> cardActionList;
        [SerializeField] private bool isEnemyCard = false;

        [Space]
        [Header("Override Action Data")]
        [SerializeField] List<OverrideData> overrides;

        //Create expression-bodied members for the properties above
        public string Id => id;
        public string CardName => cardName;
        public int ManaCost => manaCost;
        public Sprite CardSprite => cardSprite;
        public Color OutlineColor => outlineColor;
        public string Description => description;
        public List<ActionData> CardActionList => cardActionList;
       
        public List<OverrideData> Overrides => overrides;
        public bool IsEnemyCard => isEnemyCard;


        //Method to create 20 card assets in Data/Cards folder with random values
#if UNITY_EDITOR
        [MenuItem("Cat Fight / Create 20 cards")]
        public static void CreateCardData()
        {
            for (int i = 0; i < 20; i++)
            {
                CardData asset = ScriptableObject.CreateInstance<CardData>();
                asset.id = "Card_" + i;
                asset.cardName = "Card_" + i;
                asset.manaCost = Random.Range(0, 10);
                asset.cardSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/CatFight/Sprites/Cards/Card_" + i + ".png");
                asset.outlineColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                asset.description = "Card_" + i + " description";
                asset.cardActionList = new List<ActionData>();
                AssetDatabase.CreateAsset(asset, "Assets/CatFight/Data/Cards/Card_" + i + ".asset");
                AssetDatabase.SaveAssets();
            }
        }
#endif

    }



}
