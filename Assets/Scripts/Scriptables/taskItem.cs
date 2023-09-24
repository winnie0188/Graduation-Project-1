
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "task", menuName = "Scriptables/task", order = 4)]
public class taskItem : ScriptableObject
{


    [Header("基礎屬性")]
    public string task_Title;//任務標題
    [TextArea] public string task_Content;//任務內容

    [TextArea] public string task_need;//任務要求

    [TextArea] public string task_get;//任務獎勵



    [Header("物品類型")]
    public TaskType TaskType;//物品類型

    // 要到達的地方
    [SerializeField] task_Walk task_Walk;

    // 要收集的道具
    [SerializeField] task_Collect task_Collect;


    [Header("任務完成觸發")]
    //觸發對話
    public FinshTask finshTask;

    public task_Walk gettask_Walk()
    {
        return task_Walk;
    }
}