// File: Assets/MiniOdin/Editor/LokiEditorBase.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public abstract class LokiEditorBase : Editor
{
    private struct GroupKey : IEquatable<GroupKey>
    {
        public string Foldout;
        public string Tab;

        public GroupKey(string foldout, string tab)
        {
            Foldout = foldout ?? "";
            Tab = tab ?? "";
        }

        public bool Equals(GroupKey other) => Foldout == other.Foldout && Tab == other.Tab;
        public override int GetHashCode() => Foldout.GetHashCode() ^ Tab.GetHashCode();
    }

    private Dictionary<string, bool> _foldouts = new();
    private int _selectedTab = 0;
    private Dictionary<string, ReorderableList> _reorderables = new();

    public override void OnInspectorGUI()
    {
        // --- Edit Script Button (works for MonoBehaviour + ScriptableObject) ---
        MonoScript script = null;

        if (target is MonoBehaviour mb)
            script = MonoScript.FromMonoBehaviour(mb);
        else if (target is ScriptableObject so)
            script = MonoScript.FromScriptableObject(so);

        if (script != null)
        {
            if (GUILayout.Button("Edit Script", GUILayout.Height(22)))
            {
                UnityEditor.AssetDatabase.OpenAsset(script);
            }
        }

        EditorGUILayout.Space();

        serializedObject.Update();


        var type = target.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        var normalProps = new List<SerializedProperty>();
        var foldGroups = new Dictionary<string, List<SerializedProperty>>();
        var tabGroups = new Dictionary<string, Dictionary<string, List<SerializedProperty>>>();
        var foldoutTabTracker = new Dictionary<string, string>();
        var foldoutErrors = new Dictionary<string, string>();

        foreach (var field in fields)
        {
            if (field.IsDefined(typeof(HideInInspector))) continue;
            var prop = serializedObject.FindProperty(field.Name);
            if (prop == null || !ShouldShowField(field)) continue;

            var tabAttr = field.GetCustomAttribute<TabGroupAttribute>();
            var foldAttr = field.GetCustomAttribute<FoldoutGroupAttribute>();

            string tab = tabAttr?.GroupName;
            string foldout = foldAttr?.GroupName ?? "";

            if (!string.IsNullOrEmpty(foldout))
            {
                if (foldoutTabTracker.TryGetValue(foldout, out var existingTab))
                {
                    if (existingTab != tab)
                    {
                        string fieldKey = prop.name;
                        foldoutErrors[fieldKey] = $"FoldoutGroup \"{foldout}\" is used in multiple TabGroups (\"{existingTab}\" vs \"{tab}\"). This is not allowed.";
                        continue;
                    }
                }
                else
                {
                    foldoutTabTracker[foldout] = tab;
                }
            }

            if (tab != null)
            {
                if (!tabGroups.ContainsKey(tab))
                    tabGroups[tab] = new Dictionary<string, List<SerializedProperty>>();

                if (!tabGroups[tab].ContainsKey(foldout))
                    tabGroups[tab][foldout] = new List<SerializedProperty>();

                tabGroups[tab][foldout].Add(prop);
            }
            else if (!string.IsNullOrEmpty(foldout))
            {
                if (!foldGroups.ContainsKey(foldout))
                    foldGroups[foldout] = new List<SerializedProperty>();
                foldGroups[foldout].Add(prop);
            }
            else
            {
                normalProps.Add(prop);
            }
        }

        foreach (var kvp in foldoutErrors)
            EditorGUILayout.HelpBox(kvp.Value, MessageType.Error);

        foreach (var p in normalProps)
            DrawProperty(p);

        foreach (var kvp in foldGroups)
        {
            if (!_foldouts.ContainsKey(kvp.Key))
                _foldouts[kvp.Key] = true;

            _foldouts[kvp.Key] = EditorGUILayout.Foldout(_foldouts[kvp.Key], kvp.Key, true);
            if (_foldouts[kvp.Key])
            {
                EditorGUI.indentLevel++;
                foreach (var p in kvp.Value)
                    DrawProperty(p);
                EditorGUI.indentLevel--;
            }
        }

        if (tabGroups.Count > 0)
        {
            var groupNames = tabGroups.Keys.ToArray();
            _selectedTab = GUILayout.Toolbar(_selectedTab, groupNames);
            _selectedTab = Mathf.Clamp(_selectedTab, 0, groupNames.Length - 1);

            var activeGroup = groupNames[_selectedTab];
            var foldoutMap = tabGroups[activeGroup];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (var foldout in foldoutMap.Keys)
            {
                if (string.IsNullOrEmpty(foldout))
                {
                    foreach (var p in foldoutMap[foldout])
                        DrawProperty(p);
                }
                else
                {
                    if (!_foldouts.ContainsKey(foldout))
                        _foldouts[foldout] = true;

                    EditorGUILayout.BeginHorizontal(LokiSpecialGUIStyle.GetFoldoutStyle());
                    _foldouts[foldout] = EditorGUILayout.Foldout(_foldouts[foldout], foldout, true);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    if (_foldouts[foldout])
                    {
                        EditorGUI.indentLevel++;
                        foreach (var p in foldoutMap[foldout])
                            DrawProperty(p);
                        EditorGUI.indentLevel--;
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndVertical();
        }

        DrawButtons();
        serializedObject.ApplyModifiedProperties();
    }

    private bool ShouldShowField(FieldInfo field)
    {
        var sif = field.GetCustomAttribute<ShowIfAttribute>();
        if (sif != null)
        {
            var cond = field.DeclaringType.GetField(sif.Condition, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (cond == null || cond.FieldType != typeof(bool)) return false;
            bool val = (bool)cond.GetValue(target);
            if (sif.Invert) val = !val;
            if (!val) return false;
        }

        var sien = field.GetCustomAttribute<ShowIfEnumValueAttribute>();
        if (sien != null)
        {
            var ef = field.DeclaringType.GetField(sien.EnumFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (ef == null || !ef.FieldType.IsEnum) return false;
            var cur = ef.GetValue(target);
            if (!sien.TargetValues.Contains(cur)) return false;
        }

        return true;
    }


    private void DrawProperty(SerializedProperty prop)
    {
        var field = target.GetType().GetField(prop.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (field == null) return;

        bool prevEnabled = GUI.enabled;
        GUI.enabled = field.GetCustomAttribute<ReadOnlyAttribute>() == null;

        var labelAttr = field.GetCustomAttribute<LabelTextAttribute>();
        GUIContent labelContent = new(labelAttr?.Label ?? ObjectNames.NicifyVariableName(prop.name));

        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        Color originalColor = GUI.color;
        bool hasCustomColor = false;
        if (field.GetCustomAttribute<RequiredAttribute>() != null)
        {
            bool isAssigned = prop.propertyType switch
            {
                SerializedPropertyType.ObjectReference => prop.objectReferenceValue != null,
                _ => prop.propertyType != SerializedPropertyType.Generic // fallback
            };

            if (!isAssigned)
            {
                var errorStyle = new GUIStyle(EditorStyles.label);
                errorStyle.normal.textColor = Color.red;
                labelContent.text = labelContent.text + " *Required*";
            }
        }

        if (labelAttr != null)
        {
            if (!string.IsNullOrEmpty(labelAttr.ColorName) && TryParseColor(labelAttr.ColorName, out var labelColor))
            {
                labelStyle.normal.textColor = labelColor;
                GUI.color = labelColor;
                hasCustomColor = true;
            }

            if (labelAttr.FontSize > 0)
                labelStyle.fontSize = labelAttr.FontSize;

            labelStyle.fontStyle = labelAttr.FontStyle;
        }

        if (field.GetCustomAttribute<MinMaxSliderAttribute>() is MinMaxSliderAttribute mms
            && prop.propertyType == SerializedPropertyType.Vector2)
        {
            Vector2 v = prop.vector2Value;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(labelContent);
            v.x = EditorGUILayout.FloatField(v.x, GUILayout.Width(50));
            EditorGUILayout.MinMaxSlider(ref v.x, ref v.y, mms.Min, mms.Max);
            v.y = EditorGUILayout.FloatField(v.y, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            if (mms.EnforceMinMaxDistance > 0f && (v.y - v.x) < mms.EnforceMinMaxDistance)
                v.y = v.x + mms.EnforceMinMaxDistance;

            v.x = Mathf.Clamp(v.x, mms.Min, mms.Max);
            v.y = Mathf.Clamp(v.y, mms.Min, mms.Max);
            prop.vector2Value = v;
        }
        else
        {
            EditorGUILayout.PropertyField(prop, labelContent, true);
        }

        if (hasCustomColor)
            GUI.color = originalColor;

        GUI.enabled = prevEnabled;
    }

    private void DrawButtons()
    {
        var methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (var m in methods)
        {
            if (m.GetCustomAttribute<ButtonAttribute>() != null && m.GetParameters().Length == 0)
            {
                if (GUILayout.Button(ObjectNames.NicifyVariableName(m.Name)))
                    m.Invoke(target, null);
            }
        }
    }

    protected bool TryParseColor(string name, out Color color)
    {
        switch (name.ToLower())
        {
            case "red": color = Color.red; return true;
            case "green": color = Color.green; return true;
            case "blue": color = Color.blue; return true;
            case "yellow": color = Color.yellow; return true;
            case "cyan": color = Color.cyan; return true;
            case "magenta": color = Color.magenta; return true;
            case "white": color = Color.white; return true;
            case "black": color = Color.black; return true;
            case "gray":
            case "grey": color = Color.gray; return true;
            default:
                color = default;
                return false;
        }
    }
}
