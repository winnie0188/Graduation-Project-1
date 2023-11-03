using UnityEngine;

[System.Serializable]
public class FinshTalk
{
    //對話結束的新任務
    public taskItem newTask;
    //對話結束獲得道具
    public BagItem[] bagItem;

    public NPCtask NPCtask;
    public BagItem[] removeBagItem;

    public npcTaskState[] npcTaskState;

    [Header("搭配finshUI")]
    public string FinshContent;

    public bool openFinshUi;

}

[System.Serializable]
public class NPCtask
{
    public string npcName;
    public taskItem taskItem;
}


[System.Serializable]
public class npcTaskState
{
    public string npcName;
    public bool state;
}
