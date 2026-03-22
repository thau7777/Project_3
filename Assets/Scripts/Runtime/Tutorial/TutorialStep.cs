using UnityEngine;
using UnityEngine.InputSystem;

// ─────────────────────────────────────────────────────────────
//  TutorialStep  (ScriptableObject)
//  Create via: Right-click → Tutorial / Step
//
//  One step = one "screen" of instruction.
//  Add as many steps as you want to a TutorialSequence.
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(fileName = "NewTutorialStep", menuName = "Scriptable Objects/Tutorial/Step")]
public class TutorialStep : ScriptableObject
{
    [Header("Text Content")]
    [Tooltip("Main instruction text.")]
    [TextArea(3, 6)]
    public string description = "This is your health bar";

    [Tooltip("Bold header shown at the top of the tutorial box.")]
    public string buttonToPressText = "Press [E] to continue.";

    // ── Positioning ────────────────────────────────────────────
    [Header("Text Box Position")]
    [Tooltip("Extra offset in SCREEN pixels applied on top of the anchor position.\n" +
             "Positive X = right, Positive Y = up.\n" +
             "If no anchor is set on the TutorialTrigger, this offset is applied\n" +
             "from the screen-space fallback position instead.")]
    public Vector2 positionOffset = Vector2.zero;

    // ── Input ──────────────────────────────────────────────────
    [Header("Advance Condition")]
    [Tooltip("If true, ANY key or button press will advance to the next step.\n" +
             "Overrides both waitForKeyPress and overrideContinueAction.")]
    public bool waitForAnyKey = false;

    [Tooltip("If true, the player must click on a highlighted target to advance.")]
    [ShowIf("waitForAnyKey", true)]
    public bool waitForMouseClick = false;

    [Tooltip("If true the player must press the action to move to the next step.\n" +
             "If false the step auto-advances after autoAdvanceDelay seconds.")]
    [ShowIf("waitForAnyKey", true)]
    public bool waitForKeyPress = true;

    [Tooltip("The exact name of the action inside the asset above (e.g. 'Jump', 'Interact').\n" +
             "Case-sensitive. Leave empty to use the TutorialManager's global Continue action.")]
    [ShowIf("waitForAnyKey", true)]
    public string overrideActionName;
    [ShowIf("waitForAnyKey", true)]
    public bool blockTheSameInputAction = false;

    private bool _isNotUseKey = false;

    [Tooltip("Only used when waitForKeyPress = false.")]
    [ShowIf("_isNotUseKey")]
    public float autoAdvanceDelay = 2f;

    [Header("Override Continue Action (optional)")]
    [Tooltip("The Input Action Asset that contains the override action.\n" +
             "Leave empty to use the TutorialManager's global Continue action.")]
    public InputActionAsset overrideActionAsset;


    // ── Runtime Helper ─────────────────────────────────────────
    /// <summary>
    /// Returns the resolved InputAction for this step's override, or null if none is set.
    /// Call this at runtime (not in the editor) to get the live action.
    /// </summary>

    public void OnValidate()
    {
        _isNotUseKey = !waitForAnyKey && !waitForKeyPress && !waitForMouseClick; // ← update this
    }
    public InputAction GetOverrideAction()
    {
        if (overrideActionAsset == null || string.IsNullOrEmpty(overrideActionName))
            return null;

        InputAction action = overrideActionAsset.FindAction(overrideActionName, throwIfNotFound: false);

        if (action == null)
            Debug.LogWarning($"[TutorialStep] Action '{overrideActionName}' not found in '{overrideActionAsset.name}'.");

        return action;
    }
}