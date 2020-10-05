using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ConstrainsCreator))]
public class ConstraintsCreatorEdiitor : Editor
{
    private ConstrainsCreator constrainsCreator;

    public override void OnInspectorGUI()
    {
        constrainsCreator = (ConstrainsCreator)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Preview level"))
        {
            constrainsCreator.Create();
        }

        if (GUILayout.Button("Generate level"))
        {
            constrainsCreator.Generate();
        }
    }
}
