using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene
    {
        MainMenu,
        SpaceStationScene,
        BoardScene,
        LoadingScene,
    }

    private static Scene targetScene;

    public static Scene TargetScene => targetScene;

    public static bool isMultiSceneLoad = false;

    public static void Load(Scene targetScene, bool isMultiSceneLoad = false)
    {
        Loader.targetScene = targetScene;
        Loader.isMultiSceneLoad = isMultiSceneLoad;
        if (!isMultiSceneLoad)
            SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString());
        else
            SceneManager.LoadSceneAsync(Scene.LoadingScene.ToString(), LoadSceneMode.Additive);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadSceneAsync(Loader.targetScene.ToString());
    }

}