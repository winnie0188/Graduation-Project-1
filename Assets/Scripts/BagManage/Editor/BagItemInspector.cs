using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BagItem))]
public class BagItemInspector : Editor
{
    public override void OnInspectorGUI()
    {
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("BagItem_name"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("BagItem_icon"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("sellPrice"));
        EditorGUILayout.PropertyField(this.serializedObject.FindProperty("BagItem_info"));

        var BagItemType_ = this.serializedObject.FindProperty("BagItemType_");
        EditorGUILayout.PropertyField(BagItemType_);

        if (BagItemType_.enumNames[BagItemType_.enumValueIndex] == "potion")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("potion"));
        }

        if (BagItemType_.enumNames[BagItemType_.enumValueIndex] == "equipment")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("equipment"));
        }
        if (BagItemType_.enumNames[BagItemType_.enumValueIndex] == "clothe")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("clothe"));
        }
        if (BagItemType_.enumNames[BagItemType_.enumValueIndex] == "material")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("material"));
        }

        if (BagItemType_.enumNames[BagItemType_.enumValueIndex] == "food")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("food"));
        }
        if (BagItemType_.enumNames[BagItemType_.enumValueIndex] == "tool")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("tool"));
        }
        if (BagItemType_.enumNames[BagItemType_.enumValueIndex] == "block")
        {
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("block"));
        }


        this.serializedObject.ApplyModifiedProperties();
    }
}
