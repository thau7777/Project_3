using Cysharp.Threading.Tasks;
using MyRule;
using UnityEngine;
using UnityEngine.Events;

// ─────────────────────────────────────────────────────────────
//  TutorialStepBinding
//
//  Pairs with a TutorialStep by array index.
//  Holds all scene-object references that can't live in a
//  ScriptableObject (GameObjects, Transforms, etc.).
// ─────────────────────────────────────────────────────────────
[System.Serializable]
public class TutorialStepBinding
{
    [Tooltip("Objects to lift above the dark overlay for this step.\n" +
             "UI objects are reparented into the Highlight Canvas.\n" +
             "World objects need a TutorialHighlightTarget component.")]
    public GameObject[] highlightTargets;

    [Tooltip("Override the trigger's shared anchor for just this step.\n" +
             "Leave empty to use the trigger's main Anchor Target.")]
    public Transform anchorOverride;

    public string[] targetIdKeys;

}

// ─────────────────────────────────────────────────────────────
//  TutorialTrigger  (MonoBehaviour)
//
//  Drop this anywhere in your scene and assign a TutorialSequence.
//
//  Trigger modes
//  ─────────────
//  OnStart        – plays as soon as the scene loads
//  OnTriggerEnter – plays when the player walks into a 2D/3D trigger volume
//                   (set "Player Tag" to your player's tag)
//  Manual         – call Trigger() from code or a UnityEvent / Button.OnClick
// ─────────────────────────────────────────────────────────────
public class TutorialTrigger : MonoBehaviour
{
    public enum TriggerMode { OnStart, OnTriggerEnter3D, Manual }

    [Header("Sequence")]
    [Tooltip("The TutorialSequence ScriptableObject this trigger will play.")]
    public TutorialSequence sequence;

    [Header("Trigger Mode")]
    public TriggerMode triggerMode = TriggerMode.OnStart;
    [SerializeField]
    [ShowIfEnumValue("triggerMode", TriggerMode.OnStart)]
    private int _onStartTriggerDelayDuration = 1;

    [Tooltip("Tag of the GameObject that activates this trigger (usually 'Player').")]
    [ShowIfEnumValue("triggerMode", TriggerMode.OnTriggerEnter3D)]
    public string playerTag = "Player";

    [Header("Text Box Anchor (shared across all steps)")]
    [Tooltip("The mechanic object the tutorial box appears near by default.\n" +
             "Works with world-space objects and UI RectTransforms.\n" +
             "Leave empty to use the fallback position set on TutorialManager.\n" +
             "Individual steps can override this via Step Bindings → Anchor Override.")]
    public Transform anchorTarget;

    [Header("Step Bindings  (scene objects per step)")]
    [Tooltip("One entry per step in your TutorialSequence, matched by index.\n" +
             "Assign the scene objects that should be highlighted during each step.\n" +
             "You can leave entries empty if a step highlights nothing.")]
    public TutorialStepBinding[] stepBindings;

    [Header("Events (optional)")]
    [Tooltip("Called right before the sequence starts.")]
    public UnityEvent onBeforeSequenceStart;

    [Tooltip("Fires when the sequence finishes (or is skipped because playOnce was set).")]
    public UnityEvent onSequenceEnd;

    // ── Lifecycle ──────────────────────────────────────────────
    private async void Start()
    {
        if (triggerMode == TriggerMode.OnStart) 
        {
            await UniTask.Delay(_onStartTriggerDelayDuration * 1000);
            Trigger();
        }

        onSequenceEnd.AddListener(SaveData);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerMode == TriggerMode.OnTriggerEnter3D && other.CompareTag(playerTag))
            Trigger();
    }


    // ── Public API ─────────────────────────────────────────────
    public void ManualTriggerSetup(Transform anchorTarget, TutorialStepBinding[] stepBindings)
    {
        this.anchorTarget = anchorTarget;
        this.stepBindings = stepBindings;
    }

    /// <summary>
    /// Fire this tutorial from anywhere:
    ///   • Drag the TutorialTrigger into a UnityEvent slot and pick Trigger()
    ///   • Call   myTutorialTrigger.Trigger()   from any script
    ///   • Wire it to a Button.onClick
    /// </summary>
    public void Trigger()
    {
        if (GameSystemManager.Instance != null && 
            GameSystemManager.Instance.GameData.Tutorial.HasCompletedTutorial(sequence.id)) 
            return;

        if (TutorialManager.Instance == null)
        {
            Debug.LogError("[TutorialTrigger] No TutorialManager found in scene!");
            return;
        }

        //if (sequence != null && sequence.playOnce && sequence.hasPlayed)
        //{
        //    onSequenceEnd?.Invoke();
        //    return;
        //}

        onBeforeSequenceStart?.Invoke();
        TutorialManager.Instance.PlaySequence(sequence, anchorTarget, stepBindings);

        StartCoroutine(WaitForCompletion());
    }

    private System.Collections.IEnumerator WaitForCompletion()
    {
        yield return null;
        while (TutorialManager.Instance != null && TutorialManager.Instance.IsRunning)
            yield return null;
        
        yield return new WaitForSeconds(0.5f);
        onSequenceEnd?.Invoke();
    }

    private void SaveData()
    {
        if (GameSystemManager.Instance != null)
        {
            GameSystemManager.Instance.GameData.Tutorial.Tutorials[sequence.id] = true;
        }
    }
}