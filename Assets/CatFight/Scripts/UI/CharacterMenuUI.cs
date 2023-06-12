using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CatFight
{
    public class CharacterMenuUI : MonoBehaviour
    {
        public Character character;
        [SerializeField] Image characterPortrait;

        Button tapButton;

        private void Start()
        {
            tapButton = GetComponentInChildren<Button>(); 
            tapButton.onClick.AddListener(OnCharacterSelected);
        }

        public void Initialize(Character character)
        {
            this.character = character;
            characterPortrait.sprite = character.characterSprite;
        }
        public void OnCharacterSelected()
        {
            transform.DOScale(GameManager.Instance.GameSettings.CardResizeValue, 0.3f);
            GameManager.Instance.selectedCharacter = this;
            if(GameManager.Instance.party.Cats.Contains(this.character))
            {
                GameManager.Instance.SnapToRosterPosition(this);
            } 
            else if(GameManager.Instance.roster.Contains(this.character))
                GameManager.Instance.SnapToPartyPosition(this);
        }

        public void OnCharacterDeselected()
        {
            transform.DOScale(1, 0.3f);
            GameManager.Instance.selectedCharacter = null;
        }


    }
}
