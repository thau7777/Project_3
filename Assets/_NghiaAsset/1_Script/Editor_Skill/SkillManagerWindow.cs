using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Turnbase
{
#if UNITY_EDITOR
    public class SkillManagerWindow : EditorWindow
    {
        // --- Biến lưu trữ trạng thái ---
        private SkillList selectedSkillList = null;
        private Skill selectedSkill = null;
        private Editor skillListEditor = null;
        private Editor skillEditor = null;

        private Vector2 scrollPosSkillLists;
        private Vector2 scrollPosSkills;
        private Vector2 scrollPosDetails;

        // --- Cấu hình giao diện ---
        private const float ListColumnWidth = 250f;
        private const float SkillColumnWidth = 350f;

        [MenuItem("Tools/Skill Management Window")]
        public static void ShowWindow()
        {
            GetWindow<SkillManagerWindow>("Skill Manager");
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            // --- CỘT 1: DANH SÁCH SKILL LISTS (BÊN TRÁI) ---
            DrawSkillListColumn();

            // --- KHU VỰC CHÍNH (CỘT 2 & 3) ---
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            if (selectedSkillList != null)
            {
                DrawMainContent();
            }
            else
            {
                EditorGUILayout.LabelField("Chọn một Skill List bên trái hoặc tạo một List mới.", EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSkillListColumn()
        {
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(ListColumnWidth), GUILayout.ExpandHeight(true));

            EditorGUILayout.LabelField("📁 Skill Lists", EditorStyles.boldLabel);
            if (GUILayout.Button("➕ Tạo Skill List mới"))
            {
                CreateNewSkillListAsset();
            }

            EditorGUILayout.Space(5);

            string[] guids = AssetDatabase.FindAssets("t:SkillList");
            scrollPosSkillLists = EditorGUILayout.BeginScrollView(scrollPosSkillLists);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                SkillList list = AssetDatabase.LoadAssetAtPath<SkillList>(path);
                if (list == null) continue;

                bool isSelected = selectedSkillList == list;
                GUIStyle style = new GUIStyle(GUI.skin.button);
                if (isSelected)
                {
                    style.normal.background = MakeTex(2, 2, new Color(0.2f, 0.5f, 0.8f, 0.5f));
                }

                if (GUILayout.Button(list.listName, style))
                {
                    SetSelectedSkillList(isSelected ? null : list);
                }
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void DrawMainContent()
        {
            // Hiển thị thông tin Folder
            EditorGUILayout.LabelField($"Cấu hình Folder cho: {selectedSkillList.listName}", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("Đường dẫn:", selectedSkillList.folderPath, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("📁 Chọn Folder", GUILayout.Width(100)))
            {
                string path = EditorUtility.OpenFolderPanel("Chọn Folder chứa Skill", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    if (path.StartsWith(Application.dataPath))
                    {
                        selectedSkillList.folderPath = "Assets" + path.Substring(Application.dataPath.Length);
                        RefreshSkillsFromFolder();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn một thư mục bên trong Project (Assets folder).", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("🔄 Làm mới danh sách từ Folder", GUILayout.Height(25)))
            {
                RefreshSkillsFromFolder();
            }

            EditorGUILayout.Space(10);

            // Chia đôi khu vực còn lại cho Cột giữa và Cột phải
            EditorGUILayout.BeginHorizontal();

            // --- CỘT 2: DANH SÁCH SKILLS (GIỮA) ---
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(SkillColumnWidth), GUILayout.ExpandHeight(true));
            EditorGUILayout.LabelField($"Skills tìm thấy ({selectedSkillList.skills.Count})", EditorStyles.miniBoldLabel);

            scrollPosSkills = EditorGUILayout.BeginScrollView(scrollPosSkills);
            foreach (Skill skill in selectedSkillList.skills)
            {
                if (skill == null) continue;
                DrawSkillListItem(skill);
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            // --- CỘT 3: CHI TIẾT SKILL (PHẢI) ---
            EditorGUILayout.BeginVertical("HelpBox", GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            scrollPosDetails = EditorGUILayout.BeginScrollView(scrollPosDetails);

            if (selectedSkill != null && skillEditor != null)
            {
                EditorGUILayout.LabelField($"Chi tiết: {selectedSkill.skillName}", EditorStyles.boldLabel);
                EditorGUILayout.Space(5);

                EditorGUI.BeginChangeCheck();
                skillEditor.OnInspectorGUI();
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(selectedSkill);

                    AssetDatabase.SaveAssets();
                }
            }
            else
            {
                EditorGUILayout.LabelField("Chọn một Skill để chỉnh sửa.", EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSkillListItem(Skill currentSkill)
        {
            bool isSelected = selectedSkill == currentSkill;
            const float buttonHeight = 50f;

            GUIStyle style = new GUIStyle(GUI.skin.button);
            if (isSelected) style.normal.background = MakeTex(2, 2, new Color(0.2f, 0.5f, 0.8f, 0.5f));

            if (GUILayout.Button("", style, GUILayout.Height(buttonHeight), GUILayout.ExpandWidth(true)))
            {
                selectedSkill = currentSkill;
                Editor.CreateCachedEditor(selectedSkill, null, ref skillEditor);
            }

            // Vẽ nội dung đè lên Button
            Rect rect = GUILayoutUtility.GetLastRect();
            Texture2D icon = AssetPreview.GetAssetPreview(currentSkill.icon) ?? AssetPreview.GetMiniTypeThumbnail(typeof(Skill));
            GUI.DrawTexture(new Rect(rect.x + 5, rect.y + 5, 40, 40), icon, ScaleMode.ScaleToFit);

            GUI.Label(new Rect(rect.x + 50, rect.y + 5, rect.width - 60, 20), currentSkill.skillName, EditorStyles.boldLabel);
            GUI.Label(new Rect(rect.x + 50, rect.y + 25, rect.width - 60, 20), "Asset: " + currentSkill.name, EditorStyles.miniLabel);
        }

        private void SetSelectedSkillList(SkillList list)
        {
            selectedSkillList = list;
            selectedSkill = null;
            skillEditor = null;

            if (selectedSkillList != null)
            {
                Editor.CreateCachedEditor(selectedSkillList, null, ref skillListEditor);
                RefreshSkillsFromFolder();
            }
        }

        private void RefreshSkillsFromFolder()
        {
            if (selectedSkillList == null || string.IsNullOrEmpty(selectedSkillList.folderPath)) return;

            if (!Directory.Exists(selectedSkillList.folderPath))
            {
                Debug.LogWarning("Thư mục không tồn tại: " + selectedSkillList.folderPath);
                return;
            }

            // Tìm tất cả asset là Skill trong folder đó
            string[] guids = AssetDatabase.FindAssets("t:Skill", new[] { selectedSkillList.folderPath });

            selectedSkillList.skills.Clear();
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Skill skill = AssetDatabase.LoadAssetAtPath<Skill>(assetPath);
                if (skill != null)
                {
                    selectedSkillList.skills.Add(skill);
                }
            }

            EditorUtility.SetDirty(selectedSkillList);

            //AssetDatabase.SaveAssets();
        }

        private void CreateNewSkillListAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject("Lưu Skill List mới", "New Skill List", "asset", "Chọn tên file");
            if (!string.IsNullOrEmpty(path))
            {
                SkillList newSkillList = CreateInstance<SkillList>();
                AssetDatabase.CreateAsset(newSkillList, path);
                AssetDatabase.SaveAssets();
                SetSelectedSkillList(newSkillList);
            }
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i) pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
#endif
}