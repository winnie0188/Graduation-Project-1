using UnityEditor;

[CustomEditor(typeof(MonsterSkill))]
public class MonsterSkillInspector : Editor
{
    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("continued"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("CoolingTime"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("power"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("attackTime"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("STILL"));

        // continued


        var skillType = this.serializedObject.FindProperty("skillType");
        EditorGUILayout.PropertyField(skillType);

        if (skillType.enumNames[skillType.enumValueIndex] == "NONE")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_NONE"));
        }

        if (skillType.enumNames[skillType.enumValueIndex] == "NORMAL")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_NORMAL"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "SHOOT")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_SHOOT"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "COLLISION")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_COLLISION"));
        }

        if (skillType.enumNames[skillType.enumValueIndex] == "ROLL")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_ROLL"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "BUFF")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_BUFF"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "FIRE")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_FIRE"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "RUNAWAY")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_RUNAWAY"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "HOVER")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_HOVER"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "FROMTO")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_FROMTO"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "DEFENSE")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_DEFENSE"));
        }
        if (skillType.enumNames[skillType.enumValueIndex] == "SUMMON")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("SKILL_SUMMON"));
        }

        this.serializedObject.ApplyModifiedProperties();
    }
}
