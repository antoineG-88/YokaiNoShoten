using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoGravityZone))]
public class NoGravityZoneEditor : Editor
{
    private NoGravityZone noGravityZone;

    public override void OnInspectorGUI()
    {
        noGravityZone = (NoGravityZone)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Setup Edge Lines"))
        {
            noGravityZone.SetEdgeLines();
        }
    }
}
