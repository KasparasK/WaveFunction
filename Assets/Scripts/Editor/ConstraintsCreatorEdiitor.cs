using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(WaveFunctionController))]
public class ConstraintsCreatorEdiitor : Editor
{
    private WaveFunctionController waveFunctionController;

    public override void OnInspectorGUI()
    {
        waveFunctionController = (WaveFunctionController)target;

        base.OnInspectorGUI();

        if (GUILayout.Button("Generate constraints"))
        {
            waveFunctionController.CreateConstraints();
        }

        if (GUILayout.Button("Auto generate level"))
        {
            waveFunctionController.AutoGenerate();
        }

        if (GUILayout.Button("Start generation demo"))
        {
            waveFunctionController.GenerationDemo();
        }
    }
}
