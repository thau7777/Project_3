#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using System.IO;

public class SceneSwitcherWindow : EditorWindow
{
    private static string[] sceneNames = new string[0];
    private static string[] scenePaths = new string[0];
    private int selectedIndex = 0;
    private Vector2 scrollPosition;
    private string searchFilter = "";

    private static bool fetchAllScenes
    {
        get => EditorPrefs.GetBool("SceneSwitcher_FetchAllScenes", false);
        set => EditorPrefs.SetBool("SceneSwitcher_FetchAllScenes", value);
    }

    [MenuItem("Window/Scene Switcher %&s")] // Ctrl+Alt+S shortcut
    public static void ShowWindow()
    {
        var window = GetWindow<SceneSwitcherWindow>("Scene Switcher");
        window.minSize = new Vector2(300, 200);
        window.Show();
    }

    private void OnEnable()
    {
        RefreshSceneList();
        SelectCurrentScene();

        EditorBuildSettings.sceneListChanged += RefreshSceneList;
        EditorApplication.projectChanged += RefreshSceneList;
        EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
    }

    private void OnDisable()
    {
        EditorBuildSettings.sceneListChanged -= RefreshSceneList;
        EditorApplication.projectChanged -= RefreshSceneList;
        EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
    }

    private void OnSceneChanged(UnityEngine.SceneManagement.Scene prev, UnityEngine.SceneManagement.Scene current)
    {
        SelectCurrentScene();
        Repaint();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.ExpandHeight(true));

        // Header
        DrawHeader();

        GUILayout.Space(5);

        // Search bar
        DrawSearchBar();

        GUILayout.Space(5);

        // Scene list
        DrawSceneList();

        EditorGUILayout.EndVertical();

        // Footer with refresh button
        DrawFooter();
    }

    private void DrawHeader()
    {
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Scene Switcher", EditorStyles.boldLabel);

        GUILayout.FlexibleSpace();

        // Toggle for all scenes vs build scenes
        bool newFetchAllScenes = GUILayout.Toggle(fetchAllScenes, fetchAllScenes ? "All Scenes" : "Build Scenes", "Button");
        if (newFetchAllScenes != fetchAllScenes)
        {
            fetchAllScenes = newFetchAllScenes;
            RefreshSceneList();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DrawSearchBar()
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(50));
        searchFilter = EditorGUILayout.TextField(searchFilter);
        if (GUILayout.Button("×", GUILayout.Width(20)))
        {
            searchFilter = "";
            GUI.FocusControl(null);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawSceneList()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        string currentSceneName = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);

        // Filter scenes based on search
        var filteredScenes = sceneNames
            .Select((name, index) => new { name, index })
            .Where(x => string.IsNullOrEmpty(searchFilter) ||
                       x.name.ToLower().Contains(searchFilter.ToLower()))
            .ToArray();

        if (filteredScenes.Length == 0)
        {
            EditorGUILayout.HelpBox("No scenes found matching your search.", MessageType.Info);
        }
        else
        {
            foreach (var scene in filteredScenes)
            {
                bool isCurrentScene = scene.name == currentSceneName;

                EditorGUILayout.BeginHorizontal();

                // Highlight current scene
                if (isCurrentScene)
                {
                    GUI.backgroundColor = new Color(0.3f, 0.7f, 1f);
                }

                // Scene button
                string buttonLabel = isCurrentScene ? $"● {scene.name}" : scene.name;
                if (GUILayout.Button(buttonLabel, GUILayout.Height(25)))
                {
                    if (!isCurrentScene)
                    {
                        LoadScene(scene.index);
                    }
                }

                GUI.backgroundColor = Color.white;

                // Add to build button (only show if in "All Scenes" mode)
                if (fetchAllScenes)
                {
                    bool inBuildSettings = EditorBuildSettings.scenes
                        .Any(s => Path.GetFileNameWithoutExtension(s.path) == scene.name);

                    if (!inBuildSettings)
                    {
                        if (GUILayout.Button("+", GUILayout.Width(25), GUILayout.Height(25)))
                        {
                            AddSceneToBuildSettings(scenePaths[scene.index]);
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Space(2);
            }
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawFooter()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        GUILayout.Label($"{sceneNames.Length} scene(s)", EditorStyles.miniLabel);

        GUILayout.FlexibleSpace();

        if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(60)))
        {
            RefreshSceneList();
        }

        if (GUILayout.Button("Build Settings", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }

        EditorGUILayout.EndHorizontal();
    }

    private void RefreshSceneList()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        if (fetchAllScenes)
        {
            var allScenes = Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories);
            scenePaths = allScenes;
            sceneNames = allScenes
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .ToArray();
        }
        else
        {
            var validScenes = EditorBuildSettings.scenes
                .Where(scene => scene.enabled && File.Exists(scene.path))
                .ToArray();

            scenePaths = validScenes.Select(s => s.path).ToArray();
            sceneNames = validScenes
                .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                .ToArray();

            var missingScenes = EditorBuildSettings.scenes
                .Where(s => s.enabled && !File.Exists(s.path))
                .Select(s => Path.GetFileNameWithoutExtension(s.path))
                .ToArray();

            if (missingScenes.Length > 0)
            {
                Debug.LogWarning(
                    $"<color=orange>Scene Switcher:</color> {missingScenes.Length} missing scene(s) in Build Settings:\n" +
                    string.Join(", ", missingScenes)
                );
            }
        }

        SelectCurrentScene();
        Repaint();
    }

    private void SelectCurrentScene()
    {
        string currentScene = Path.GetFileNameWithoutExtension(EditorSceneManager.GetActiveScene().path);
        selectedIndex = System.Array.IndexOf(sceneNames, currentScene);
        if (selectedIndex == -1) selectedIndex = 0;
    }

    private void LoadScene(int index)
    {
        if (index < 0 || index >= scenePaths.Length)
            return;

        string scenePath = scenePaths[index];

        if (string.IsNullOrEmpty(scenePath) || !File.Exists(scenePath))
        {
            Debug.LogWarning($"<color=orange>Scene Switcher:</color> Scene not found: {sceneNames[index]}");
            RefreshSceneList();
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }

    private void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = EditorBuildSettings.scenes.ToList();

        if (!scenes.Any(s => s.path == scenePath))
        {
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            Debug.Log($"<color=green>Scene Switcher:</color> Added '{Path.GetFileName(scenePath)}' to Build Settings");
            RefreshSceneList();
        }
    }
}

// Optional: Add a toolbar icon for quick access
[InitializeOnLoad]
public static class SceneSwitcherQuickAccess
{
    static SceneSwitcherQuickAccess()
    {
        // Add menu item to quickly open current scene's folder
        EditorApplication.contextualPropertyMenu += OnPropertyContextMenu;
    }

    private static void OnPropertyContextMenu(GenericMenu menu, SerializedProperty property)
    {
        menu.AddItem(new GUIContent("Scene Switcher/Open Window"), false, () => SceneSwitcherWindow.ShowWindow());
    }
}
#endif