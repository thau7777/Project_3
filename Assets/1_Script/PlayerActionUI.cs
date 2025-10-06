using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class PlayerActionUI : MonoBehaviour
{    public Character Owner { get; private set; }

    public BattleManager battleManager;

    public Button attackButton;
    public Button skillButton;
    public Button parryButton;
    public Button confirmButton;
    public Button summomonButton;
    public Button confirmButton2;
    public Button cancelButton;

    public GameObject playerActionsPanel;
    public GameObject playerActionsPanel2;
    private Character currentCharacter;

    
    [TabGroup("Skill")]public List<Button> skillButtons;
    [TabGroup("Skill")]public GameObject PlayerSkillPanel;

    [TabGroup("Summon")]public List<Button> summonButtons;
    [TabGroup("Summon")]public GameObject PlayerSummonPanel;



    [Header("Parry UI")]
    public Image parryFillImage;
    public Sprite defaultParrySprite;
    public Sprite readyParrySprite;

    public Action OnParryAttempted;

    private Skill selectedSkillToConfirm;

    void Awake()
    {
        playerActionsPanel2 = GameObject.Find("PlayerAction2");

        battleManager = FindFirstObjectByType<BattleManager>();

    }

    private void Start()
    {
        EventBus<OffUIAction>.Raise(new OffUIAction(panelName: "PlayerAction2"));

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

    }

    public void OnCancelClicked()
    {
        CameraAction.instance.NormalAttack(currentCharacter, true);

        if (currentCharacter != null && currentCharacter.isPlayer)
        {
            currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.waitingState);
        }

        playerActionsPanel.SetActive(true);
        PlayerSkillPanel.SetActive(false);
        confirmButton.gameObject.SetActive(false);

        EventBus<OffUIAction>.Raise(new OffUIAction(panelName: "PlayerAction2"));
        
    }


    public void Hide()
    {
        playerActionsPanel.SetActive(false);

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
        Debug.Log("OnAttackClicked được gọi.");

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
        Debug.Log("Sử dụng Kỹ năng!");

        GameObject actionObject = GameObject.Find("Action");
        GameObject cancelObject = GameObject.Find("Cancel");

        if (actionObject != null)
        {
            confirmButton2 = actionObject.GetComponent<Button>();
        }

        if (cancelObject != null)
        {
            cancelButton = cancelObject.GetComponent<Button>();
        }

        if (confirmButton2 != null)
        {
            confirmButton2.onClick.RemoveAllListeners();
            confirmButton2.onClick.AddListener(OnConfirmClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(OnCancelClicked);
        }

        CameraAction.instance.ReadySkill(currentCharacter);

        PlayerSummonPanel.SetActive(false);


        confirmButton.gameObject.SetActive(false);

        if (PlayerSkillPanel.activeSelf == true)
        {
            PlayerSkillPanel.SetActive(false);
        }
        else
        {
            PlayerSkillPanel.SetActive(true);
        }
    }

    public void SetupSummonUI(List<Skill> skills)
    {
        List<Skill> summonSkills = skills.Where(s => s.skillType == SkillType.Summon).ToList();
        foreach (var b in summonButtons) b.gameObject.SetActive(false);
        for (int i = 0; i < summonSkills.Count && i < summonButtons.Count; i++)
        {
            Button currentButton = summonButtons[i];
            currentButton.onClick.RemoveAllListeners();

            Skill skillToUse = summonSkills[i];
            Skill captured = skillToUse;
            currentButton.onClick.AddListener(() => OnSkillButtonClicked(captured));

            Image buttonImage = currentButton.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = currentButton.GetComponentInChildren<Image>();
            }

            if (buttonImage != null && skillToUse.icon != null)
            {
                buttonImage.sprite = skillToUse.icon;
                buttonImage.color = Color.white;
            }
            

            currentButton.gameObject.SetActive(true);
        }
    }

    private void OnSummonClicked()
    {
        Debug.Log("sử dụng Triệu hồi!");
        PlayerSummonPanel.SetActive(true);
        CameraAction.instance.ReadySkill(currentCharacter);

        SetupSkillUI(currentCharacter.skills);

        PlayerSummonPanel.SetActive(true);
        PlayerSkillPanel.SetActive(false);


    }

    private void OnSkillButtonClicked(Skill selectedSkill)
    {
        if (currentCharacter == null) return;

        OnSkillClicked();

        Debug.Log($"OnSkillButtonClicked được gọi với kỹ năng: {selectedSkill.skillName} cho {currentCharacter.name}");

        EventBus<OnUIAction>.Raise(new OnUIAction(panelName: "PlayerAction2"));

        playerActionsPanel.SetActive(false);


        currentCharacter.stateMachine.SwitchState(
            new ReadyStateSkill(currentCharacter.stateMachine, selectedSkill)
        );

        selectedSkillToConfirm = selectedSkill;
        confirmButton.gameObject.SetActive(true);
    }

    public void SetupSkillUI(List<Skill> skills)
    {
        Debug.Log("SetupSkillUI được gọi.");


        List<Skill> damageSkills = skills.Where(s => s.skillType != SkillType.Summon).ToList();

        foreach (var b in skillButtons) b.gameObject.SetActive(false);

        for (int i = 0; i < damageSkills.Count && i < skillButtons.Count; i++)
        {
            Button currentButton = skillButtons[i];
            currentButton.onClick.RemoveAllListeners();

            Skill skillToUse = damageSkills[i];
            Skill captured = skillToUse;
            currentButton.onClick.AddListener(() => OnSkillButtonClicked(captured));


            Image buttonImage = currentButton.GetComponent<Image>();
            if (buttonImage == null)
            {
                buttonImage = currentButton.GetComponentInChildren<Image>();
            }

            if (buttonImage != null && skillToUse.icon != null)
            {
                buttonImage.sprite = skillToUse.icon;
                buttonImage.color = Color.white;
            }
            else if (buttonImage != null)
            {
                buttonImage.sprite = null;
                buttonImage.color = new Color(1, 1, 1, 0);
            }

            currentButton.gameObject.SetActive(true);
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
        Debug.Log("OnConfirmClicked được gọi.");

        EventBus<OffUIAction>.Raise(new OffUIAction(panelName: "PlayerAction2"));


        //CameraAction.instance.ResetCamera();



        EventBus<HidePanelEvent>.Raise(new HidePanelEvent(panelName: "PlayerPanelControll"));

        if (currentCharacter == null) return;

        if (currentCharacter.stateMachine.currentState is ReadyStateSkill currentState)
        {
            Debug.Log("Gọi OnConfirm() của ReadyStateSkill.");
            currentState.OnConfirm();

            PlayerSkillPanel.SetActive(false);

        }
        else if (currentCharacter.stateMachine.currentState is ReadyState)
        {
            Debug.Log("Chuyển từ ReadyState sang AttackingState.");
            currentCharacter.stateMachine.SwitchState(currentCharacter.stateMachine.attackingState);

            PlayerSkillPanel.SetActive(false);
        }
    }
}
