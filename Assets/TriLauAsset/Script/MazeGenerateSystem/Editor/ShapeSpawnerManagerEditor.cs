using UnityEngine;
using UnityEditor;
using MyRule;

[CustomEditor(typeof(ShapeSpawnerManager))]
public class ShapeSpawnerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ShapeSpawnerManager manager = (ShapeSpawnerManager)target;

        if (GUILayout.Button("+ Add Spawner"))
        {
            AddNewSpawner(manager);
        }
    }

    private void AddNewSpawner(ShapeSpawnerManager manager)
    {
        GameObject spawnerGO = new GameObject("Spawner_" + manager.spawners.Count);
        spawnerGO.transform.parent = manager.transform;

        GameObject pointA = new GameObject("PointA");
        GameObject pointB = new GameObject("PointB");
        pointA.transform.parent = spawnerGO.transform;
        pointB.transform.parent = spawnerGO.transform;

        pointA.transform.localPosition = manager.transform.position;
        pointB.transform.localPosition = manager.transform.position + new Vector3(10, 0, 0);

        var spawnerScript = spawnerGO.AddComponent<ShapeSpawner>();
        var pointAScript = pointA.AddComponent<MazePoint>();
        var pointBScript = pointB.AddComponent<MazePoint>();

        spawnerScript.pointA = pointAScript;
        spawnerScript.pointB = pointBScript;
        spawnerScript.shapePrefab = manager.spawnerPrefab;

        pointAScript.point = pointA.transform;
        pointBScript.point = pointB.transform;

        var data = new SpawnerData
        {
            spawner = spawnerScript,
            spawnerPrefab = manager.spawnerPrefab,
            pointA = pointAScript,
            pointB = pointBScript,
        };
        manager.spawners.Add(data);

        EditorUtility.SetDirty(manager);
    }
}
