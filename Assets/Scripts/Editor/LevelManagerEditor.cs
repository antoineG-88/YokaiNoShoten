using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(LevelManager))]
public class LevelManagerEditor : Editor
{
    private LevelManager levelManager;

    public override void OnInspectorGUI()
    {
        levelManager = (LevelManager)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Load Deco Scene"))
        {
            OpenDecoScene();
        }

        if (GUILayout.Button("Delete zone save file"))
        {
            DeleteSave();
        }
    }

    public void OpenDecoScene()
    {
        if (levelManager.decoScenePath != "")
        {
            levelManager.decoScene = EditorSceneManager.OpenScene(levelManager.decoScenePath, OpenSceneMode.Additive);
        }
        else
        {
            Debug.LogWarning("No path has been set for deco scene");
        }
    }

    public void DeleteSave()
    {
        SaveSystem.DeleteSaveFile(levelManager.zoneName);
    }
}
