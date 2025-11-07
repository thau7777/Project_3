using System.Collections.Generic;
using System.Linq;
using Turnbase;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject AvatarGroupPrefab;
    public Transform UIContainer;

    [Header("Combatant List UI")]
    public GameObject combatantButtonPrefab;
    public Transform combatantButtonContainer;
    public Button toggleButton;

    private CharacterStatUI statDisplayPanel;
    private BattleManager battleManager;
    private bool isShowingPlayers = true;

    private Dictionary<Character, AvatarGroup> characterToUI = new Dictionary<Character, AvatarGroup>();


    public void InitializeCombatantButtons(List<Character> allCombatants, CharacterStatUI statUI, BattleManager bm)
    {
        this.statDisplayPanel = statUI;
        this.battleManager = bm;

        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveAllListeners();
            toggleButton.onClick.AddListener(OnToggleButtonClicked);
        }

        SpawnCombatantButtons(isShowingPlayers, allCombatants);
    }


    public void SpawnCharacterUI(Character character)
    {
        if (character.isPlayer)
        {
            if (AvatarGroupPrefab == null || UIContainer == null)
            {
                return;
            }

            GameObject uiInstance = Instantiate(AvatarGroupPrefab, UIContainer);

            AvatarGroup uiGroup = uiInstance.GetComponent<AvatarGroup>();
            if (uiGroup != null)
            {
                uiGroup.SetOwner(character);

                characterToUI.Add(character, uiGroup);

                uiGroup.UpdateUI(character.stats, character.info);
            }
        }
        else
        {
            AvatarGroup uiGroup = character.GetComponentInChildren<AvatarGroup>();
            if (uiGroup != null)
            {
                uiGroup.SetOwner(character);
                if (!characterToUI.ContainsKey(character))
                {
                    characterToUI.Add(character, uiGroup);
                }
                uiGroup.UpdateUI(character.stats, character.info);
            }
        }
    }
    public void UpdateCharacterUI(Character character)
    {
        if (characterToUI.TryGetValue(character, out AvatarGroup uiGroup))
        {
            uiGroup.UpdateUI(character.stats, character.info);
        }
    }

    public void UpdateAllCharacterUIs(List<Character> combatants)
    {
        foreach (var pair in characterToUI)
        {
            pair.Value.UpdateUI(pair.Key.stats, pair.Key.info);
        }

    }

    public void HideParryUI(Character character)
    {
        if (character.ownUI != null)
        {
            character.ownUI.ShowParryUI(false);
            character.ownUI.SetParrySprite(false);
        }
    }

    public void OnToggleButtonClicked()
    {
        if (battleManager == null)
        {
            return;
        }

        isShowingPlayers = !isShowingPlayers;
        SpawnCombatantButtons(isShowingPlayers, battleManager.allCombatants);
    }

    public void SpawnCombatantButtons(bool showPlayers, List<Character> allCombatants)
    {
        if (combatantButtonPrefab == null || combatantButtonContainer == null || statDisplayPanel == null)
        {
            return;
        }

        foreach (Transform child in combatantButtonContainer)
        {
            Destroy(child.gameObject);
        }

        var filteredCombatants = allCombatants
            .Where(c => c.isPlayer == showPlayers && c.isAlive)
            .ToList();

        Character firstCombatant = null;

        foreach (Character combatant in filteredCombatants)
        {
            GameObject buttonGO = Instantiate(combatantButtonPrefab, combatantButtonContainer);
            CombatantButton combatantButton = buttonGO.GetComponent<CombatantButton>();

            if (combatantButton != null)
            {
                combatantButton.Setup(combatant, statDisplayPanel);
            }

            if (firstCombatant == null)
            {
                firstCombatant = combatant;
            }
        }

        if (statDisplayPanel != null)
        {
            if (firstCombatant == null)
            {
                statDisplayPanel.HideStats();
            }
            
        }
    }
}