using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class taskObj
{ }

[System.Serializable]
public class task_Walk
{
    [Header("目標位置")]
    public Vector3 taskPos;
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