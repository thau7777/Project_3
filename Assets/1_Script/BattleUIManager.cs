using System.Collections.Generic;
using Turnbase;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject AvatarGroupPrefab;
    public Transform UIContainer;


    private Dictionary<Character, AvatarGroup> characterToUI = new Dictionary<Character, AvatarGroup>();


    public void SpawnCharacterUI(Character character)
    {
        if (AvatarGroupPrefab == null || UIContainer == null)
        {
            Debug.LogError("Cần gán AvatarGroup Prefab VÀ UI Container trong BattleManager!");
            return;
        }

        GameObject uiInstance = Instantiate(AvatarGroupPrefab, UIContainer);

        AvatarGroup uiGroup = uiInstance.GetComponent<AvatarGroup>();
        if (uiGroup != null)
        {
            uiGroup.SetOwner(character);

            characterToUI.Add(character, uiGroup);

            uiGroup.UpdateUI(character.stats);
        }
    }

    public void UpdateCharacterUI(Character character)
    {
        if (characterToUI.TryGetValue(character, out AvatarGroup uiGroup))
        {
            uiGroup.UpdateUI(character.stats);
        }
    }

    public void UpdateAllCharacterUIs(List<Character> combatants)
    {
        foreach (var pair in characterToUI)
        {
            pair.Value.UpdateUI(pair.Key.stats);
        }

    }

    public void HideParryUI(Character character)
    {
        // Giả định logic Parry UI nằm trong PlayerActionUI (ownUI)
        if (character.ownUI != null)
        {
            character.ownUI.ShowParryUI(false);
            character.ownUI.SetParrySprite(false);
        }
    }

}
