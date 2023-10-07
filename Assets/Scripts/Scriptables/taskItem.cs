
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

    //要跟隨特定角色
    [SerializeField] task_followNpc task_Follow;

    //要完成的引導任務
    [SerializeField] task_Guide task_Guide;

    //要打開的panel，關閉panel結束任務
    [SerializeField] task_Sign task_Sign;

    [Header("是否生成引導物件")]
    public bool InstantiateNewCircle;
    [Header("是否生成路線")]
    public bool isInstantiatePath;


    [Header("任務完成觸發")]
    //觸發對話
    public FinshTask finshTask;

    public task_Walk gettask_Walk()
    {
        return task_Walk;
    }

    public task_followNpc gettask_Follow()
    {
        return task_Follow;
    }

    public task_Guide gettask_Guide()
    {
        return task_Guide;
    }

    public task_Sign gettask_Sign()
    {
        return task_Sign;
    }
}