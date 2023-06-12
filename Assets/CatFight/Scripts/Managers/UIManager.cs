using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

namespace CatFight
{
    public class UIManager : MonoBehaviour
    {

        public static UIManager Instance;

        [SerializeField] private List<CharacterUI> allies;
        [SerializeField] private List<CharacterUI> enemies;

        [SerializeField] private Camera uiCamera;
        [SerializeField] private Transform targetGlow;
        [SerializeField] private Transform selectGlow;

        [SerializeField] private TMP_Text currentMana;
        [SerializeField] private TMP_Text maxMana;
        [SerializeField] private TMP_Text waveReward;


        public GameObject winScreen;
        public GameObject loseScreen;

        [SerializeField] private Button endTurnButton;
        [SerializeField] private Button sneezeButton;
        [SerializeField] private Button shredButton;
        [SerializeField] private Button sitButton;

        [SerializeField] private TMP_Text treatAmountText;

        public Image fadeBlack;

        public Transform shopView;

        public float glowYoffset = -0.5f;

        public Camera UiCamera { get => uiCamera; set => uiCamera = value; }
        public Transform TargetGlow { get => targetGlow; set => targetGlow = value; }
        public Transform SelectGlow { get => selectGlow; set => selectGlow = value; }
        public List<CharacterUI> Allies => allies;
        public List<CharacterUI> Enemies => enemies;

        private void Start()
        {
            HideTargetGlow();
            GameManager.Instance.party.TreatAmountUpdated += UpdateTreatAmountText;
        }


        private void UpdateTreatAmountText(int amount)
        {
            if (treatAmountText != null)
                treatAmountText.text = amount.ToString();
        }

        private void Awake()
        {
            Instance = this;
            BattleManager.OnManaChange += UpdateCurrentManaUI;
            BattleManager.OnMaxManaChange += UpdateMaxManaUI;
            HandManager.OnHandChange += OnHandChange;
        }

        private void OnDisable()
        {
            HandManager.OnHandChange -= OnHandChange;
            BattleManager.OnManaChange -= UpdateCurrentManaUI;
            BattleManager.OnMaxManaChange -= UpdateMaxManaUI;
            GameManager.Instance.party.TreatAmountUpdated -= UpdateTreatAmountText;
        }

        //Combine two lists into a new list
        public CharacterUI CheckCharacterOnTouchPosition(Vector2 touchPos)
        {
            bool isOnCharacter = false;
            List<CharacterUI> characterUIs = Allies.Concat(enemies).ToList();
            for (int i = 0; i < characterUIs.Count; i++)
            {
                RectTransform rectToCheck = characterUIs[i].Rect;
                if (rectToCheck == null) continue;
                if (!characterUIs[i].gameObject.activeInHierarchy) continue;
                if (RectTransformUtility.RectangleContainsScreenPoint(rectToCheck, touchPos, UiCamera))
                {
                    Debug.Log("Inside the rect of character: " + characterUIs[i].name);
                    isOnCharacter = true;
                    if (characterUIs[i].gameObject.activeInHierarchy) return characterUIs[i];
                }
            }

            if (!isOnCharacter) HideTargetGlow();
            return null;
        }

        public void HideTargetGlow()
        {
            allies.ForEach(x=>x.TargetGlow.gameObject.SetActive(false));
            enemies.ForEach(x=>x.TargetGlow.gameObject.SetActive(false));
        }

        public void LoadCharacterUI(Character character, int index)
        {
            if (character.characterType == CharacterType.Ally)
            {
                Allies[index].InitCharacterUI(character);
            }
            if (character.characterType == CharacterType.Enemy)
            {
                enemies[index].InitCharacterUI(character);
            }
        }

        public void ClearCharacters()
        {
            Allies.ForEach(x => x.WorldTransform.gameObject.SetActive(false));
            enemies.ForEach(x => x.WorldTransform.gameObject.SetActive(false));

            Allies.ForEach(x => x.gameObject.SetActive(false));
            enemies.ForEach(x => x.gameObject.SetActive(false));

        }

        public void ShowWinScreen()
        {
            waveReward.text = "";
            winScreen.SetActive(true);
            int traits = GameManager.Instance.GetCurrentWaveReward();
            GameManager.Instance.GainTraits(traits);
            waveReward.text = traits.ToString();
        }

        public void ShowLoseScreen()
        {
            loseScreen.SetActive(true);
        }

        public void HideWinScreen()
        {
            winScreen.SetActive(false);
        }

        public void HideLoseScreen()
        {
            loseScreen.SetActive(false);
        }

        private void UpdateMaxManaUI(int newMaxMana)
        {
            maxMana.text = newMaxMana.ToString();
        }

        private void UpdateCurrentManaUI(int newCurrentMana)
        {
            currentMana.text = newCurrentMana.ToString();
        }

        public void DisableEndTurnButton()
        {
            endTurnButton.interactable = false;
        }

        public void EnableEndTurnButton()
        {
            endTurnButton.interactable = true;
        }

        public void OnHandChange(int handCount)
        {
            if (handCount == 0)
            {
                ToggleSneezeButton(false);
                ToggleShredButton(false);
            }
            else
            {
                ToggleSneezeButton(BattleManager.Instance.DoABigSneezeAvailable);
                ToggleShredButton(BattleManager.Instance.ShredACardAvailable);
            }
        }

        public void ToggleShredButton(bool toggle)
        {
            shredButton.interactable = toggle;
        }

        public void ToggleSneezeButton(bool toggle)
        {
            sneezeButton.interactable = toggle;
        }

        public void OpenShopView()
        {
            treatAmountText.text = GameManager.Instance.party.TreatAmount.ToString();
            shopView.gameObject.SetActive(true);
        }

        public void CloseShopView()
        {
            shopView.gameObject.SetActive(false);
        }

        public void FadeIn()
        {
            fadeBlack.DOFade(0, 0.3f).SetEase(Ease.InOutBack);
        }

        public void SelectFirstAvailableAlly()
        {
            ClearGlows();
            allies.Where(x=>x.gameObject.activeInHierarchy).FirstOrDefault().SelectThisCharacter();
        }


        public void ClearGlows()
        {
            allies.ForEach(x=>x.selectGlow.SetActive(false));
        }

        public void UpdateTargetGlows(Target target, CharacterUI character)
        {
            allies.ForEach(x => x.TargetGlow.gameObject.SetActive(false));
            enemies.ForEach(x => x.TargetGlow.gameObject.SetActive(false));

            switch (target)
            {
                case Target.None:
                    break;
                case Target.SingleAlly:
                    if(character!= null && character.CurrentCharacter.characterType == CharacterType.Ally) { character.TargetGlow.gameObject.SetActive(true); }
                    break;
                case Target.SingleEnemy:
                    if (character != null && character.CurrentCharacter.characterType == CharacterType.Enemy) { character.TargetGlow.gameObject.SetActive(true); }
                    break;
                case Target.AllAllies:
                    if(character.CurrentCharacter.characterType == CharacterType.Ally) allies.ForEach(x => x.TargetGlow.gameObject.SetActive(true));
                    break;
                case Target.AllEnemies:
                    if (character.CurrentCharacter.characterType == CharacterType.Enemy) enemies.ForEach(x => x.TargetGlow.gameObject.SetActive(true));
                    break;
                case Target.AllCharacters:
                    break;
                case Target.Self:
                    if (character != null && character.CurrentCharacter.characterType == CharacterType.Ally) { character.TargetGlow.gameObject.SetActive(true); }
                    break;
            }
        }

    }
}
