// using UnityEditor;
// using UnityEngine;

// [CustomEditor(typeof(BagManage))]
// public class BagManageEditor : Editor
// {
//     bool isAllShow = false;
//     bool isRigShow = false;
//     bool isLefShow = false;

//     bool iscantArm = false;
//     bool iscanArm = false;


//     public override void OnInspectorGUI()
//     {
//         serializedObject.Update();
//         isAllShow = EditorGUILayout.Foldout(isAllShow, "bagSore");
//         if (isAllShow)
//         {
//             // 右側裝備欄
//             EditorGUI.indentLevel++;
//             isRigShow = EditorGUILayout.Foldout(isRigShow, "右側裝備欄");
//             EditorGUI.indentLevel++;
//             if (isRigShow)
//             {
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("bagSore.potionBagItem"));
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("bagSore.equipmentBagItem"));
//             }
//             EditorGUI.indentLevel -= 2;


//             // 左側裝備欄
//             EditorGUI.indentLevel++;
//             isLefShow = EditorGUILayout.Foldout(isLefShow, "左側裝備欄");
//             EditorGUI.indentLevel++;
//             if (isLefShow)
//             {
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("bagSore.clotheBagItem"));
//             }

//             EditorGUI.indentLevel -= 2;

//             // 無法手持
//             EditorGUI.indentLevel++;
//             iscantArm = EditorGUILayout.Foldout(iscantArm, "無法手持");
//             EditorGUI.indentLevel++;
//             if (iscantArm)
//             {
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("bagSore.materialBagItem"));
//             }
//             EditorGUI.indentLevel -= 2;

//             // 可以手持
//             EditorGUI.indentLevel++;
//             iscanArm = EditorGUILayout.Foldout(iscanArm, "可以手持");
//             EditorGUI.indentLevel++;
//             if (iscanArm)
//             {
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("bagSore.foodBagItem"));
//                 EditorGUILayout.PropertyField(serializedObject.FindProperty("bagSore.toolBagItem"));
//             }
//             EditorGUI.indentLevel -= 2;
//         }

//         serializedObject.ApplyModifiedProperties();
//     }
// }
