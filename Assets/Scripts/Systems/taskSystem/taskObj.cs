using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taskObj
{ }


[System.Serializable]
public class task_followNpc
{
    [Header("npc名字")]
    public string npcName;
    [Header("NPC和END距離")]
    public float npcEnd;
    //NPC跟玩家離多遠會停止
    [Header("與NPC的距離")]
    public float distance;
    //------------idle不用理
    [Header("目的地")]
    public Vector3 endPOS;
    [Header("與目標距離")]
    //目的地跟玩家離多遠結束
    public float endDistance;
    //------------idle不用理

    [Header("NPC說的話")]
    public FollowSayType followSayType;
    [Header("是否靜止，只是適用在LINER，對話完任務完成")]
    public bool isIdle;
    [Header("npc對話")]
    public NPCsay[] nPCsays;
    public string[] TalkNPCNames;
    public int currentIndex;

    [Header("是否靠近才說話，默認是離太遠")]
    public bool isNearSay;
    [Header("之後用程式動態改")]
    public Transform npc = null;
}

[System.Serializable]
public class NPCsay
{
    public string whoTalk;
    public string content;
}
public enum FollowSayType
{
    //隨機
    RADOM,
    //線性
    LINER
}

[System.Serializable]
public class task_Walk
{
    [Header("目標位置")]
    public Vector3 taskPos;
    public float distance;
}

[System.Serializable]
public class task_Guide
{
    //對應指導任務
    public int guide;
    //list長度
    [Header("長度")]
    public int ListLength;
}

[System.Serializable]
public class task_Sign
{
    [Header("要開啟的panelID")]
    public int panelId;
}



[System.Serializable]
public class task_Collect
{
    [Header("需要收集的東西")]
    public task_Item[] task_Items;
}

[System.Serializable]
public class task_Item
{
    public BagItem task_item;
    public int task_ItemsAccount;
}