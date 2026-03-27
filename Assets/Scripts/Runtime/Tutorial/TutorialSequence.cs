using UnityEngine;

// ─────────────────────────────────────────────────────────────
//  TutorialSequence  (ScriptableObject)
//  Create via: Right-click → Tutorial / Sequence
//
//  A sequence is ONE mechanic's full tutorial
//  (e.g. "Dash Tutorial", "Inventory Tutorial").
//  Point a TutorialTrigger at it to fire it in-game.
// ─────────────────────────────────────────────────────────────
[CreateAssetMenu(fileName = "NewTutorialSequence", menuName = "Scriptable Objects/Tutorial/Sequence")]
public class TutorialSequence : ScriptableObject
{
    [ReadOnly] public string id;
#if UNITY_EDITOR
    [ContextMenu("Generate New ID")]
    public void GenerateNewID()
    {
        id = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
    }
#endif
    [Tooltip("Human-readable label, used in logs and the editor.")]
    public string sequenceName = "My Tutorial";

    [Tooltip("Ordered list of steps for this tutorial.")]
    public TutorialStep[] steps;

    [Tooltip("If true, this sequence can only ever play once per game session.\n" +
             "Subsequent triggers are silently ignored.")]
    public bool playOnce = true;

    // Runtime flag – reset between sessions or via TutorialManager.ResetAll()
    [System.NonSerialized]
    public bool hasPlayed = false;
}