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
    //NPC跟玩家離多遠會停止
    [Header("與NPC的距離")]
    public float distance;
    [Header("目的地")]
    public Vector3 endPOS;
    [Header("與目標距離")]
    //目的地跟玩家離多遠結束
    public float endDistance;
    [Header("是否生成引導物件")]
    public bool InstantiateNewCircle;

    [Header("NPC說的話")]
    public string[] NPCsay;

    [Header("之後用程式動態改")]
    public Transform npc = null;
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