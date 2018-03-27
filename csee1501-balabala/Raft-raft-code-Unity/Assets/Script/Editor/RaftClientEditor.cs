using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RaftClient))]
public class RaftClientEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var targetScript = (RaftClient)target;

        // Show command queue
        EditorGUILayout.LabelField("Command Cache");
        if (targetScript.m_commandCache != null)
        {
            EditorGUI.indentLevel++;
            int index = 0;
            foreach (var command in targetScript.m_commandCache)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(string.Format("Element {0}", index++));
                EditorGUILayout.TextArea(command.ToString());
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.Space();

        // Show historic command list
        EditorGUILayout.LabelField("Histroic Commands");
        if (targetScript.m_historicCommand != null)
        {
            EditorGUI.indentLevel++;
            int index = 0;
            foreach (var command in targetScript.m_historicCommand)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel(string.Format("Element {0}", index++));
                EditorGUILayout.TextArea(command.ToString());
                EditorGUILayout.EndHorizontal();
            }
            EditorGUI.indentLevel--;
        }

        Repaint();
    }
}
