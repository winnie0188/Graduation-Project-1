using UnityEditor;

[CustomEditor(typeof(taskItem))]
public class taskInspector : Editor
{
    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Title"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Content"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_need"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_get"));

        var TaskType_ = this.serializedObject.FindProperty("TaskType_");
        EditorGUILayout.PropertyField(TaskType_);

        if (TaskType_.enumNames[TaskType_.enumValueIndex] == "walk")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Walk"));
        }

        if (TaskType_.enumNames[TaskType_.enumValueIndex] == "collect")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("task_Collect"));
        }

        this.serializedObject.ApplyModifiedProperties();
    }
}
