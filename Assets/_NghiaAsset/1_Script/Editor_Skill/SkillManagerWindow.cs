using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Turnbase
{
    public class SkillManagerWindow : EditorWindow
    {
        // --- Biến lưu trữ trạng thái ---
        private SkillList selectedSkillList = null;
        private Skill selectedSkill = null;
        private Editor skillListEditor = null;
        private Editor skillEditor = null;

        // Đã đổi tên scrollPosList thành scrollPosSkillLists
        private Vector2 scrollPosSkillLists; // Dành cho Danh sách Skill Lists (cột trái)
        private Vector2 scrollPosSkills;     // MỚI: Dành cho Danh sách Skills con (cột giữa)
        private Vector2 scrollPosDetails;     // Dành cho Chi tiết Skill (cột phải)

        // --- Biến cho chiều rộng cố định ---
        private const float ListColumnWidth = 250f;
        // ĐÃ TĂNG: Chiều rộng cố định cho cột Skill con để chứa Tên và Mô tả
        private const float SkillColumnWidth = 350f;

        // --- Khởi tạo Window ---
        [MenuItem("Tools/Skill Management Window")]
        public static void ShowWindow()
        {
            GetWindow<SkillManagerWindow>("Skill Manager");
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        // --- Hàm chính để vẽ giao diện ---
        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            // --- Cột Danh sách Skill Lists (Cột trái) ---
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(ListColumnWidth), GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("📁 Skill Lists", EditorStyles.boldLabel);
            if (GUILayout.Button("➕ Tạo Skill List mới"))
            {
                CreateNewSkillListAsset();
            }

            EditorGUILayout.Space(5);

            // Tìm tất cả các ScriptableObject thuộc loại SkillList trong Project
            string[] guids = AssetDatabase.FindAssets("t:SkillList");
            List<SkillList> allSkillLists = new List<SkillList>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SkillList list = AssetDatabase.LoadAssetAtPath<SkillList>(path);
                if (list != null)
                {
                    allSkillLists.Add(list);
                }
            }

            // SỬA LỖI: Sử dụng scrollPosSkillLists cho cột này
            scrollPosSkillLists = EditorGUILayout.BeginScrollView(scrollPosSkillLists, false, false);

            // Vẽ nút cho từng Skill List
            foreach (SkillList list in allSkillLists)
            {
                bool isSelected = selectedSkillList == list;

                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (isSelected)
                {
                    style.normal.background = MakeTex(2, 2, new Color(0.2f, 0.5f, 0.8f, 0.5f));
                }

                if (GUILayout.Button(list.listName, style))
                {
                    if (isSelected)
                    {
                        SetSelectedSkillList(null);
                    }
                    else
                    {
                        SetSelectedSkillList(list);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // ---------------------------------
            // Khu vực Chỉnh sửa Skill List đã chọn (Mở rộng hết phần còn lại)
            // ---------------------------------
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (selectedSkillList != null && skillListEditor != null)
            {
                EditorGUI.BeginChangeCheck();

                skillListEditor.DrawHeader();
                EditorGUILayout.Space(5);

                DrawSkillListAndDetails(skillListEditor.serializedObject);

                if (EditorGUI.EndChangeCheck())
                {
                    skillListEditor.serializedObject.ApplyModifiedProperties();
                    EditorUtility.SetDirty(selectedSkillList);
                    AssetDatabase.SaveAssets();
                    Repaint();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Chọn một Skill List bên trái hoặc tạo một List mới.", EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        // --- Hàm phụ: Thiết lập Skill List đang chọn ---
        private void SetSelectedSkillList(SkillList list)
        {
            selectedSkillList = list;
            selectedSkill = null;
            skillEditor = null;

            if (selectedSkillList != null)
            {
                Editor.CreateCachedEditor(selectedSkillList, null, ref skillListEditor);
            }
            else
            {
                skillListEditor = null;
            }
        }

        // --- Hàm phụ: Vẽ danh sách Skill và chi tiết Skill con ---
        private void DrawSkillListAndDetails(SerializedObject so)
        {
            SerializedProperty listNameProp = so.FindProperty("listName");
            SerializedProperty skillsProp = so.FindProperty("skills");

            so.Update();

            EditorGUILayout.PropertyField(listNameProp);
            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            // --- Cột Danh sách Skill (trong List đã chọn) (Cột giữa) ---
            // Đã thêm giới hạn chiều rộng SkillColumnWidth
            EditorGUILayout.BeginVertical("Box", GUILayout.ExpandHeight(true), GUILayout.Width(SkillColumnWidth));

            // SỬA LỖI: Sử dụng scrollPosSkills cho cột này
            scrollPosSkills = EditorGUILayout.BeginScrollView(scrollPosSkills, GUILayout.ExpandHeight(true));

            // --- Vẽ các Skill con đã có ---
            for (int i = 0; i < skillsProp.arraySize; i++)
            {
                SerializedProperty skillElement = skillsProp.GetArrayElementAtIndex(i);
                Skill currentSkill = skillElement.objectReferenceValue as Skill;

                if (currentSkill == null) continue;

                DrawSkillButton(currentSkill, skillElement);
            }

            EditorGUILayout.EndScrollView();

            // ===============================================
            // KHU VỰC DRAG AND DROP
            // ===============================================

            Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "Kéo và Thả Skill (.asset) vào đây");

            HandleDragAndDrop(dropArea, skillsProp);

            // ===============================================

            EditorGUILayout.EndVertical();

            // ---------------------------------
            // --- Cột Chi tiết Skill con (Cột phải) ---
            // ---------------------------------
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            scrollPosDetails = EditorGUILayout.BeginScrollView(scrollPosDetails, GUILayout.ExpandHeight(true));

            if (selectedSkill != null && skillEditor != null)
            {
                // Thay thế markdown bằng EditorStyles.boldLabel
                EditorGUILayout.LabelField($"Chi tiết Skill: {selectedSkill.skillName}", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);

                EditorGUI.BeginChangeCheck();
                skillEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedSkill);
                    AssetDatabase.SaveAssets();
                    Repaint();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Chọn một Skill từ danh sách bên trái để chỉnh sửa.", EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();

            so.ApplyModifiedProperties();
        }

        // --- Hàm phụ: Vẽ nút Skill và nút Xóa ---
        private void DrawSkillButton(Skill currentSkill, SerializedProperty skillElement)
        {
            bool isSelected = selectedSkill == currentSkill;
            // Chiều cao nút lớn hơn để chứa Icon + Tên + Mô tả
            const float buttonHeight = 60f;
            const float deleteButtonWidth = 35f; // Chiều rộng cho nút X

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            if (isSelected)
            {
                Color highlightColor = new Color(0.2f, 0.5f, 0.8f, 0.5f);
                buttonStyle.normal.background = MakeTex(2, 2, highlightColor);
                buttonStyle.active.background = MakeTex(2, 2, highlightColor * 1.5f);
            }

            // Bắt đầu vùng nút lớn (bao gồm Icon, Text và nút X)
            EditorGUILayout.BeginHorizontal(GUILayout.Height(buttonHeight));

            // 1. Vùng Button chính (Icon + Text), chiếm hết chiều rộng còn lại
            if (GUILayout.Button("", buttonStyle, GUILayout.ExpandWidth(true), GUILayout.Height(buttonHeight)))
            {
                if (isSelected)
                {
                    selectedSkill = null;
                    skillEditor = null;
                }
                else
                {
                    selectedSkill = currentSkill;
                    Editor.CreateCachedEditor(selectedSkill, null, ref skillEditor);
                }
            }
            // Lấy vị trí của nút vừa được vẽ
            Rect buttonRect = GUILayoutUtility.GetLastRect();

            // --- Bắt đầu Vẽ nội dung bên trong Button (Icon, Tên, Mô tả) ---

            // 1.1 Icon (Bên trái)
            const float iconSize = 48f;
            Texture2D iconTexture = AssetPreview.GetAssetPreview(currentSkill.icon) ?? AssetPreview.GetMiniTypeThumbnail(typeof(Skill));
            Rect iconRect = new Rect(buttonRect.x + 5, buttonRect.y + (buttonHeight - iconSize) / 2, iconSize, iconSize);
            GUI.DrawTexture(iconRect, iconTexture, ScaleMode.ScaleToFit);

            // 1.2 Text (Bên phải Icon)
            // Chiều rộng Text Area: (Chiều rộng nút - Icon - 5px space - 5px padding trái)
            float textWidth = buttonRect.width - iconSize - 10;
            Rect textRect = new Rect(buttonRect.x + 5 + iconSize + 5, buttonRect.y + 5, textWidth, buttonHeight - 10);

            // Tên Skill (In đậm)
            GUIStyle nameStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.UpperLeft,
                clipping = TextClipping.Clip,
            };
            GUI.Label(new Rect(textRect.x, textRect.y, textRect.width, 20), currentSkill.skillName, nameStyle);

            // Mô tả Skill (Chữ nhỏ, xám)
            GUIStyle descStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                normal = { textColor = Color.gray }
            };
            // Cắt mô tả để chỉ hiển thị một phần
            string displayDescription = currentSkill.description;
            if (displayDescription.Length > 50)
            {
                displayDescription = displayDescription.Substring(0, 50).Trim() + "...";
            }

            GUI.Label(new Rect(textRect.x, textRect.y + 20, textRect.width, 35), displayDescription, descStyle);

            // --- Kết thúc Vẽ nội dung ---

            // 2. Nút Delete (X) (Bên phải cùng)
            // Sử dụng GUILayout.Button để nó được xếp hàng ngang bên cạnh nút chính
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("X", GUILayout.Width(deleteButtonWidth), GUILayout.Height(buttonHeight)))
            {
                if (EditorUtility.DisplayDialog("Xác nhận Xóa Skill",
                                               $"Bạn có chắc chắn muốn xóa Skill '{currentSkill.skillName}' khỏi List '{selectedSkillList.listName}' không?",
                                               "Xóa", "Hủy bỏ"))
                {
                    if (selectedSkill == currentSkill)
                    {
                        selectedSkill = null;
                        skillEditor = null;
                    }

                    skillElement.objectReferenceValue = null;

                    if (IsSubAsset(currentSkill))
                    {
                        DestroyImmediate(currentSkill, true);
                        Debug.Log($"Đã xóa Sub-Asset Skill: {currentSkill.skillName}");
                    }

                    EditorUtility.SetDirty(selectedSkillList);
                    AssetDatabase.SaveAssets();
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.EndHorizontal();
        }

        // --- Hàm phụ: Kiểm tra xem Object có phải là Sub-Asset không ---
        private bool IsSubAsset(Object asset)
        {
            if (asset == null || selectedSkillList == null) return false;

            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(assetPath)) return false;

            // Lấy tất cả Sub-Assets của Asset cha (selectedSkillList)
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(selectedSkillList));

            // Kiểm tra xem asset có nằm trong danh sách Sub-Assets của cha không
            return subAssets.Contains(asset);
        }

        // --- Hàm phụ: Xử lý Kéo và Thả (Giữ nguyên) ---
        private void HandleDragAndDrop(Rect dropArea, SerializedProperty skillsProp)
        {
            if (dropArea.Contains(Event.current.mousePosition))
            {
                switch (Event.current.type)
                {
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        bool validDrag = true;
                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if (!(draggedObject is Skill))
                            {
                                validDrag = false;
                                break;
                            }
                        }

                        if (validDrag)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                            if (Event.current.type == EventType.DragPerform)
                            {
                                DragAndDrop.AcceptDrag();

                                bool listModified = false;
                                foreach (Object draggedObject in DragAndDrop.objectReferences)
                                {
                                    Skill skillToAdd = draggedObject as Skill;
                                    if (skillToAdd != null)
                                    {
                                        bool exists = false;
                                        for (int i = 0; i < skillsProp.arraySize; i++)
                                        {
                                            if (skillsProp.GetArrayElementAtIndex(i).objectReferenceValue == skillToAdd)
                                            {
                                                exists = true;
                                                break;
                                            }
                                        }

                                        if (!exists)
                                        {
                                            skillsProp.arraySize++;
                                            skillsProp.GetArrayElementAtIndex(skillsProp.arraySize - 1).objectReferenceValue = skillToAdd;
                                            listModified = true;
                                        }
                                    }
                                }

                                if (listModified)
                                {
                                    EditorUtility.SetDirty(selectedSkillList);
                                    AssetDatabase.SaveAssets();
                                    skillsProp.serializedObject.ApplyModifiedProperties();
                                    Repaint();
                                }

                                Event.current.Use();
                            }
                        }
                        break;
                }
            }
        }

        // --- Hàm phụ: Tạo Skill List mới (Giữ nguyên) ---
        private void CreateNewSkillListAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject("Lưu Skill List mới",
                                                             "New Skill List",
                                                             "asset",
                                                             "Vui lòng chọn tên file cho Skill List mới");

            if (!string.IsNullOrEmpty(path))
            {
                SkillList newSkillList = CreateInstance<SkillList>();
                AssetDatabase.CreateAsset(newSkillList, path);
                AssetDatabase.SaveAssets();
                SetSelectedSkillList(newSkillList);
            }
        }

        // --- Hàm tiện ích: Tạo Texture cho màu nền highlight (Giữ nguyên) ---
        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}