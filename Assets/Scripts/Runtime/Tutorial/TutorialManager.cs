using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Overlay Canvas  (sortOrder 50)")]
    public Canvas overlayCanvas;
    public Image overlayImage;
    [Range(0f, 1f)] public float overlayAlpha = 0.75f;
    public float fadeDuration = 0.35f;

    [Header("Highlight Canvas  (sortOrder 52)")]
    public Canvas highlightCanvas;

    [Header("Tutorial Box (inside Highlight Canvas)")]
    public GameObject tutorialBox;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI buttonToPressText;
    public GameObject boxAnimationImage;

    [Header("Box Pulse Animation")]
    public float pulseDuration = 0.8f;
    public float pulseTargetScale = 1.5f;

    [Header("Text Box Positioning")]
    public Camera referenceCamera;
    public Vector2 fallbackScreenPosition = new Vector2(0.5f, 0.15f);

    [Header("Global Continue Action  (New Input System)")]
    public InputActionReference globalContinueAction;

    public bool IsRunning => _isRunning;

    private TutorialSequence _currentSequence;
    private int _stepIndex;
    private bool _isRunning;
    private Coroutine _runCoroutine;
    private Coroutine _pulseCoroutine;
    private Transform _anchorTarget;
    private TutorialStepBinding[] _stepBindings;
    private TutorialStepBinding _currentBinding;   // ← ADD
    private bool _mouseClickedTarget;              // ← ADD
    // Snapshot of every action that was enabled before we blocked input.
    private readonly List<InputAction> _snapshotEnabledActions = new();

    private struct UIParentRecord
    {
        public GameObject obj;
        public Transform originalParent;
        public int originalSiblingIndex;
        public Vector3 originalLocalScale;
        public Vector3 originalLocalPosition;
        public Quaternion originalLocalRotation;
    }
    private readonly List<UIParentRecord> _movedUIObjects = new();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetOverlayAlpha(0f);
        overlayCanvas.gameObject.SetActive(false);
        tutorialBox.SetActive(false);

        if (boxAnimationImage != null)
            boxAnimationImage.SetActive(false);
    }

    private void OnEnable() => globalContinueAction?.action.Enable();
    private void OnDisable() => globalContinueAction?.action.Disable();

    public void PlaySequence(TutorialSequence sequence,
                             Transform anchorTarget = null,
                             TutorialStepBinding[] stepBindings = null)
    {
        if (sequence == null) { Debug.LogWarning("[TutorialManager] PlaySequence called with null sequence."); return; }
        if (sequence.playOnce && sequence.hasPlayed) return;
        if (_isRunning) StopSequenceImmediate();

        _currentSequence = sequence;
        _anchorTarget = anchorTarget;
        _stepBindings = stepBindings;
        _stepIndex = 0;
        _isRunning = true;

        _runCoroutine = StartCoroutine(RunSequence());
    }

    public void AdvanceStep() => _stepIndex++;

    public void StopSequenceImmediate()
    {
        if (_runCoroutine != null) StopCoroutine(_runCoroutine);
        StopPulseAnimation();
        CleanupHighlights();
        RestoreAllActions();
        SetOverlayAlpha(0f);
        overlayCanvas.gameObject.SetActive(false);
        tutorialBox.SetActive(false);
        Time.timeScale = 1f;
        _isRunning = false;
    }

    public void ResetAll(TutorialSequence[] sequences)
    {
        foreach (var s in sequences) s.hasPlayed = false;
    }

    // ── Input Blocking ─────────────────────────────────────────

    private void BlockAllActionsExceptButtonPressDetect(InputAction continueAction)
    {
        _snapshotEnabledActions.Clear();

        foreach (var action in InputSystem.ListEnabledActions())
        {
            if (action == continueAction) continue;

            // Keep UI EventSystem actions alive so mouse clicks still register
            if (action.name == "Point" ||
                action.name == "Click" ||
                action.name == "Navigate" ||
                action.name == "Submit" ||
                action.name == "Cancel" ||
                action.name == "ScrollWheel") continue;

            _snapshotEnabledActions.Add(action);
            action.Disable();
        }
    }

    private void BlockAllActionsExcept(InputAction continueAction)
    {
        _snapshotEnabledActions.Clear();

        var continueBindingPaths = new HashSet<string>();
        foreach (var binding in continueAction.bindings)
        {
            if (!string.IsNullOrEmpty(binding.effectivePath))
                continueBindingPaths.Add(binding.effectivePath);
        }

        foreach (var action in InputSystem.ListEnabledActions())
        {
            if (action == continueAction) continue;

            // Keep UI EventSystem actions alive so mouse clicks still register
            if (action.name == "Point" ||
                action.name == "Click" ||
                action.name == "Navigate" ||
                action.name == "Submit" ||
                action.name == "Cancel" ||
                action.name == "ScrollWheel") continue;

            bool sharesBinding = false;
            foreach (var binding in action.bindings)
            {
                if (continueBindingPaths.Contains(binding.effectivePath))
                {
                    sharesBinding = true;
                    break;
                }
            }

            if (sharesBinding) continue;

            _snapshotEnabledActions.Add(action);
            action.Disable();
        }
    }

    private void RestoreAllActions()
    {
        foreach (var action in _snapshotEnabledActions)
            action?.Enable();

        _snapshotEnabledActions.Clear();
    }

    // ── Core Coroutine ─────────────────────────────────────────

    private IEnumerator RunSequence()
    {
        _currentSequence.hasPlayed = true;
        Time.timeScale = 0f;

        overlayCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(FadeOverlay(0f, overlayAlpha, fadeDuration));

        while (_stepIndex < _currentSequence.steps.Length)
        {
            TutorialStep step = _currentSequence.steps[_stepIndex];
            int entryIndex = _stepIndex;

            ShowStep(step, entryIndex);

            InputAction continueAction = step.GetOverrideAction() ?? globalContinueAction?.action;
            continueAction?.Enable();

            bool needsInput = step.waitForAnyKey || step.waitForKeyPress || step.waitForMouseClick;

            if (needsInput)
            {
                // ── Set up click listener ──────────────────────────
                if (step.waitForMouseClick)
                {
                    _mouseClickedTarget = false;
                    AddClickListeners(_currentBinding?.highlightTargets);
                }

                // ── Block input appropriately ──────────────────────
                if (step.waitForAnyKey)
                {
                    BlockAllActionsExceptButtonPressDetect(continueAction);
                }
                else if (step.waitForKeyPress)
                {
                    if (step.blockTheSameInputAction)
                        BlockAllActionsExceptButtonPressDetect(continueAction);
                    else
                        BlockAllActionsExcept(continueAction);
                }
                // waitForMouseClick only → no key blocking needed

                // ── Build unified wait condition ───────────────────
                bool anyKeyPressed = false;
                IDisposable subscription = null;

                if (step.waitForAnyKey)
                {
                    subscription = InputSystem.onAnyButtonPress.CallOnce(_ =>
                    {
                        anyKeyPressed = true;
                        subscription?.Dispose();
                        subscription = null;
                    });
                }

                yield return new WaitUntil(() =>
                {
                    if (_stepIndex != entryIndex) return true;
                    if (step.waitForMouseClick && _mouseClickedTarget) return true;
                    if (step.waitForAnyKey && anyKeyPressed) return true;
                    if (step.waitForKeyPress && continueAction != null && continueAction.WasPressedThisFrame()) return true;
                    return false;
                });

                subscription?.Dispose();
                subscription = null;

                // ── Cleanup ────────────────────────────────────────
                if (step.waitForMouseClick)
                    RemoveClickListeners(_currentBinding?.highlightTargets);

                RestoreAllActions();
            }
            else
            {
                // Auto-advance
                float elapsed = 0f;
                while (elapsed < step.autoAdvanceDelay && _stepIndex == entryIndex)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }
            }

            if (_stepIndex == entryIndex) _stepIndex++;

            StopPulseAnimation();

            if (step.waitForKeyPress || step.waitForAnyKey)
                RestoreAllActions();

            yield return new WaitForSecondsRealtime(0.1f);
        }

        HideStep();
        yield return StartCoroutine(FadeOverlay(overlayAlpha, 0f, fadeDuration));

        CleanupHighlights();
        overlayCanvas.gameObject.SetActive(false);

        Time.timeScale = 1f;
        _isRunning = false;
    }
    // ── Mouse Click Helpers ────────────────────────────────────

    private void AddClickListeners(GameObject[] targets)
    {
        if (targets == null) return;

        foreach (var obj in targets)
        {
            if (obj == null) continue;

            // Collect all raycastable Graphics on self AND children
            var graphics = obj.GetComponentsInChildren<Graphic>(includeInactive: false);

            if (graphics.Length == 0)
            {
                Debug.LogWarning($"[TutorialManager] {obj.name} has no Graphic in itself or children - clicks won't register!");
                continue;
            }

            bool addedAtLeastOne = false;

            foreach (var graphic in graphics)
            {
                if (!graphic.raycastTarget) continue;

                var trigger = graphic.gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>()
                           ?? graphic.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();

                var entry = new UnityEngine.EventSystems.EventTrigger.Entry
                {
                    eventID = UnityEngine.EventSystems.EventTriggerType.PointerClick
                };
                entry.callback.AddListener(_ => _mouseClickedTarget = true);
                trigger.triggers.Add(entry);

                addedAtLeastOne = true;
            }

            if (!addedAtLeastOne)
                Debug.LogWarning($"[TutorialManager] {obj.name} has Graphics but all have raycastTarget=false!");
        }
    }

    private void RemoveClickListeners(GameObject[] targets)
    {
        if (targets == null) return;

        foreach (var obj in targets)
        {
            if (obj == null) continue;

            // Clean up from self AND all children
            var triggers = obj.GetComponentsInChildren<UnityEngine.EventSystems.EventTrigger>(includeInactive: false);

            foreach (var trigger in triggers)
            {
                trigger.triggers.RemoveAll(e =>
                    e.eventID == UnityEngine.EventSystems.EventTriggerType.PointerClick);

                if (trigger.triggers.Count == 0)
                    Destroy(trigger);
            }
        }
    }
    private void ShowStep(TutorialStep step, int stepIndex)
    {
        if (buttonToPressText) buttonToPressText.text = step.buttonToPressText;
        if (descriptionText) descriptionText.text = step.description;

        tutorialBox.SetActive(true);
        buttonToPressText.gameObject.SetActive(step.buttonToPressText != "");

        _currentBinding = (_stepBindings != null && stepIndex < _stepBindings.Length)  // ← STORE IT
            ? _stepBindings[stepIndex] : null;

        Transform anchor = (_currentBinding?.anchorOverride != null) ? _currentBinding.anchorOverride : _anchorTarget;

        CleanupHighlights();

        // DEBUG: kiểm tra trạng thái binding
        Debug.Log($"[Tutorial] ShowStep {stepIndex} — binding={(object)_currentBinding ?? "NULL"} | " +
                  $"highlightTargets={(_currentBinding?.highlightTargets?.Length.ToString() ?? "null")} | " +
                  $"targetIdKeys={(_currentBinding?.targetIdKeys?.Length.ToString() ?? "null")}");

        // Ưu tiên highlightTargets kéo thả; nếu không có thì tìm theo targetIdKeys
        if (_currentBinding?.highlightTargets != null && _currentBinding.highlightTargets.Length > 0)
        {
            foreach (var t in _currentBinding.highlightTargets)
            LiftUIObjects(_currentBinding.highlightTargets);
        }
        else if (_currentBinding?.targetIdKeys != null && _currentBinding.targetIdKeys.Length > 0)
        {
            var found = new List<GameObject>();
            var identities = FindObjectsOfType<TutorialIdentity>(true);

            Debug.Log($"[Tutorial] targetIdKeys lookup — keys: [{string.Join(", ", _currentBinding.targetIdKeys)}] | TutorialIdentity found in scene: {identities.Length}");

            foreach (string key in _currentBinding.targetIdKeys)
            {
                bool matched = false;
                foreach (var id in identities)
                {
                    if (id.tutorialId == key)
                    {
                        found.Add(id.gameObject);
                        Debug.Log($"[Tutorial] ✅ Matched key '{key}' → {id.gameObject.name} (active={id.gameObject.activeInHierarchy})");
                        matched = true;
                        break;
                    }
                }
                if (!matched)
                    Debug.LogWarning($"[Tutorial] ❌ No TutorialIdentity found for key '{key}'");
            }

            if (found.Count > 0) LiftUIObjects(found.ToArray());
            else Debug.LogWarning("[Tutorial] targetIdKeys: không tìm thấy object nào để highlight!");
        }

        PositionTutorialBox(step, anchor);
        StartPulseAnimation();
    }

    private void HideStep()
    {
        StopPulseAnimation();
        tutorialBox.SetActive(false);
        CleanupHighlights();
    }

    // ── Box Pulse Animation ────────────────────────────────────

    private void StartPulseAnimation()
    {
        StopPulseAnimation();
        if (boxAnimationImage == null) return;

        RectTransform pulseRect = boxAnimationImage.GetComponent<RectTransform>();
        if (pulseRect != null) pulseRect.localScale = Vector3.one;

        SetGraphicAlpha(boxAnimationImage, 1f);
        boxAnimationImage.SetActive(true);
        _pulseCoroutine = StartCoroutine(PulseLoop());
    }

    private void StopPulseAnimation()
    {
        if (_pulseCoroutine != null)
        {
            StopCoroutine(_pulseCoroutine);
            _pulseCoroutine = null;
        }
        if (boxAnimationImage != null)
            boxAnimationImage.SetActive(false);
    }

    private IEnumerator PulseLoop()
    {
        RectTransform pulseRect = boxAnimationImage.GetComponent<RectTransform>();

        while (true)
        {
            if (pulseRect != null) pulseRect.localScale = Vector3.one;
            SetGraphicAlpha(boxAnimationImage, 1f);

            float elapsed = 0f;
            while (elapsed < pulseDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / pulseDuration);
                float s = Mathf.Lerp(1f, pulseTargetScale, t);
                if (pulseRect != null) pulseRect.localScale = new Vector3(s, s, 1f);
                SetGraphicAlpha(boxAnimationImage, Mathf.Lerp(1f, 0f, t));
                yield return null;
            }

            if (pulseRect != null) pulseRect.localScale = new Vector3(pulseTargetScale, pulseTargetScale, 1f);
            SetGraphicAlpha(boxAnimationImage, 0f);
        }
    }

    private static void SetGraphicAlpha(GameObject go, float alpha)
    {
        if (go == null) return;
        foreach (var graphic in go.GetComponentsInChildren<Graphic>(includeInactive: true))
        {
            Color c = graphic.color;
            c.a = alpha;
            graphic.color = c;
        }
    }

    // ── Text Box Positioning ───────────────────────────────────

    private void PositionTutorialBox(TutorialStep step, Transform anchor)
    {
        RectTransform boxRect = tutorialBox.GetComponent<RectTransform>();
        if (boxRect == null) return;

        Vector2 screenPoint;

        if (anchor != null)
        {
            var anchorRect = anchor.GetComponent<RectTransform>();
            if (anchorRect != null)
            {
                Vector3[] corners = new Vector3[4];
                anchorRect.GetWorldCorners(corners);
                Vector3 worldCenter = (corners[0] + corners[2]) * 0.5f;
                screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldCenter);
            }
            else
            {
                Camera cam = referenceCamera != null ? referenceCamera : Camera.main;
                screenPoint = cam.WorldToScreenPoint(anchor.position);
            }
        }
        else
        {
            screenPoint = new Vector2(
                fallbackScreenPosition.x * Screen.width,
                fallbackScreenPosition.y * Screen.height);
        }

        screenPoint += step.positionOffset;

        RectTransform canvasRect = highlightCanvas.GetComponent<RectTransform>();
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPoint, highlightCanvas.worldCamera, out Vector2 localPoint))
        {
            boxRect.localPosition = localPoint;
        }
    }

    // ── UI Highlight Helpers ───────────────────────────────────

    private void LiftUIObjects(GameObject[] targets)
    {
        if (targets == null) return;

        Camera cam = referenceCamera != null ? referenceCamera : Camera.main;
        RectTransform canvasRect = highlightCanvas.GetComponent<RectTransform>();

        foreach (var obj in targets)
        {
            if (obj == null) continue;

            // Check if this object lives inside a World Space Canvas
            Canvas parentCanvas = obj.GetComponentInParent<Canvas>();
            bool isInWorldSpaceCanvas = parentCanvas != null
                                     && parentCanvas.renderMode == RenderMode.WorldSpace;

            // Snapshot full transform state before any changes
            _movedUIObjects.Add(new UIParentRecord
            {
                obj = obj,
                originalParent = obj.transform.parent,
                originalSiblingIndex = obj.transform.GetSiblingIndex(),
                originalLocalScale = obj.transform.localScale,
                originalLocalPosition = obj.transform.localPosition,
                originalLocalRotation = obj.transform.localRotation
            });

            if (isInWorldSpaceCanvas)
            {
                // ── World Space Canvas element ──────────────────────────────
                // 1. Capture world position BEFORE reparenting
                Vector3 worldPos = obj.transform.position;

                // 2. Convert world position → screen point via reference camera
                Vector2 screenPoint = cam.WorldToScreenPoint(worldPos);

                // 3. Reparent into the highlight canvas (no worldPositionStays so
                //    scale/rotation don't inherit the world-space canvas transform)
                obj.transform.SetParent(highlightCanvas.transform, worldPositionStays: false);
                obj.transform.localScale = Vector3.one * 70;
                obj.transform.localRotation = Quaternion.identity;
                // 4. Convert screen point → local position inside the highlight canvas
                //    and apply it so the element sits exactly where it did on screen
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvasRect, screenPoint, highlightCanvas.worldCamera, out Vector2 localPoint))
                {
                    obj.transform.localPosition = localPoint;
                }
            }
            else
            {
                // ── Regular Screen Space UI element ─────────────────────────
                obj.transform.SetParent(highlightCanvas.transform, worldPositionStays: true);
                //obj.transform.localScale = obj.transform.localScale;
                //obj.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void CleanupHighlights()
    {
        foreach (var record in _movedUIObjects)
        {
            if (record.obj == null) continue;

            record.obj.transform.SetParent(record.originalParent, worldPositionStays: false);
            record.obj.transform.SetSiblingIndex(record.originalSiblingIndex);
            record.obj.transform.localScale = record.originalLocalScale;
            record.obj.transform.localPosition = record.originalLocalPosition;
            record.obj.transform.localRotation = record.originalLocalRotation;
        }
        _movedUIObjects.Clear();
    }

    // ── Fade Helper ────────────────────────────────────────────

    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            SetOverlayAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }
        SetOverlayAlpha(to);
    }

    private void SetOverlayAlpha(float alpha)
    {
        if (overlayImage == null) return;
        Color c = overlayImage.color;
        c.a = alpha;
        overlayImage.color = c;
    }
}