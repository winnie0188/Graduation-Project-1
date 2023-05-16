
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "task", menuName = "Scriptables/task", order = 4)]
public class task : ScriptableObject
{
    public enum TaskType
    {
        // 走到特定位置
        walk,
        // 收集特定道具
        collect

    }

    [Header("基礎屬性")]
    public string task_Title;//任務標題
    [TextArea] public string task_Content;//任務內容


#pragma warning disable CS0414

    [Header("物品類型")]
    public TaskType TaskType_;//物品類型

    // 要到達的地方
    [SerializeField] task_Walk task_Walk;

    // 要收集的道具
    [SerializeField] task_Collect task_Collect;


    private void OnValidate()
    {
        if (TaskType_ != TaskType.walk)
        {
            task_Walk = null;
        }
        if (TaskType_ != TaskType.collect)
        {
            task_Collect = null;
        }
    }
#pragma warning restore CS0414
}
