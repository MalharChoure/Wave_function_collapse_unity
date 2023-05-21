/// <summary>
/// This script belongs to cowsins™ as a part of the cowsins´ FPS Engine. All rights reserved. 
/// </summary>
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 
/// <summary>
/// Manage UI actions such as displaying a killfeed panel
/// </summary>
public class UIController : MonoBehaviour
{

    [Tooltip("An object showing death events will be displayed on kill")]public bool displayEvents;

    [Tooltip("UI element which contains the killfeed. Where the kilfeed object will be instantiated and parented to"),SerializeField]
    private GameObject killfeedContainer;

    [Tooltip("Object to spawn"),SerializeField] private GameObject killfeedObject;

    public void AddKillfeed(string name)
    {
        GameObject killfeed = Instantiate(killfeedObject, transform.position, Quaternion.identity, killfeedContainer.transform);
        killfeed.transform.GetChild(0).Find("Text").GetComponent<TextMeshProUGUI>().text = "You killed: " + name ;
    }

    public void Hitmarker()
    {
        Instantiate(Resources.Load("Hitmarker"),transform.position,Quaternion.identity,transform);
    }

    public void ChangeScene(int scene) => SceneManager.LoadScene(scene); 

}
#if UNITY_EDITOR
[System.Serializable]
[CustomEditor(typeof(UIController))]
public class UIControllerEditor : Editor
{

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        UIController myScript = target as UIController;

        EditorGUILayout.LabelField("EVENTS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("displayEvents"));
        if (myScript.displayEvents)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("killfeedContainer"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("killfeedObject"));
        }

        serializedObject.ApplyModifiedProperties();

    }
}
#endif