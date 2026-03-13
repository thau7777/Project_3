using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;



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
        public Button itemButton;
        public Button actionButton; 
        public Button cancelButton;

        public GameObject playerActionsPanel;
        public GameObject playerActionsPanel2;
        private Character currentCharacter;


        [TabGroup("Skill")] public SkillEntryUI skillEntryPrefab;
        [TabGroup("Skill")] public GameObject PlayerSkillPanel;

        [TabGroup("Summon")] public SkillEntryUI summonEntryPrefab;
        [TabGroup("Summon")] public GameObject PlayerSummonPanel;

        [TabGroup("Item")] public ItemEntryUI itemEntryPrefab;
        [TabGroup("Item")] public GameObject PlayerItemPanel;


        private List<SkillEntryUI> instantiatedSkillEntries = new List<SkillEntryUI>();
        private List<SkillEntryUI> instantiatedSummonEntries = new List<SkillEntryUI>();
        private List<ItemEntryUI> instantiatedItemEntries = new List<ItemEntryUI>();

        private bool isWaitingForConfirmation = false;

        [Header("Parry UI")]
        public Image parryFillImage;
        public Sprite defaultParrySprite;
        public Sprite readyParrySprite;

        public Action OnParryAttempted;

        private Animator animator;

        [SerializeField] private Skill selectedSkillToConfirm;

        private PlayerTurnBasedActions inputLogic;

        void Awake()
        {
            GameObject actionPanel2 = GameObject.Find("PlayerAction2");
            if (actionPanel2 != null)
            {
                playerActionsPanel2 = actionPanel2;
            }

            battleManager = FindFirstObjectByType<BattleManager>();


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
            itemButton.onClick.AddListener(OnItemClicked);

            PlayerSkillPanel.gameObject.SetActive(false);
            PlayerSummonPanel.gameObject.SetActive(false);
            PlayerItemPanel.gameObject.SetActive(false);

            

            Hide();
        }

        public void SetOwner(Character owner)
        {
            if (inputLogic != null)
            {
                inputLogic.QEvent -= OnAttackClicked;
                inputLogic.EEvent -= OnSkillClicked;
                inputLogic.REvent -= OnItemClicked;
                inputLogic.SpaceEvent -= OnConfirmClicked;
                inputLogic.SummonEvent -= OnSummonClicked;
            }

            Owner = owner;
            currentCharacter = owner;

            if (Owner != null)
            {
                battleManager = Owner.battleManager;
                inputLogic = Owner.stateMachine.inputLogic;

                if (inputLogic != null)
                {
                    inputLogic.QEvent += OnAttackClicked;   // Phím Q: Tấn công
                    inputLogic.EEvent += OnSkillClicked;    // Phím E: Kỹ năng
                    inputLogic.REvent += OnItemClicked;     // Phím R: Vật phẩm
                    inputLogic.SpaceEvent += OnConfirmClicked; // Phím Space: Xác nhận
                    inputLogic.SummonEvent += OnSummonClicked;      // Phím F: Triệu hồi

                }
            }

            Hide();
        }

        private void OnDestroy()
        {
            if (inputLogic != null)
            {
                inputLogic.QEvent -= OnAttackClicked;
                inputLogic.EEvent -= OnSkillClicked;
                inputLogic.REvent -= OnItemClicked;
                inputLogic.SpaceEvent -= OnConfirmClicked;
                inputLogic.SummonEvent -= OnSummonClicked;
            }
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

            attackButton.interactable = true;
            skillButton.interactable = true;

            CameraAction.instance.NormalCamera(currentCharacter);

            animator.Play("Idle");

            if (currentCharacter != null && currentCharacter.isPlayer)
            {
                currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.waitingState);
            }

            playerActionsPanel.SetActive(true);
            PlayerSkillPanel.SetActive(false);
            PlayerSummonPanel.SetActive(false);
            PlayerItemPanel.SetActive(false);

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
            if (!playerActionsPanel.activeInHierarchy) return;

            isWaitingForConfirmation = true;
            selectedSkillToConfirm = null;

            attackButton.interactable = false;
            skillButton.interactable = true;
            summomonButton.interactable = true;
            itemButton.interactable = true;


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
            PlayerItemPanel.SetActive(false);

        }

        private void OnSkillClicked()
        {
            if (!playerActionsPanel.activeInHierarchy) return;

            bool hasEnoughManaForAnySkill = currentCharacter.skills.Any(s => s.manaCost <= currentCharacter.stats.currentMP);

            if (!hasEnoughManaForAnySkill)
            {
                Debug.LogWarning("Không đủ Mana để sử dụng bất kỳ kỹ năng nào!");
                return;
            }

            isWaitingForConfirmation = false;

            attackButton.interactable = true;
            skillButton.interactable = false;
            summomonButton.interactable = true;
            itemButton.interactable = true;

            Debug.Log("sử dụng Kỹ năng!");


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
            PlayerItemPanel.SetActive(false);
            confirmButton.gameObject.SetActive(false);

            SetupSkillUI(currentCharacter.skills);
            PlayerSkillPanel.SetActive(true);

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

        private void SetUPItemUI(List<Tb_Item> items)
        {
            foreach (var entry in instantiatedItemEntries)
            {
                if (entry != null) Destroy(entry.gameObject);
            }
            instantiatedItemEntries.Clear();

            if (itemEntryPrefab == null)
            {
                Debug.LogError("Item Entry Prefab chưa được gán trong Inspector!");
                return;
            }

            foreach (Tb_Item itemToUse in items)
            {
                if (itemToUse.quantity > 0)
                {
                    ItemEntryUI newEntry = Instantiate(itemEntryPrefab, PlayerItemPanel.transform);

                    newEntry.SetUp(itemToUse, OnItemButtonClicked);

                    instantiatedItemEntries.Add(newEntry);
                }
            }
        }        
        

        private void OnSummonClicked()
        {

            isWaitingForConfirmation = false;
            selectedSkillToConfirm = null;

            attackButton.interactable = true;
            skillButton.interactable = true;
            summomonButton.interactable = false;
            itemButton.interactable = true;


            Debug.Log("sử dụng Triệu hồi!");

            animator.Play("Summon_Cast");

            SetupSummonUI(currentCharacter.skills);

            CameraAction.instance.ReadySkill(currentCharacter);

            ApplyButtonAction_Cancel();

            PlayerSummonPanel.SetActive(true);
            PlayerSkillPanel.SetActive(false);
            PlayerItemPanel.SetActive(false);
            confirmButton.gameObject.SetActive(false);

        }

        private void OnItemButtonClicked(Tb_Item selectedItem)
        {
            if (currentCharacter == null) return;

            isWaitingForConfirmation = true;

            EventBus<OnUIAction>.Raise(new OnUIAction(panelName: "PlayerAction2"));

            playerActionsPanel.SetActive(false);
            PlayerItemPanel.SetActive(false);

            currentCharacter.stateMachine.SwitchState(new ReadyStateItem(currentCharacter.stateMachine, selectedItem));

            if (actionButton != null) actionButton.gameObject.SetActive(true);

            ApplyButtonAction_Cancel();
        }

        private void OnItemClicked()
        {
            isWaitingForConfirmation = false;
            attackButton.interactable = true;
            skillButton.interactable = true;
            summomonButton.interactable = true;
            itemButton.interactable = false;

            Debug.Log("sử dụng Item!");
            SetUPItemUI(currentCharacter.item);

            CameraAction.instance.ReadyUseItem(currentCharacter);

            PlayerSummonPanel.SetActive(false);
            PlayerSkillPanel.SetActive(false);
            PlayerItemPanel.SetActive(true);
            confirmButton.gameObject.SetActive(false);

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

                bool canAfford = currentCharacter.stats.currentMP >= skillToUse.manaCost;

                newEntry.Setup(skillToUse, OnSkillButtonClicked);

                Button btn = newEntry.GetComponent<Button>();
                if (btn != null)
                {
                    btn.interactable = canAfford;
                }

                instantiatedSkillEntries.Add(newEntry);
            }
            if (instantiatedSkillEntries.Count > 0)
            {
                StartCoroutine(FocusFirstSkill());
            }
        }

        private IEnumerator FocusFirstSkill()
        {
            yield return new WaitForEndOfFrame();
            if (instantiatedSkillEntries.Count > 0)
            {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(instantiatedSkillEntries[0].gameObject);
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
            if (PlayerSkillPanel.activeInHierarchy || PlayerSummonPanel.activeInHierarchy || PlayerItemPanel.activeInHierarchy)
            {
                GameObject currentSelected = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
                if (currentSelected != null)
                {
                    // Thử lấy component SkillEntry hoặc ItemEntry
                    var skillEntry = currentSelected.GetComponent<SkillEntryUI>();
                    if (skillEntry != null) { skillEntry.SelectThisSkill(); return; }

                    var itemEntry = currentSelected.GetComponent<ItemEntryUI>();
                    if (itemEntry != null) { itemEntry.SelectThisItem(); return; } 
                }
            }

            if (!isWaitingForConfirmation) return;

            isWaitingForConfirmation = false;

            attackButton.interactable = true;
            skillButton.interactable = true;
            summomonButton.interactable = true;
            itemButton.interactable = true;

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
            else if (currentCharacter.stateMachine.currentState is ReadyStateItem itemState)
            {
                Tb_Item selectedItem = itemState.SelectedItem;

                if (selectedItem != null)
                {
                    itemState.OnConfirm();
                }
                else
                {
                    Debug.LogWarning("Không tìm thấy vật phẩm trong State!");
                    currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.waitingState);
                }
            }
            else if (currentCharacter.stateMachine.currentState is ReadyState)
            {
                currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.attackingState);
            }

            PlayerSkillPanel.SetActive(false);
            PlayerSummonPanel.SetActive(false);
            playerActionsPanel.SetActive(false);
            PlayerItemPanel.SetActive(false);

            selectedSkillToConfirm = null;
        }
    }

}