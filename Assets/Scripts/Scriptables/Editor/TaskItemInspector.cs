
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(taskItem))]
public class TaskItemInspector : Editor
{
    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Title"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Content"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_need"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_get"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("InstantiateNewCircle"));


        var TaskType = this.serializedObject.FindProperty("TaskType");
        EditorGUILayout.PropertyField(TaskType);

        if (TaskType.enumNames[TaskType.enumValueIndex] == "walk")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Walk"));
        }

        if (TaskType.enumNames[TaskType.enumValueIndex] == "collect")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Collect"));
        }

        if (TaskType.enumNames[TaskType.enumValueIndex] == "follow")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Follow"));
        }
        if (TaskType.enumNames[TaskType.enumValueIndex] == "guide")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Guide"));
        }
        if (TaskType.enumNames[TaskType.enumValueIndex] == "sign")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Sign"));
        }
        //InstantiateNewCircle
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("finshTask"));

        this.serializedObject.ApplyModifiedProperties();
    }
}
