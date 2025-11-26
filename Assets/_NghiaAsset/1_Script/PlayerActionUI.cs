using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace Turnbase
{
    public class PlayerActionUI : MonoBehaviour
    {
        public Character Owner { get; private set; }

        public BattleManager battleManager;


        public Button attackButton;
        public Button skillButton;
        public Button parryButton;
        public Button confirmButton;
        public Button summomonButton;
        public Button actionButton; 
        public Button cancelButton;

        public GameObject playerActionsPanel;
        public GameObject playerActionsPanel2;
        private Character currentCharacter;


        [TabGroup("Skill")] public SkillEntryUI skillEntryPrefab;
        [TabGroup("Skill")] public GameObject PlayerSkillPanel;

        [TabGroup("Summon")] public SkillEntryUI summonEntryPrefab;
        [TabGroup("Summon")] public GameObject PlayerSummonPanel;

        private List<SkillEntryUI> instantiatedSkillEntries = new List<SkillEntryUI>();
        private List<SkillEntryUI> instantiatedSummonEntries = new List<SkillEntryUI>();

        private bool isWaitingForConfirmation = false;

        [Header("Parry UI")]
        public Image parryFillImage;
        public Sprite defaultParrySprite;
        public Sprite readyParrySprite;

        public Action OnParryAttempted;

        private Animator animator;

        [SerializeField] private Skill selectedSkillToConfirm;

        void Awake()
        {
            GameObject actionPanel2 = GameObject.Find("PlayerAction2");
            if (actionPanel2 != null)
            {
                playerActionsPanel2 = actionPanel2;
            }

            battleManager = FindFirstObjectByType<BattleManager>();

        }

        void Update()
        {
            if (isWaitingForConfirmation)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Button activeConfirmButton = null;

                    if (actionButton != null && actionButton.gameObject.activeInHierarchy)
                    {
                        activeConfirmButton = actionButton;
                    }
                    else if (confirmButton != null && confirmButton.gameObject.activeInHierarchy)
                    {
                        activeConfirmButton = confirmButton;
                    }

                    if (activeConfirmButton != null)
                    {
                        OnConfirmClicked();
                    }
                }
            }
        }

        private void Start()
        {
            EventBus<OffUIAction>.Raise(new OffUIAction(panelName: "PlayerAction2"));

            animator = Owner.GetComponent<Animator>();

            attackButton.onClick.AddListener(OnAttackClicked);
            skillButton.onClick.AddListener(OnSkillClicked);
            parryButton.onClick.AddListener(OnParryClicked);
            confirmButton.onClick.AddListener(OnConfirmClicked);
            summomonButton.onClick.AddListener(OnSummonClicked);


            PlayerSkillPanel.gameObject.SetActive(false);
            PlayerSummonPanel.gameObject.SetActive(false);


            Hide();
        }

        public void SetOwner(Character owner)
        {
            Owner = owner;
            currentCharacter = owner;

            if (Owner != null)
            {
                battleManager = Owner.battleManager;
            }

            Hide();
        }

        public void ShowUI()
        {
            StartCoroutine(ShowDelay());
        }

        private IEnumerator ShowDelay()
        {
            yield return new WaitForSeconds(1f);

            playerActionsPanel.SetActive(true);
            confirmButton.gameObject.SetActive(false);
            if (parryButton != null) parryButton.gameObject.SetActive(false);
            if (parryFillImage != null) parryFillImage.gameObject.SetActive(false);

            if (summomonButton != null)
            {
                bool isSummoner = Owner != null && Owner.characterClass == CharacterClass.Summon;
                summomonButton.gameObject.SetActive(isSummoner);
            }

        }

        public void OnCancelClicked()
        {
            isWaitingForConfirmation = false;
            selectedSkillToConfirm = null;

            CameraAction.instance.NormalCamera(currentCharacter);

            animator.Play("Idle");

            if (currentCharacter != null && currentCharacter.isPlayer)
            {
                currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.waitingState);
            }

            playerActionsPanel.SetActive(true);
            PlayerSkillPanel.SetActive(false);
            PlayerSummonPanel.SetActive(false);
            confirmButton.gameObject.SetActive(false);

            EventBus<OffUIAction>.Raise(new OffUIAction(panelName: "PlayerAction2"));
        }


        public void Hide()
        {
            isWaitingForConfirmation = false;

            playerActionsPanel.SetActive(false);
            PlayerSkillPanel.SetActive(false);
            PlayerSummonPanel.SetActive(false);

            if (parryButton != null) parryButton.gameObject.SetActive(false);
            if (parryFillImage != null) parryFillImage.gameObject.SetActive(false);
        }

        public void ShowParryUI(bool showParry)
        {
            if (parryFillImage != null)
            {
                parryFillImage.gameObject.SetActive(showParry);
            }
            if (parryButton != null)
            {
                parryButton.gameObject.SetActive(showParry);
            }
        }

        public void SetParrySprite(bool ready)
        {
            if (parryFillImage != null)
            {
                if (ready)
                {
                    parryFillImage.sprite = readyParrySprite;
                }
                else
                {
                    parryFillImage.sprite = defaultParrySprite;
                }
            }
        }

        public void UpdateParryFill(float normalizedValue)
        {
            if (parryFillImage != null)
            {
                parryFillImage.fillAmount = normalizedValue;
            }
        }

        public void SetActiveCharacter(Character character)
        {
            currentCharacter = character;
        }

        private void OnAttackClicked()
        {
            isWaitingForConfirmation = true;
            selectedSkillToConfirm = null;

            Debug.Log("OnAttackClicked được gọi.");

            animator.Play("Idle");

            CameraAction.instance.NormalAttack(currentCharacter, false);

            if (currentCharacter != null && currentCharacter.isPlayer)
            {
                currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.readyState);
                confirmButton.gameObject.SetActive(true);
            }

            PlayerSkillPanel.SetActive(false);
            PlayerSummonPanel.SetActive(false);
        }

        private void OnSkillClicked()
        {
            isWaitingForConfirmation = false;

            switch (currentCharacter.characterClass)
            {
                case CharacterClass.Sword_Shield:
                    animator.Play("Warrio_Cast");
                    break;
                case CharacterClass.Magical:
                    animator.Play("Magic_Cast");
                    break;
                case CharacterClass.Summon:
                    animator.Play("Summon_Cast");
                    break;
                default:
                    animator.Play("Magic_Cast");
                    break;
            }

            ApplyButtonAction_Cancel();


            CameraAction.instance.ReadySkill(currentCharacter);

            PlayerSummonPanel.SetActive(false);


            confirmButton.gameObject.SetActive(false);

            if (PlayerSkillPanel.activeSelf == true)
            {
                PlayerSkillPanel.SetActive(false);
            }
            else
            {
                SetupSkillUI(currentCharacter.skills);
                PlayerSkillPanel.SetActive(true);
            }
        }


        public void SetupSummonUI(List<Skill> skills)
        {
            List<Skill> summonSkills = skills.Where(s => s.skillType == SkillType.Summon).ToList();

            foreach (var entry in instantiatedSummonEntries)
            {
                Destroy(entry.gameObject);
            }
            instantiatedSummonEntries.Clear();

            if (summonEntryPrefab == null)
            {
                Debug.LogError("Summon Entry Prefab chưa được gán!");
                return;
            }

            foreach (Skill skillToUse in summonSkills)
            {
                SkillEntryUI newEntry = Instantiate(summonEntryPrefab, PlayerSummonPanel.transform);

                newEntry.Setup(skillToUse, OnSkillButtonClicked);

                instantiatedSummonEntries.Add(newEntry);
            }
        }
        private void OnSummonClicked()
        {

            isWaitingForConfirmation = false;
            selectedSkillToConfirm = null;

            Debug.Log("sử dụng Triệu hồi!");
            SetupSummonUI(currentCharacter.skills);

            CameraAction.instance.ReadySkill(currentCharacter);

            ApplyButtonAction_Cancel();

            PlayerSummonPanel.SetActive(true);
            PlayerSkillPanel.SetActive(false);
            confirmButton.gameObject.SetActive(false);

            EventBus<OnUIAction>.Raise(new OnUIAction(panelName: "PlayerAction2"));
        }

        private void OnSkillButtonClicked(Skill selectedSkill)
        {
            if (currentCharacter == null) return;

            isWaitingForConfirmation = true;

            EventBus<OnUIAction>.Raise(new OnUIAction(panelName: "PlayerAction2"));

            playerActionsPanel.SetActive(false);

            PlayerSkillPanel.SetActive(false);
            PlayerSummonPanel.SetActive(false);


            currentCharacter.stateMachine.SwitchState(
                new ReadyStateSkill(currentCharacter.stateMachine, selectedSkill)
            );

            selectedSkillToConfirm = selectedSkill;

            if (actionButton != null) actionButton.gameObject.SetActive(true);
        }

        private void ApplyButtonAction_Cancel()
        {
            GameObject actionObject = GameObject.Find("Action");
            GameObject cancelObject = GameObject.Find("Cancel");
            if (actionObject != null)
            {
                actionButton = actionObject.GetComponent<Button>();
            }

            if (cancelObject != null)
            {
                cancelButton = cancelObject.GetComponent<Button>();
            }

            if (actionButton != null)
            {
                actionButton.onClick.RemoveAllListeners();
                actionButton.onClick.AddListener(OnConfirmClicked);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCancelClicked);
            }


        }


        public void SetupSkillUI(List<Skill> skills)
        {
            List<Skill> damageSkills = skills.Where(s => s.skillType != SkillType.Summon).ToList();

            foreach (var entry in instantiatedSkillEntries)
            {
                Destroy(entry.gameObject);
            }
            instantiatedSkillEntries.Clear();

            if (skillEntryPrefab == null)
            {
                Debug.LogError("Skill Entry Prefab chưa được gán!");
                return;
            }

            foreach (Skill skillToUse in damageSkills)
            {
                SkillEntryUI newEntry = Instantiate(skillEntryPrefab, PlayerSkillPanel.transform);
                newEntry.Setup(skillToUse, OnSkillButtonClicked);

                instantiatedSkillEntries.Add(newEntry);
            }
        }

        private void OnParryClicked()
        {
            Debug.Log("Nút Parry được nhấn.");

            if (parryButton != null)
            {
                parryButton.gameObject.SetActive(false);
            }

            OnParryAttempted?.Invoke();
        }

        private void OnConfirmClicked()
        {
            isWaitingForConfirmation = false;

            Debug.Log("OnConfirmClicked được gọi.");

            EventBus<OffUIAction>.Raise(new OffUIAction(panelName: "PlayerAction2"));

            EventBus<HidePanelEvent>.Raise(new HidePanelEvent(panelName: "PlayerPanelControll"));

            if (currentCharacter == null) return;

            if (currentCharacter.stateMachine.currentState is ReadyStateSkill currentState)
            {
                Debug.Log("Gọi OnConfirm() của ReadyStateSkill.");

                if (selectedSkillToConfirm == null)
                {
                    Debug.LogError("selectedSkillToConfirm bị null khi ở ReadyStateSkill! Hành động bị hủy.");
                    currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.waitingState);
                    return;
                }

                int manaCost = selectedSkillToConfirm.manaCost;

                if (currentCharacter.stats.currentMP >= manaCost)
                {
                    currentCharacter.stats.currentMP -= manaCost;
                    currentCharacter.battleUIManager.UpdateCharacterUI(currentCharacter);

                    currentState.OnConfirm();
                }
                else
                {
                    Debug.LogWarning($"{currentCharacter.name} không đủ Mana ({manaCost}) để dùng kỹ năng {selectedSkillToConfirm.skillName}!");
                    currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.waitingState);
                }

            }
            else if (currentCharacter.stateMachine.currentState is ReadyState)
            {
                Debug.Log("Chuyển từ ReadyState sang AttackingState.");
                currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.attackingState);
            }

            PlayerSkillPanel.SetActive(false);
            PlayerSummonPanel.SetActive(false);
            playerActionsPanel.SetActive(false);

            selectedSkillToConfirm = null;
        }
    }

}