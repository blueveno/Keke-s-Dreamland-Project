using UnityEngine;
using System.Collections;
using UnityEditor;

using KekeDreamLand;

[CustomEditor(typeof(KekeDreamlandTool))]
public class KekeDreamlandToolEditor : Editor
{
    private bool gridsDisplayed = false;
    private bool bordersDisplayed = false;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        KekeDreamlandTool tools = (KekeDreamlandTool) target;

        EditorGUILayout.LabelField("Boing manipulation :");
        if (GUILayout.Button("Select Boing"))
        {
            tools.SelectBoing();
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Areas manipulation :");

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Toggle grids"))
        {
            gridsDisplayed = !gridsDisplayed;
            tools.DisplayGrid(gridsDisplayed);
        }

        if (GUILayout.Button("Toggle borders"))
        {
            bordersDisplayed = !bordersDisplayed;
            tools.DisplayBorder(bordersDisplayed);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("Need a new button ? Ask to Bib' !", MessageType.Info);
    }
}