using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace CatFight
{
    public class CharacterUI : MonoBehaviour
    {
        [SerializeField] private Character _character = new();
        [SerializeField] private Transform _worldTransform;
        [SerializeField] private Image _healthBar;
        [SerializeField] private TMP_Text _currentHealthText;
        [SerializeField] private TMP_Text _maxHealthText;
        [SerializeField] private Transform _activeEffectParent;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private Transform targetGlow;
        [SerializeField] private Transform defenceIcon;
        [SerializeField] private TMP_Text defenceText;
        [SerializeField] private TMP_Text damageText;
        [SerializeField] private TMP_Text statusEffectText;


        private List<GameObject> _activeEffects = new();
        private List<GameObject> _activeReactions = new();


        [SerializeField] private bool isEnemy;
        public GameObject selectGlow;

        [Space]
        [Header("UI Animations")]

        public Sequence attackAnimation;
        public Sequence takeDamageAnimation;
        public Sequence deathAnimation;
        public Sequence damageTextAnimation;
        public Sequence statusTextAnimation;



        public Character CurrentCharacter { get => _character; set => _character = value; }
        public Transform WorldTransform { get => _worldTransform; set => _worldTransform = value; }
        public Image HealthBar { get => _healthBar; set => _healthBar = value; }
        public Transform ActiveEffectParent { get => _activeEffectParent; set => _activeEffectParent = value; }
        public RectTransform Rect { get => _rect; set => _rect = value; }

        public Transform TargetGlow { get => targetGlow; set => targetGlow = value; }

        [Space]
        [Header("Animation")]
        //Create properties to tweak attakAnimation dotween variables
        public float punchXPosition = 0.5f;
        public float punchDuration = 0.3f;

        //Create field to tweak takeDamageAnimation dotween variables
        public float flashDuration = 1f;
        public int flashValue = 20;
        public Ease flashEase = Ease.InOutFlash;

        public void Start()
        {

        }
        private void Awake()
        {

        }

        private void Update()
        {
            if (BattleManager.Instance.SelectedCharacter != null && BattleManager.Instance.SelectedCharacter != this && selectGlow.activeSelf)
            {
                selectGlow.SetActive(false);
            }
        }

        public void InitCharacterUI(Character character)
        {
            CurrentCharacter = character;
            gameObject.SetActive(true);
            selectGlow.SetActive(false);
            defenceIcon.gameObject.SetActive(false);

            CurrentCharacter.OnHealthChange += UpdateHealth;
            CurrentCharacter.OnStatusEffectInflicted += AddActiveEffect;
            CurrentCharacter.OnStatusEffectEnded += RemoveActiveEffect;
            CurrentCharacter.OnStatusEffectUpdated += UpdateActiveEffect;

            CurrentCharacter.OnDefenceChange += UpdateDefenceUI;
            CurrentCharacter.OnDefenceUp += AddDefenceUI;
            CurrentCharacter.OnTakeDamage += Character_OnTakeDamage;

            CurrentCharacter.HealToMax();

            ClearActives();
            SetTweens();

            UpdateDefenceUI(CurrentCharacter.Defence);
            TargetGlow.gameObject.SetActive(false);
        }



        public void ClearActives()
        {
            _activeEffects.Clear();
            _activeReactions.Clear();
            foreach (Transform item in ActiveEffectParent)
            {
                if (item.GetSiblingIndex() == 0) continue;
                Destroy(item.gameObject);
            }
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float fillAmount = currentHealth / maxHealth;
            DOTween.To(() => HealthBar.fillAmount, x => HealthBar.fillAmount = x, fillAmount, 0.3f);
            _currentHealthText.text = currentHealth.ToString();
            _maxHealthText.text = maxHealth.ToString();
        }

        public void SelectThisCharacter()
        {
            if (!CurrentCharacter.IsSelectable()) return;

            if (BattleManager.Instance.SelectedCharacter != null)
            {

                if(BattleManager.Instance.SelectedCharacter._worldTransform != null && BattleManager.Instance.SelectedCharacter._worldTransform.GetComponentInChildren<SpriteRenderer>() != null)
                {
                    BattleManager.Instance.SelectedCharacter._worldTransform.GetComponentInChildren<SpriteRenderer>().material.DOFloat(0, "OutlineAlpha", 0.3f);
                    BattleManager.Instance.SelectedCharacter._worldTransform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 0;
                }              
            }
            BattleManager.Instance.SelectedCharacter = this;
            if(_worldTransform!= null && _worldTransform.GetComponentInChildren<SpriteRenderer>() != null)
            {
                _worldTransform.GetComponentInChildren<SpriteRenderer>().sortingOrder = 1;
                BattleManager.Instance.SelectedCharacter._worldTransform.GetComponentInChildren<SpriteRenderer>().material.DOFloat(1, "OutlineAlpha", 0.3f);
            }
           


            selectGlow.SetActive(true);
            Debug.Log(CurrentCharacter.CharacterName + " selected");
            return;
        }



        public void AddActiveEffect(CardAction effect)
        {
            GameObject effectUI = Instantiate(GameManager.Instance.GameSettings.ActiveEffectUIPrefab, ActiveEffectParent);
            effectUI.GetComponent<ActiveEffectUI>().UpdateEffectUI(effect);
            _activeEffects.Add(effectUI);

            if (effect.Effect.type == StatusEffectType.Stun || effect.Effect.type == StatusEffectType.Poison || effect.Effect.type == StatusEffectType.AttackDebuff)
            {
                TakeDamageAnimation(0);
            }

            switch (effect.Effect.type)
            {
                case StatusEffectType.None:
                    break;
                case StatusEffectType.Poison:
                    StatusEffectAnimation("POISONED!");
                    break;
                case StatusEffectType.Stun:
                    StatusEffectAnimation("STUNNED!");
                    break;
                case StatusEffectType.Protected:
                    StatusEffectAnimation("PROTECTED!");
                    break;
                case StatusEffectType.AttackBuff:
                    StatusEffectAnimation("DAMAGE UP!");
                    break;
                case StatusEffectType.DefenseBuff:
                    StatusEffectAnimation("DEFENCE UP!");
                    break;
                case StatusEffectType.AttackDebuff:
                    StatusEffectAnimation("DAMAGE DOWN!");
                    break;
                case StatusEffectType.DefenseDebuff:
                    StatusEffectAnimation("DEFENCE DOWN!");
                    break;
                case StatusEffectType.HealthBuff:
                    StatusEffectAnimation("HEALTH UP!");
                    break;
            }
        }

        public void RemoveActiveEffect(CardAction expiredEffect)
        {

            _activeEffects.Where(x => x.GetComponent<ActiveEffectUI>().ActiveEffect == expiredEffect).First().gameObject.SetActive(false);
            _activeEffects.RemoveAll(x => x.GetComponent<ActiveEffectUI>().ActiveEffect == expiredEffect);
            Debug.Log(expiredEffect.ActionName + " has expired");
        }

        public void UpdateActiveEffect(CardAction updatedEffect)
        {
            foreach (var effect in _activeEffects)
            {
                if (effect.GetComponent<ActiveEffectUI>().ActiveEffect == updatedEffect)
                {
                    effect.GetComponent<ActiveEffectUI>().UpdateEffectUI(updatedEffect);
                }
            }
        }

        private void RemoveActiveReaction(CardAction expiredReaction)
        {
            int count = _activeReactions.Count;
            for (int i = 0; i < count; i++)
            {
                if (_activeReactions[i].GetComponent<ActiveReactionUI>().ActiveReaction.GetType() == expiredReaction.GetType())
                {
                    _activeReactions[i].SetActive(false);
                    _activeReactions.RemoveAt(i);
                    Debug.Log(expiredReaction.ActionName + " has expired");
                    return;

                }
            }
        }

        private void AddActiveReaction(CardAction reaction)
        {
            GameObject reactionUI = Instantiate(GameManager.Instance.GameSettings.ActiveReactionUIPrefab, ActiveEffectParent);

            reactionUI.GetComponent<ActiveReactionUI>().UpdateEffectUI(reaction);
            _activeReactions.Add(reactionUI);
        }

        private void OnDisable()
        {
            if (CurrentCharacter != null)
            {
                CurrentCharacter.OnHealthChange -= UpdateHealth;
                CurrentCharacter.OnStatusEffectInflicted -= AddActiveEffect;
                CurrentCharacter.OnStatusEffectEnded -= RemoveActiveEffect;
                CurrentCharacter.OnStatusEffectUpdated -= UpdateActiveEffect;

                CurrentCharacter.OnDefenceChange -= UpdateDefenceUI;
                CurrentCharacter.OnDefenceUp -= AddDefenceUI;
                CurrentCharacter.OnTakeDamage -= Character_OnTakeDamage;


            }

        }

        public void UpdateDefenceUI(int newDefence)
        {
            if (newDefence == 0) { defenceIcon.gameObject.SetActive(false); }
            else
            {
                defenceIcon.gameObject.SetActive(true);
                defenceText.text = newDefence.ToString();
            }
        }

        public void SetTweens()
        {

            attackAnimation = DOTween.Sequence().Append(_worldTransform.DOPunchPosition(new Vector3(punchXPosition, 0, 0), punchDuration)).Pause().SetAutoKill(false);

            takeDamageAnimation = DOTween.Sequence().Append(_worldTransform.GetComponentInChildren<SpriteRenderer>().DOFade(0.5f, flashDuration).SetEase(flashEase, flashValue)).Pause().SetAutoKill(false);
            takeDamageAnimation.OnComplete(() =>
            {
                if (CurrentCharacter.IsDead())
                {
                    _worldTransform.GetComponentInChildren<SpriteRenderer>().DOFade(0.5f, 0.5f).SetEase(Ease.OutBounce);
                }
                else
                {
                    takeDamageAnimation.Rewind();
                }
            });

            damageTextAnimation = DOTween.Sequence().Append(damageText.DOFade(1, 0.5f).SetEase(Ease.OutQuint)).Join(damageText.transform.DOMoveY(damageText.transform.position.y + 0.4f, 0.7f).SetEase(Ease.OutQuint))
              .Append(damageText.DOFade(0, 0.2f)).Pause().SetAutoKill(false).OnComplete(() => damageTextAnimation.Rewind());

            statusTextAnimation = DOTween.Sequence().Append(statusEffectText.DOFade(1, 0.2f).SetEase(Ease.OutQuint)).Join(statusEffectText.transform.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.3f, vibrato: 0).SetEase(Ease.OutQuint))
                .Append(statusEffectText.DOFade(0, 0.5f)).Pause().SetAutoKill(false).OnComplete(() => statusTextAnimation.Rewind());
        }

        private void Character_OnTakeDamage(int damageTaken)
        {

            TakeDamageAnimation(damageTaken);

        }

        private void Character_OnDealDamage(bool dealtDamage = false)
        {
            if (dealtDamage)
            {
                DealDamageAnimation();
            }
        }

        [Button]
        public void DealDamageAnimation()
        {
            attackAnimation.Restart();
        }

        [Button]
        public void TakeDamageAnimation(int damageTaken)
        {
            takeDamageAnimation.Restart();

            if (damageTaken > 0)
            {
                damageText.text = damageTaken.ToString();
                damageTextAnimation.Restart();
            }

        }

        public void StatusEffectAnimation(string effect)
        {
            statusEffectText.text = effect;
            statusTextAnimation.Restart();
        }

        public void AddDefenceUI(bool var)
        {
            StatusEffectAnimation("DEFENCE UP!");
        }

    }
}
