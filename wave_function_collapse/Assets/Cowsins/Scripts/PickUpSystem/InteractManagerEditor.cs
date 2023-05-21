#if UNITY_EDITOR
/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
using UnityEngine;
using UnityEditor;

[System.Serializable]
[CustomEditor(typeof(InteractManager))]
public class InteractManagerEditor : Editor
{
    private string[] tabs = { "References", "Interaction", "UI","Events" };
    private int currentTab = 0;

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        InteractManager myScript = target as InteractManager;

        Texture2D myTexture = Resources.Load<Texture2D>("CustomEditor/interactManager_CustomEditor") as Texture2D;
        GUILayout.Label(myTexture);

        EditorGUILayout.BeginVertical();
        currentTab = GUILayout.Toolbar(currentTab, tabs);
        EditorGUILayout.Space(10f);
        EditorGUILayout.EndVertical();
        #region variables

        if (currentTab >= 0 || currentTab < tabs.Length)
        {
            switch (tabs[currentTab])
            {
                case "References":
                    EditorGUILayout.LabelField("REFERENCES", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("mask"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponGenericPickeable"));
                    break;
                case "Interaction":
                    EditorGUILayout.LabelField("INTERACTION", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("detectInteractionDistance"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("progressRequiredToInteract"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("interactInterval"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("canDrop"));
                    if(myScript.canDrop)
                    {
                        EditorGUI.indentLevel++; 
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("droppingDistance"));
                        EditorGUI.indentLevel--; 
                    }
                    break;
                case "UI":
                    EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("interactUI"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("interactText"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("forbiddenInteractionUI"));
                    break;
                case "Events":
                    EditorGUILayout.LabelField("OTHERS", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("events"));
                    break;

            }
        }

        #endregion
        EditorGUILayout.Space(10f);
        serializedObject.ApplyModifiedProperties();

    }
}
#endif