using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RaftTime))]
public class RaftTimeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RaftTime targetScript = (RaftTime)target;

        targetScript.TimeScale = EditorGUILayout.FloatField("Time Scale", targetScript.TimeScale);

        EditorGUILayout.FloatField("Delt Time", targetScript.DeltTime);

        EditorGUILayout.FloatField("Real Delt Time", targetScript.RealDeltTime);

        EditorGUILayout.FloatField("Current Time", targetScript.CurrentTime);

        EditorGUILayout.FloatField("Real Current Time", targetScript.RealCurrentTime);
    }

}
